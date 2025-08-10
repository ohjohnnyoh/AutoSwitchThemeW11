$ErrorActionPreference = 'Stop'

$iscc = Get-Command ISCC.exe -ErrorAction SilentlyContinue
if (-not $iscc) {
  $candidates = @(
    'C:\Program Files (x86)\Inno Setup 6\ISCC.exe',
    'C:\Program Files\Inno Setup 6\ISCC.exe'
  )
  foreach ($c in $candidates) {
    if (Test-Path $c) { $iscc = $c; break }
  }
}

if (-not $iscc) { throw 'ISCC.exe (Inno Setup 6) not found. Install Inno Setup 6 and add it to PATH.' }

& $iscc "$PSScriptRoot\installer\AutoSwitchThemeW11.iss"
Write-Host "Installer output: $PSScriptRoot\installer\Output" -ForegroundColor Green


