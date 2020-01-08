@echo off
dotnet tool restore --verbosity quiet
dotnet dotnet-cake .\Build.cake -target=%1 %2 %3 %4 %5
pause