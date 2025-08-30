@echo off

:check_dependencies
if not exist "%ProgramFiles%\dotnet\dotnet.exe" (
    echo ERROR: dotnet not found. Please install dotnet to continue.
    goto end
)
if "%VSINSTALLDIR%"=="" (
	set VSINSTALLDIR=%ProgramFiles%\Microsoft Visual Studio\2022\Community
)

if not exist "%VSINSTALLDIR%\MSBuild\Current\Bin\msbuild.exe" (
    echo ERROR: "%VSINSTALLDIR%\MSBuild\Current\Bin\msbuild.exe" not found. Please install Visual Studio to continue.
    goto end
)

for /F "tokens=4 delims=\\" %%G in ("%VSINSTALLDIR%") do set "vsversion=%%G"
if %vsversion% LSS 2022 (
    echo ERROR: This script requires Visual Studio 2022 or newer. You are using version %vsversion%.
    goto end
)

goto run_build

:run_build
echo *** RUNNING BUILD ***
dotnet run --project build\build-serene.csproj --no-dependencies -- %*
if %ERRORLEVEL% GEQ 1 GOTO :error
goto build_nuget_package

:build_nuget_package
echo *** BUILDING NUGET PACKAGE ***
dotnet pack --no-dependencies -p:SkipPatchPackageJson=true vsix\Serene.Templates\Serene.Templates.csproj
if %ERRORLEVEL% GEQ 1 GOTO :error
goto build_vsix_package

:build_vsix_package
echo *** BUILDING VSIX PACKAGE ***
"%VSINSTALLDIR%\MSBuild\Current\Bin\MSBuild.exe" "vsix\Serene.VSIX.sln" -verbosity:m
if %ERRORLEVEL% GEQ 1 GOTO :error
rem start vsix\bin\Serene.Template.vsix
goto install_template

:install_template
echo *** UNINSTALLING THE DOTNET NEW TEMPLATE ***
dotnet new uninstall Serene.Templates
echo *** INSTALLING THE DOTNET NEW TEMPLATE ***
dotnet new install vsix\.nupkg\Serene.Templates*.nupkg

echo *** CREATING PROJECT FROM DOTNET NEW TEMPLATE ***
for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
set ldt=%ldt:~0,4%%ldt:~4,2%%ldt:~6,2%_%ldt:~8,2%%ldt:~10,2%%ldt:~12,2%
mkdir .\.vs
cd .\.vs
dotnet new serene -n SereneTest_%ldt%

cd SereneTest_%ldt%
cd SereneTest_%ldt%.Web
echo *** RUNNING THE PROJECT FIRST TIME ***
dotnet run
echo *** GENERATING CODE FOR VERSIONINFO TABLE ***
dotnet sergen g --connection-key Default --table dbo.VersionInfo --module Default --identifier VersionInfo --permission Administration:General -what *
echo *** RUNNING THE PROJECT SECOND TIME ***
dotnet run

echo *** DROPPING THE TEST DATABASE ***
sqlcmd -S "(localdb)\MSSqlLocalDB" -Q "DECLARE @kill varchar(8000) = ''; SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';' FROM sys.dm_exec_sessions WHERE database_id in (db_id('SereneTest_%ldt%_Default_v1'), db_id('SereneTest_%ldt%_Northwind_v1')) AND is_user_process = 1; EXEC(@kill);DROP DATABASE SereneTest_%ldt%_Default_v1;DROP DATABASE SereneTest_%ldt%_Northwind_v1"

cd ..\..\
rmdir SereneTest_%ldt% /S /Q
cd ..

goto push_confirmation

:push_confirmation

dir /b vsix\.nupkg\*.nupkg
echo.
choice /C PC /N /M "[P]ush NuGet Package, or [C]ancel?: "
if errorlevel 2 goto end
if errorlevel 1 goto push

:push
nuget push -source https://www.nuget.org/api/v2/package .\vsix\.nupkg\Serene.Templates*.nupkg
if %ERRORLEVEL% GEQ 1 GOTO :error
start microsoft-edge:https://marketplace.visualstudio.com/manage/publishers/volkanceylan/extensions/sereneserenityapplicationtemplate/edit
goto end

:error
echo ERROR: An error occurred during the build process.
echo ERROR CODE: %ERRORLEVEL%
pause
goto end

:end
exit /B 0