$msbuild = "C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"

cd ..\FelicaLib_Remastered
Invoke-Expression ($msbuild + " FelicaLib_Remastered.sln /p:Configuration=Release /t:Clean")
Invoke-Expression ($msbuild + " FelicaLib_Remastered.sln /p:Configuration=Release /t:Rebuild")

cd .\FelicaLib_DotNet
.\NuGetPackup.exe

move *.nupkg ..\..\Published -Force
