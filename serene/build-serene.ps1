#Requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $RemainingArgs
)

$ErrorActionPreference = 'Continue'

function Stop-WithError {
    param(
        [string] $Message = 'An error occurred during the build process.',
        [int] $ExitCode = 1
    )
    Write-Host "ERROR: $Message"
    Write-Host "ERROR CODE: $ExitCode"
    if (-not [Console]::IsInputRedirected) {
        Read-Host 'Press Enter to exit' | Out-Null
    }
    exit $ExitCode
}

function Assert-ExitCode {
    param([int] $ExitCode = $LASTEXITCODE)
    if ($ExitCode -ne 0) { Stop-WithError -ExitCode $ExitCode }
}

function Invoke-DotnetRun {
    param([string] $WorkingDirectory = $PWD)

    $proc = Start-Process -FilePath $dotnet -ArgumentList 'run' -WorkingDirectory $WorkingDirectory -NoNewWindow -PassThru
    if ([Console]::IsInputRedirected) {
        $proc.WaitForExit()
        return $proc.ExitCode
    }

    $stoppedByUser = $false
    $controlC = [Console]::TreatControlCAsInput
    [Console]::TreatControlCAsInput = $true
    try {
        while (-not $proc.HasExited) {
            if ([Console]::KeyAvailable) {
                $key = [Console]::ReadKey($true)
                if (($key.Modifiers -band [ConsoleModifiers]::Control) -and ($key.Key -eq [ConsoleKey]::C)) {
                    & taskkill /PID $proc.Id /T /F 2>&1 | Out-Null
                    $stoppedByUser = $true
                    break
                }
            }
            Start-Sleep -Milliseconds 100
        }
        $proc.WaitForExit()
    }
    finally {
        [Console]::TreatControlCAsInput = $controlC
    }

    if ($stoppedByUser) { return 0 }
    return $proc.ExitCode
}

$root = $PSScriptRoot
Set-Location -LiteralPath $root

# check_dependencies
$dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
if (-not (Test-Path -LiteralPath $dotnet)) {
    Stop-WithError -Message 'dotnet not found. Please install dotnet to continue.'
}

$vsInstallDir = $env:VS2026INSTALLDIR
if ([string]::IsNullOrEmpty($vsInstallDir)) {
    $vsInstallDir = Join-Path $env:ProgramFiles 'Microsoft Visual Studio\2026\Community'
    if (-not (Test-Path -LiteralPath $vsInstallDir)) {
        $vsInstallDir = Join-Path $env:ProgramFiles 'Microsoft Visual Studio\18\Community'
    }
}

$msbuild = Join-Path $vsInstallDir 'MSBuild\Current\Bin\msbuild.exe'
if (-not (Test-Path -LiteralPath $msbuild)) {
    Stop-WithError -Message "`"$msbuild`" not found. Please install Visual Studio to continue."
}

# run_build
Write-Host '*** RUNNING BUILD ***'
& $dotnet run --project (Join-Path $root 'build\build-serene.csproj') --no-dependencies --no-launch-profile -- @RemainingArgs
Assert-ExitCode

# build_nuget_package
Write-Host '*** BUILDING NUGET PACKAGE ***'
& $dotnet pack --no-dependencies -p:SkipPatchPackageJson=true (Join-Path $root 'vsix\Serene.Templates\Serene.Templates.csproj')
Assert-ExitCode

# build_vsix_package
Write-Host '*** BUILDING VSIX PACKAGE ***'
& $msbuild (Join-Path $root 'vsix\Serene.VSIX.slnx') -verbosity:m /p:EnableProjectDataOnBuild=false
Assert-ExitCode

# install_template
Write-Host '*** UNINSTALLING THE DOTNET NEW TEMPLATE ***'
& $dotnet new uninstall Serene.Templates
Write-Host '*** INSTALLING THE DOTNET NEW TEMPLATE ***'
& $dotnet new install (Join-Path $root 'vsix\.nupkg\Serene.Templates*.nupkg')
Assert-ExitCode

Write-Host '*** CREATING PROJECT FROM DOTNET NEW TEMPLATE ***'
$ldt = Get-Date -Format 'yyyyMMdd_HHmmss'
$projectName = "SereneTest_$ldt"

$vsDir = Join-Path $root '.vs'
New-Item -ItemType Directory -Force -Path $vsDir | Out-Null
$directoryPackagesProps = Join-Path $vsDir 'Directory.Packages.props'
if (-not (Test-Path -LiteralPath $directoryPackagesProps)) {
    Set-Content -LiteralPath $directoryPackagesProps -Value '<Project></Project>' -NoNewline -Encoding UTF8
}
Set-Location -LiteralPath $vsDir

& $dotnet new serene -n $projectName
Assert-ExitCode

$webDir = Join-Path $vsDir "$projectName\$projectName.Web"
Set-Location -LiteralPath $webDir

Write-Host '*** RUNNING THE PROJECT FIRST TIME ***'
Assert-ExitCode -ExitCode (Invoke-DotnetRun -WorkingDirectory $webDir)

Write-Host '*** GENERATING CODE FOR VERSIONINFO TABLE ***'
& $dotnet sergen g --connection-key Default --table dbo.VersionInfo --module Default --identifier VersionInfo --permission Administration:General -what '*'
Assert-ExitCode

Write-Host '*** RUNNING THE PROJECT SECOND TIME ***'
Assert-ExitCode -ExitCode (Invoke-DotnetRun -WorkingDirectory $webDir)

Write-Host '*** DROPPING THE TEST DATABASE ***'
$query = "DECLARE @kill varchar(8000) = ''; SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';' FROM sys.dm_exec_sessions WHERE database_id in (db_id('SereneTest_${ldt}_Default_v1'), db_id('SereneTest_${ldt}_Northwind_v1')) AND is_user_process = 1; EXEC(@kill);DROP DATABASE SereneTest_${ldt}_Default_v1;DROP DATABASE SereneTest_${ldt}_Northwind_v1"
& sqlcmd -S '(localdb)\MSSqlLocalDB' -Q $query
Assert-ExitCode

Set-Location -LiteralPath $vsDir
Remove-Item -LiteralPath (Join-Path $vsDir $projectName) -Recurse -Force
Set-Location -LiteralPath $root

# push_confirmation
Get-ChildItem -Path (Join-Path $root 'vsix\.nupkg') -Filter '*.nupkg' -Name -ErrorAction SilentlyContinue
Write-Host ''
do {
    $choice = Read-Host '[P]ush NuGet Package, or [C]ancel? (P/C)'
} while ($choice -notmatch '^[PpCc]$')
if ($choice -notmatch '^[Pp]') { exit 0 }

& nuget push -source https://nuget.org (Join-Path $root 'vsix\.nupkg\Serene.Templates*.nupkg')
Assert-ExitCode
Start-Process 'microsoft-edge:https://marketplace.visualstudio.com/manage/publishers/volkanceylan/extensions/sereneserenityapplicationtemplate/edit'

exit 0
