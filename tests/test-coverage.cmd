set mydir=%~dp0
set rootdir=%mydir%..\
set resultsdir=%mydir%TestResults\

cd %mydir%
FOR /d /r . %%d IN ("TestResults") DO @IF EXIST "%%d" rd /s /q "%%d"

dotnet test --collect:"XPlat Code Coverage;Format=json,cobertura" ..\src\Serenity.Net.sln

dotnet tool restore
dotnet reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./TestResults/coverage -assemblyfilters:-*.Tests;-*.Test

start ./TestResults/coverage/index.html