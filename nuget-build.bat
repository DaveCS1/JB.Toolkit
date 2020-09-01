cd JB.Toolkit
del /q /f *.nupkg
nuget.exe pack -IncludeReferencedProjects -properties Configuration=Release