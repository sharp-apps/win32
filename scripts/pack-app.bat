PUSHD ..
if exist dist\.publish copy dist\.publish scripts\deploy 
rd /q /s dist
x run _bundle.ss -to /dist
copy wwwroot\* dist
copy scripts\deploy\* dist

REM Uncomment if app requires a .NET .dll:
dotnet publish -c release
md dist\plugins 
copy bin\release\netcoreapp3.1\publish\win32.dll dist\plugins\
POPD
