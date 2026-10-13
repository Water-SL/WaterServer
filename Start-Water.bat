@echo off
setlocal
echo.
echo   WATER  SCP:SL server host
echo.
where dotnet >nul 2>&1
if errorlevel 1 (
    echo Hata: .NET SDK bulunamadi. https://dot.net adresinden yukle.
    pause
    exit /b 1
)
dotnet build "%~dp0WaterServer\WaterServer.csproj" -c Release --nologo -v q
if errorlevel 1 ( pause & exit /b 1 )
dotnet run --project "%~dp0WaterServer\WaterServer.csproj" -c Release -f net10.0 --no-build
endlocal
