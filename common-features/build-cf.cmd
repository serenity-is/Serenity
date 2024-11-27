@echo off
ECHO *** RUNNING COMMON-FEATURES BUILD ***
dotnet run --project build\build-cf.csproj -- %*
if %ERRORLEVEL% GEQ 1 GOTO :end
if "%~1"=="push" goto :propush

:end
pause
goto :eof

:propush
if not exist %~dp0\..\..\pro-features\build-pro.cmd goto :end
CHOICE /M "Run pro-features\build-pro.cmd %*%"
if %ERRORLEVEL% EQU 1 (
    cd %~dp0\..\..\pro-features
    call build-pro.cmd %*%
)
