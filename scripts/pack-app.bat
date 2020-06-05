PUSHD ..
rd /q /s dist
md dist\assets dist\db
xcopy /E wwwroot\assets .\dist\assets\
xcopy /E wwwroot\db .\dist\db\
x run _bundle.ss -to /dist
copy wwwroot\* dist
copy scripts\deploy\app.settings dist

REM Uncomment if app requires a .NET .dll:
dotnet publish -c release
md dist\plugins 
copy bin\release\netcoreapp3.1\publish\win32.dll dist\plugins\
POPD