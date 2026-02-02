[CmdletBinding(PositionalBinding = $false)]
param (
    [Parameter()]
    [string] $output = "",

    [Parameter()]
    [switch] $clearOutput = $false,

    [Parameter()]
    [string] $version = "",

    [Parameter(ValueFromRemainingArguments)]
    [string[]] $remainingArgs
)

Import-Module $PSScriptRoot\..\Invoke-Process.psm1

if ($clearOutput)
{
    $dir = Resolve-Path -Path ($output -ne "" ? $output : "../artifacts")
    $items = Get-ChildItem -Path $dir -Filter "Translate*nupkg"
    if ($items.Length -gt 0)
    {
        Write-Host "Removing $($items.Length) items (.*nupkg) at '$dir'"
        $items | ForEach-Object { Remove-Item -Path $_}
    }
}

$projects = Get-ChildItem -Include "*.csproj" -Recurse -Name | Resolve-Path -Relative

$cmd = "dotnet", "pack"
$arguments = "-c", "Release"

foreach ($project in $projects) {
    $expression = $cmd + $project + $arguments;
    if ($output -ne "") {
        $expression += "-o", $output
    }
    if ($version -ne "") {
        $expression += "-p:Version=$version", "-p:DisableGitVersionTask=true"
    }
    $expression = ($expression + $remainingArgs) -join " "
    Write-Host "$ $expression"
    Invoke-Process $expression
}
