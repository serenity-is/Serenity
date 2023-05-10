@echo off
dotnet tool restore --verbosity quiet
dotnet dotnet-cake %~dp0\_build.cake -target=%1 %2 %3 %4 %5
pause