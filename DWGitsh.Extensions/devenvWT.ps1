# launch the 

# Import-Module .\DWGitsh.Extensions.dll -force;

$PSCommand = "Import-Module .\DWGitsh.Extensions.dll -force; cd PSScripts; .\Start-GitShell.ps1;"

$curFolder = get-item .
$curDir = $curFolder.Fullname

$rootFolder = get-item $env:VSProjectDir

$devenvPath = "$($env:VSProjectDir)devenv.ps1"

#write-host "You will need to manually load the dev environment in Windows Terminal to begin working" -ForegroundColor Yellow
#write-host "as Windows Terminal does not yet support passing arguments to powershell commands as part of the startup" -ForegroundColor Yellow

#Write-Host ""
#Write-Host "Note: As of now, debugging is not possible from a Windows Terminal session" -ForegroundColor Red
#Write-Host ""

#Write-Host ""
#write-host "Type: (or paste from clipboard)" -ForegroundColor Cyan

#Write-Host ""

#Write-Host "Working : $curDir" -ForegroundColor Green
#Write-Host "script  : $devenvPath" -ForegroundColor Green
#Set-Clipboard $devenvPath

#Write-Host ""

#Write-Host "Press [Enter] to launch Windows Terminal... "
#Read-Host

$wtArgs = @("WT", "--title `"DWGitsh Dev Env`"", "-d `"$curDir`"", "powershell.exe")

$windowTitle = "DWGitsh Dev Env - $($curFolder.Name)"

$cmdArgs = "new-tab --title `"$($windowTitle)`" --tabColor '#0A9999' -d `"$curDir`""
$cmdArgs +=" pwsh.exe -noexit -File `"$devenvPath`" "

$startExe = New-Object System.Diagnostics.ProcessStartInfo
$startExe.Arguments = $cmdArgs
$startExe.WorkingDirectory = "$curDir"
$startExe.FileName = "WT"

$proc = [System.Diagnostics.Process]::Start($startExe)

Start-sleep -seconds 2

$runningProc = get-process | where {$_.MainWindowTitle -eq $windowTitle} 
$runningProc.WaitForExit()
