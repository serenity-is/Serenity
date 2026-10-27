@echo off
setlocal EnableExtensions
pushd "%~dp0.."

set HUSKY=0

echo *** pnpm install ***
call pnpm install
if errorlevel 1 goto :fail

echo *** pnpm -r build ***
call pnpm -r build
if errorlevel 1 goto :fail

echo *** pnpm -r test --coverage ***
call pnpm -r test --coverage
if errorlevel 1 goto :fail

echo *** dtsbundler tests ***
call pnpm --filter @serenity-is/dtsbundler test:dist --coverage
if errorlevel 1 goto :fail

echo *** dotnet build Serenity.slnx ***
call dotnet build Serenity.slnx /p:SkipTSBuild=true
if errorlevel 1 goto :fail

echo *** dotnet test ***
call dotnet test Serenity.slnx --no-build --verbosity normal
if errorlevel 1 goto :fail

popd
exit /b 0

:fail
set EXITCODE=%ERRORLEVEL%
popd
exit /b %EXITCODE%
