cd JB.Toolkit
del /q /f *.nupkg
nuget.exe pack -IncludeReferencedProjects -properties Configuration=Release
nuget.exe push JB.Toolkit.1.0.1.3.nupkg -Source https://api.nuget.org/v3/index.json -Timeout 300