# launch the 

# Import-Module .\DWGitsh.Extensions.dll -force;

$PSCommand = "Import-Module .\DWGitsh.Extensions.dll -force; cd PSScripts; .\Start-GitShell.ps1;"
$curDir = (Get-location).Path

$devenvPath = "..\..\..\devenv.ps1"

write-host "You will need to manually load the dev environment in Windows Terminal to begin debugging" -ForegroundColor Yellow
write-host "as Windows Terminal does not yet support passing arguments to powershell commands as part of the startup" -ForegroundColor Yellow

$pid
Write-Host ""
write-host "Type: (or paste from clipboard)" -ForegroundColor Cyan

Write-Host ""

Write-Host "$devenvPath" -ForegroundColor Green
Set-Clipboard $devenvPath

Write-Host ""

Write-Host "Press [Enter] to launch Windows Terminal... "
Read-Host

$wtArgs = @("--title `"DWGitsh Dev Env`"", "-d `"$curDir`"", "powershell.exe")

& WT --title "DWGitsh Dev Env" -d "$curDir" "powershell.exe"

#Start-Process  -FilePath "WT" -ArgumentList $wtArgs -Wait

#& wt new-tab --title "DWGitsh Dev Env" -d "$curDir" #"powershell.exe -Command write-host"'

#Stop-Process $pid