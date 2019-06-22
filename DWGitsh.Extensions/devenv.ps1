<#
Visual Studio debug config, in case it gets wiped out (again)

Project Properties -> Debug -> 
	Start Action -> Start External Program
	it might also be under Launch -> Executable

	C:\windows\system32\windowspowershell\v1.0\powershell.exe

	-- Command line arguments  ( or Application Arguments)
	-NoExit -Command "C:\path\to\projectdir\devenv.ps1"

	I'm not sure why but -NoExit has to be first otherwise it doesn't honor it

#>

Import-Module .\DWGitsh.Extensions.dll -force;
cd PSScripts
.\Start-GitShell.ps1
