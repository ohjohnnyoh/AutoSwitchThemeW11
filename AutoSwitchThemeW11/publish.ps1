param(
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$rid = 'win-x64'
$outDir = Join-Path $PSScriptRoot 'publish' | Join-Path -ChildPath $rid

dotnet publish $PSScriptRoot\AutoSwitchThemeW11.csproj `
    -c $Configuration `
    -r $rid `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=false `
    -p:SelfContained=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $outDir

Write-Host "Output: $outDir" -ForegroundColor Green



