@IF EXIST "%~dp0\node.exe" (
    "%~dp0\node.exe"  "%~dp0\less\lib\lessc.js" %*
) ELSE (
    node  "%~dp0\less\lib\lessc.js" %*
)