#Requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string] $Version
)

$ErrorActionPreference = 'Stop'
$startSharpRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$release = Join-Path $startSharpRoot 'release.ps1'

& $release retry $Version serene:build
exit $LASTEXITCODE
