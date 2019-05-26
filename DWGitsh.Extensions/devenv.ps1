<#
Visual Studio debug config, in case it gets wiped out (again)

Project Properties - Debug
-- Start Action - Start External Program
	C:\windows\system32\windowspowershell\v1.0\powershell.exe


-- Command line arguments
	-NoExit -Command "C:\sw\spike\gitsh\devenv.ps1"

I'm not sure why but -NoExit has to be first otherwise it doesn't honor it

#>

cd C:\SW\Spike\gitsh\Gitsh.Extensions;
Import-Module .\bin\Debug\Gitsh.Extensions.dll -force;
cd PSScripts
.\Start-GitShell.ps1
