:: Created by npm, please don't edit manually.
@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe" "%~dp0\node_modules\clean-css\bin\cleancss" %*
) ELSE (
  node "%~dp0\node_modules\Clean-css\bin\cleancss" %*
)
