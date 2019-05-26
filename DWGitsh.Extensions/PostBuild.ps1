
param(
    [string] $ProjectDir,
    [string] $TargetDir
)

# Meant to be called from the Post-Build Event
#  powershell.exe -NoLogo -NoProfile -File $(SolutionDir)PostBuild.ps1 -ProjectDir $(SolutionDir) -TargetDir $(TargetDir)


$deployFolder = "$($ProjectDir)Deploy"

write-output "Copying files to $deployFolder"
Copy-Item "$($projectDir)PSScripts\defaultColors.csv" -destination $deployFolder -Force
Copy-Item "$($projectDir)PSScripts\gitsh-LibUtils.ps1" -destination $deployFolder -Force
Copy-Item "$($projectDir)PSScripts\Start-GitShell.ps1" -destination $deployFolder -Force
Copy-Item "$($TargetDir)Powershell.Utility.dll" -destination $deployFolder -Force
Copy-Item "$($TargetDir)Gitsh.Extensions.dll" -destination $deployFolder -Force