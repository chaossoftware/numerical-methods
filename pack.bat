@echo off
set SRC_ROOT=%1
set OUT_DIR=%2

for %%p in (ChaosSoft.Core) do (dotnet pack %SRC_ROOT%\%%p\%%p.csproj -o %OUT_DIR% -c Release --no-build -p:NuspecFile=%SRC_ROOT%\%%p\%%p.nuspec)