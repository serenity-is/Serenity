@echo off
ECHO *** RUNNING BUILD ***
dotnet run --project build\build-cf.csproj -- %*
if %ERRORLEVEL% GEQ 1 GOTO :end
:end
pause