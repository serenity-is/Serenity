@IF EXIST "%~dp0\node.exe" (
  @IF EXIST "%~dp0\less\lib\lessc" (
    "%~dp0\node.exe"  "%~dp0\less\lib\lessc" %*
  ) ELSE (
    "%~dp0\node.exe" "%~dp0\less\bin\lessc" %*
  )
) ELSE (
  @IF EXIST "%~dp0\less\lib\lessc" (
    node  "%~dp0\less\lib\lessc" %*
  ) ELSE (
    node  "%~dp0\less\bin\lessc" %*
  )
)