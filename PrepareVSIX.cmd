@echo off
"Tools\Cake\Cake.exe" Samples\BasicApplication\VSIX\PrepareVSIX.cake %*
"c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\ide\devenv.exe" Samples\BasicApplication\VSIX\BasicApplication.VSIX.csproj /build
start Samples\BasicApplication\VSIX\bin\Debug
pause