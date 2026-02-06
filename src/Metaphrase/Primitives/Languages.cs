using Metaphrase.Primitives.Events;
using Metaphrase.Primitives.Internal;
using System.Runtime.CompilerServices;

namespace Metaphrase.Primitives;

/// <summary>
/// Manages a collection of language translations.
/// </summary>
/// <remarks>Language keys are compared using <see cref="StringComparer.OrdinalIgnoreCase"/></remarks>
public sealed class Languages : IDisposable
{
    private readonly Subject<LanguageTranslationChangeEvent> onTranslationChange = new();
    private readonly ConcurrentLazyDictionary<string, TranslationsWrapper> store;

    /// <summary>
    /// Gets an observable sequence that notifies when a language translation changes.
    /// </summary>
    public Observable<LanguageTranslationChangeEvent> OnTranslationChange => onTranslationChange.AsObservable();

    /// <summary>
    /// Initializes a new instance of the <see cref="Languages"/> class.
    /// </summary>
    public Languages()
    {
        store = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Languages"/> class with the specified initial languages.
    /// </summary>
    /// <param name="languages">The initial languages and their translations.</param>
    public Languages(IDictionary<string, Translations> languages)
    {
        var observer = onTranslationChange.AsObserver();
        var wrapped = languages.Select(kv => new TranslationsWrapper(kv, observer).ToKv(kv.Key));
        store = new(wrapped, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the collection contains the specified language key.
    /// </summary>
    /// <param name="key">The language key to locate in the collection.</param>
    /// <returns>true if the collection contains an element with the specified key; otherwise, false.</returns>
    public bool Contains(string key)
    {
        return store.ContainsKey(key);
    }

    /// <summary>
    /// Tries to get the translations for the specified language key.
    /// </summary>
    /// <param name="key">The language key.</param>
    /// <param name="result">When this method returns, contains the translations associated with the specified key, if the key is found; otherwise, null.</param>
    /// <returns>true if the language key is found; otherwise, false.</returns>
    public bool TryGet(string key, [NotNullWhen(true)] out Translations? result)
    {
        if (store.TryGetValue(key, out var lazy))
        {
            result = lazy.Value.Inner;
            return true;
        }
        Unsafe.SkipInit(out result);
        return false;
    }

    /// <summary>
    /// Gets the translations for the specified language key.
    /// </summary>
    /// <param name="key">The language key.</param>
    /// <returns>The translations for the specified language key.</returns>
    public Translations Get(string key)
    {
        var onTranslationChange = this.onTranslationChange;
        var value = store.GetOrAdd(key, key => new TranslationsWrapper(key, new(), onTranslationChange.AsObserver()));
        return value.Inner;
    }

    /// <summary>
    /// Sets the translations for the specified language key.
    /// </summary>
    /// <param name="key">The language key.</param>
    /// <param name="value">The translations to set.</param>
    public void Set(string key, Translations value)
    {
        var onTranslationChange = this.onTranslationChange;
        store.AddOrUpdate(
            key: key,
            addFactory: key => new TranslationsWrapper(key, new(), onTranslationChange.AsObserver()),
            updateFactory: (key, previous) =>
            {
                previous.Dispose();
                return new TranslationsWrapper(key, value, onTranslationChange.AsObserver());
            }
        );
    }

    /// <summary>
    /// Removes the translations for the specified language key.
    /// </summary>
    /// <param name="key">The language key.</param>
    public void Remove(string key)
    {
        if (store.TryRemove(key, out var value) && value.IsValueCreated)
        {
            value.Value.Dispose();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        onTranslationChange.Dispose();
    }

    private readonly struct TranslationsWrapper : IDisposable
    {
        private readonly IDisposable sub;

        public Translations Inner { get; }

        public TranslationsWrapper(string key, Translations inner, Observer<LanguageTranslationChangeEvent> observer)
        {
            Inner = inner;
            sub = inner.OnTranslationChange.Select(key, static (on, key) => new LanguageTranslationChangeEvent(key, on.Key, on.Translation)).Subscribe(observer);
        }

        public TranslationsWrapper(KeyValuePair<string, Translations> kv, Observer<LanguageTranslationChangeEvent> observer)
        {
            Inner = kv.Value;
            sub = kv.Value.OnTranslationChange.Select(kv.Key, static (on, key) => new LanguageTranslationChangeEvent(key, on.Key, on.Translation)).Subscribe(observer);
        }

        public KeyValuePair<string, TranslationsWrapper> ToKv(string key)
        {
            return KeyValuePair.Create(key, this);
        }

        public void Dispose()
        {
            sub.Dispose();
        }
    }
}
