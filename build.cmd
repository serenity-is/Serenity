@echo off
dotnet run --project %~dp0\build\build.csproj -- %*%
if "%~1"=="push" goto :cfpush
pause
goto :eof

:cfpush
echo.
CHOICE /M "Run common-features\build-cf.cmd %*%"
If %ERRORLEVEL% EQU 1 (
    cd %~dp0\common-features
    call build-cf.cmd %*%
)
