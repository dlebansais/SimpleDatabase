@echo off

if not exist ".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" goto error_console1
if not exist ".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" goto error_console2
if not exist ".\Test-SimpleDatabase\bin\x64\Debug\Test-SimpleDatabase.dll" goto error_not_built
if not exist ".\Test-SimpleDatabase\bin\x64\Release\Test-SimpleDatabase.dll" goto error_not_built
if exist .\Test-SimpleDatabase\*.log del .\Test-SimpleDatabase\*.log
if exist .\Test-SimpleDatabase\obj\x64\Debug\Coverage-SimpleDatabase-Debug_coverage.xml del .\Test-SimpleDatabase\obj\x64\Debug\Coverage-SimpleDatabase-Debug_coverage.xml
if exist .\Test-SimpleDatabase\obj\x64\Release\Coverage-SimpleDatabase-Release_coverage.xml del .\Test-SimpleDatabase\obj\x64\Release\Coverage-SimpleDatabase-Release_coverage.xml
".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" -targetargs:".\Test-SimpleDatabase\bin\x64\Debug\Test-SimpleDatabase.dll --trace=Debug --labels=All" -filter:"+[SimpleDatabase*]* -[Test-SimpleDatabase*]*" -output:".\Test-SimpleDatabase\obj\x64\Debug\Coverage-SimpleDatabase-Debug_coverage.xml" -showunvisited
".\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" -register:user -target:".\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe" -targetargs:".\Test-SimpleDatabase\bin\x64\Release\Test-SimpleDatabase.dll --trace=Debug --labels=All" -filter:"+[SimpleDatabase*]* -[Test-SimpleDatabase*]*" -output:".\Test-SimpleDatabase\obj\x64\Release\Coverage-SimpleDatabase-Release_coverage.xml" -showunvisited
if exist .\Test-SimpleDatabase\obj\x64\Debug\Coverage-SimpleDatabase-Debug_coverage.xml .\packages\Codecov.1.1.1\tools\codecov -f ".\Test-SimpleDatabase\obj\x64\Debug\Coverage-SimpleDatabase-Debug_coverage.xml" -t "f8a67ae5-78f5-411d-8683-7077ba4e738f"
if exist .\Test-SimpleDatabase\obj\x64\Release\Coverage-SimpleDatabase-Release_coverage.xml .\packages\Codecov.1.1.1\tools\codecov -f ".\Test-SimpleDatabase\obj\x64\Release\Coverage-SimpleDatabase-Release_coverage.xml" -t "f8a67ae5-78f5-411d-8683-7077ba4e738f"
goto end

:error_console1
echo ERROR: OpenCover.Console not found.
goto end

:error_console2
echo ERROR: nunit3-console not found.
goto end

:error_not_built
echo ERROR: Test-SimpleDatabase.dll not built (both Debug and Release are required).
goto end

:end
