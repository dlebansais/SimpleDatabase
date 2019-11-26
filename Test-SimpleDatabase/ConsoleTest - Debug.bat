@echo off

if not exist "..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" goto error_console
if not exist "..\Test-SimpleDatabase\bin\x64\Debug\Test-SimpleDatabase.dll" goto error_SimpleDatabase
del *.log
"..\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" --trace=Debug --labels=All "..\Test-SimpleDatabase\bin\x64\Debug\Test-SimpleDatabase.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_SimpleDatabase
echo ERROR: Test-SimpleDatabase.dll not built.
goto end

:end
