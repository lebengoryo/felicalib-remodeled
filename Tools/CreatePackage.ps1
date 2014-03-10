$msbuild = "C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"

cd ..\FelicaLib_Remodeled\FelicaLib_DotNet
Invoke-Expression ($msbuild + " FelicaLib_DotNet.csproj /p:Configuration=Release /t:Clean")
Invoke-Expression ($msbuild + " FelicaLib_DotNet.csproj /p:Configuration=Release /t:Rebuild")

.\NuGetPackup.exe

move *.nupkg ..\..\Published -Force
explorer ..\..\Published
