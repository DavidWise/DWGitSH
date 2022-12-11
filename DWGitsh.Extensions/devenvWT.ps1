# launches the Windows Terminal using the profile configured

$tabColor = "#0A9999"

$curFolder = get-item .
$curDir = $curFolder.Fullname

$rootFolder = get-item $env:VSProjectDir

$devenvPath = "$($env:VSProjectDir)devenv.ps1"

$windowTitle = "DWGitsh Dev Env - $($curFolder.Name)"

$cmdArgs = "new-tab --title `"$($windowTitle)`" --tabColor `"$tabColor`" -d `"$curDir`""
$cmdArgs +=" pwsh.exe -noexit -File `"$devenvPath`" "

$startExe = New-Object System.Diagnostics.ProcessStartInfo
$startExe.Arguments = $cmdArgs
$startExe.WorkingDirectory = "$curDir"
$startExe.FileName = "WT"

$proc = [System.Diagnostics.Process]::Start($startExe)

Start-sleep -seconds 2

$runningProc = get-process | where {$_.MainWindowTitle -eq $windowTitle} 
$runningProc.WaitForExit()
