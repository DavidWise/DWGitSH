<#
Visual Studio debug config, in case it gets wiped out (again)

Project Properties - Debug
-- Start Action - Start External Program
	C:\windows\system32\windowspowershell\v1.0\powershell.exe


-- Command line arguments
	-NoExit -Command "C:\path\to\projectdir\devenv.ps1"

I'm not sure why but -NoExit has to be first otherwise it doesn't honor it

#>

Get-Location
Import-Module .\DWGitsh.Extensions.dll -force;
cd PSScripts
.\Start-GitShell.ps1
