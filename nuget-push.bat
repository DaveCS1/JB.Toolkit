cd JBToolkit
del /q /f *.nupkg
nuget.exe pack -IncludeReferencedProjects -properties Configuration=Release
nuget.exe push JBToolkit.1.0.0.2 -Source https://api.nuget.org/v3/index.json -Timeout 300