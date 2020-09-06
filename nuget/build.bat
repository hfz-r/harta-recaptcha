"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe" "..\library\Recaptcha.csproj" /t:Rebuild /property:Configuration=Release

copy "..\library\bin\Release\*" ".\recaptcha\lib\.NetFramework 4.0"

nuget.exe pack ".\recaptcha\recaptcha.nuspec" -OutputDirectory ".\packages\ "

