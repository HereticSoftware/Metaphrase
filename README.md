# Translate

[![Build Action](https://github.com/HereticSoftware/Translate/actions/workflows/build.yaml/badge.svg)](https://github.com/HereticSoftware/Translate/actions/workflows/build.yaml)
[![Publish Action](https://github.com/HereticSoftware/Translate/actions/workflows/publish.yaml/badge.svg)](https://github.com/HereticSoftware/Translate/actions/workflows/publish.yaml)
[![License](https://img.shields.io/github/license/HereticSoftware/Translate?style=flat)](https://github.com/HereticSoftware/Translate/blob/main/LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET%208-%23512bd4?style=flat)](https://dotnet.microsoft.com/)
[![.NET 9](https://img.shields.io/badge/.NET%209-%23512bd4?style=flat)](https://dotnet.microsoft.com/)
[![Downloads](https://img.shields.io/nuget/dt/Translate?style=flat)](https://www.nuget.org/packages/Translate.Contracts/)

A C# port of the [ngx-Translate](https://github.com/ngx-Translate/core). The port is not one to one and also aims to be more C# friendly where possible.

The library provides you with a `TranslateService` which combined with a `TranslateLoader` (HttpLoader built in) enables you to load, compile and display your translations using formatting with keys. For much stronger formatting there exists a supporting package that uses the awesome [SmartFormat](https://github.com/axuno/SmartFormat/) package.

## Packages

| Package | Stable | Pre |
|:--|:--|:--|
| **Translate** | [![Translate](https://img.shields.io/nuget/v/Translate)](https://www.nuget.org/packages/Translate) | [![Translate](https://img.shields.io/nuget/vpre/Translate)](https://www.nuget.org/packages/Translate) |
| **Translate.SmartFormat** | [![Translate.SmartFormat](https://img.shields.io/nuget/v/Translate.SmartFormat)](https://www.nuget.org/packages/Translate.SmartFormat) | [![Translate.SmartFormat](https://img.shields.io/nuget/vpre/Translate.SmartFormat)](https://www.nuget.org/packages/Translate.SmartFormat) |

# Usage

Description
- Translate
    - Contains the `abstractions`, `defaults`, `primitives`, `http loader` and the `service`.
- Translate.SmartFormat
    - Contains the `SmartFormatParser`.

Installation:
- `Translate` in projects that you want to use the service or any of the primitives.
- `Translate.SmartFormat` in projects that use the service and you want to replace the default parser.

# Getting Started

The section will describe how to get started with Translate in a `Blazor Wasm` using the `HttpLoader` storing the language files at `wwwroot/i18n`.

1. Add the `Translate` package.
```console
dotnet add package Translate
```
2. Add the appropriate services to the service provider.
```csharp
services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddScoped<TranslateLoader, TranslateHttpLoader>(); // This will use the default options
services.AddScoped<TranslateService>(); // This will use the default parser
```
3. Use the service

`TranslateService` also supports a "pipe" syntax to get your translation values.

For the tanslation `key: hello` and `value: Hello` you can do:
```csharp
Translate.Instant("hello") // prints "Hello"
Translate | "hello" // prints "Hello"
```

For the translation `key: welcome`, `value: Welcome {user}!` and `param: user`.
```csharp
Translate.Instant("hello", new { user = "panos" }) // prints "Welcome panos"!
Translate | "hello" | new { user = "panos" } // prints "Welcome panos"!
```

# Contributing

For general contribution information you can read the [Raven Tail Contributing document](https://github.com/HereticSoftware/.github/blob/main/CONTRIBUTING.md).

## Local Development

To develop you need:
1. dotnet 9.0 SDK
2. Visual Studio or VS Code with the C# extension.
3. Configured your IDE for the [TUnit](https://thomhurst.github.io/TUnit/) library.
