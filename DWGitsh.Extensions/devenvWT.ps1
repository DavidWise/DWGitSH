# launch the 

# Import-Module .\DWGitsh.Extensions.dll -force;

$PSCommand = "Import-Module .\DWGitsh.Extensions.dll -force; cd PSScripts; .\Start-GitShell.ps1;"
$curDir = (Get-location).Path

$devenvPath = "..\..\..\devenv.ps1"

write-host "You will need to manually load the dev environment in Windows Terminal to begin working" -ForegroundColor Yellow
write-host "as Windows Terminal does not yet support passing arguments to powershell commands as part of the startup" -ForegroundColor Yellow

Write-Host ""
Write-Host "Note: As of now, debugging is not possible from a Windows Terminal session" -ForegroundColor Red
Write-Host ""

Write-Host ""
write-host "Type: (or paste from clipboard)" -ForegroundColor Cyan

Write-Host ""

Write-Host "$devenvPath" -ForegroundColor Green
Set-Clipboard $devenvPath

Write-Host ""

Write-Host "Press [Enter] to launch Windows Terminal... "
Read-Host

$wtArgs = @("WT", "--title `"DWGitsh Dev Env`"", "-d `"$curDir`"", "powershell.exe")


$startExe = New-Object System.Diagnostics.ProcessStartInfo
$startExe.Arguments = "--title `"DWGitsh Dev Env`" -d `"$curDir`" `"powershell.exe`""
$startExe.WorkingDirectory = $curDir
$startExe.FileName = "WT"


$proc = [System.Diagnostics.Process]::Start($startExe)

