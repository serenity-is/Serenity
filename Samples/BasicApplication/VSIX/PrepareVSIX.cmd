@echo off
"..\..\..\Tools\Cake\Cake.exe" .\PrepareVSIX.cake %*
"c:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\ide\devenv.exe" BasicApplication.VSIX.csproj /build
pause