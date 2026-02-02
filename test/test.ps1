Import-Module $PSScriptRoot\..\Invoke-Process.psm1

$projects = Get-ChildItem -Include "*.csproj" -Recurse -Name | Resolve-Path -Relative

$cmd = "dotnet", "test"
$arguments = "-c", "Release"

foreach ($project in $projects) {
    $expression = ($cmd + $project + $arguments + $args) -join " ";
    Write-Host "$ $expression"
    Invoke-Process $expression
}
