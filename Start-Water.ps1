#!/usr/bin/env pwsh
# Water Launcher - SCP:SL sunucusunu başlatır
#Requires -Version 5

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

$project = Join-Path $root "WaterServer\WaterServer.csproj"

if (-not (Test-Path $project)) {
    Write-Host "Hata: WaterServer.csproj bulunamadi. Launcher'i Water-SCPSL klasorunden calistir." -ForegroundColor Red
    exit 1
}

$sdk = & dotnet --list-sdks 2>$null | Select-Object -Last 1
if (-not $sdk) {
    Write-Host "Hata: .NET SDK bulunamadi. https://dot.net adresinden yukle." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "  WATER  SCP:SL server host" -ForegroundColor Cyan
Write-Host "  Derleniyor..." -ForegroundColor DarkGray
Write-Host ""

dotnet build $project -c Release --nologo -v q
if ($LASTEXITCODE -ne 0) {
    Write-Host "Derleme basarisiz." -ForegroundColor Red
    exit $LASTEXITCODE
}

dotnet run --project $project -c Release -f net10.0 --no-build @args
