
param(
    [string] $ProjectDir = "D:\Dev\Github\DWGitsh\DWGitsh.Extensions\",
    [string] $TargetDir = "D:\Dev\Github\DWGitsh\DWGitsh.Extensions\bin\debug\net48"
)

# Meant to be called from the Post-Build Event
#  powershell.exe -NoLogo -NoProfile -File $(SolutionDir)PostBuild.ps1 -ProjectDir $(SolutionDir) -TargetDir $(TargetDir)

$firstMessageWritten = $false

function WriteInitialMessage([string] $deploy) {
    if ($firstMessageWritten -eq $false) {
        write-output "Copying files to $deploy"
    }
    $script:firstMessageWritten = $true
}

function EnsureTrailingSlash([string] $path) {
    if ($path.EndsWith("\")) { return $path }

    $trimmedPath = $path.TrimEnd("\ ".ToCharArray())
    return "$trimmedPath\";
}

function CopyFileIfDifferent([string] $source, [string] $dest) {
	$sourceInfo = Get-ChildItem $source
    $destName = "$($dest)\$($sourceInfo.Name)"
	$destInfo = Get-ChildItem $destName -ErrorAction SilentlyContinue

    if ($destInfo -eq $null -or $destInfo.Exists -eq $false -or $sourceInfo.LastWriteTime -ne $destInfo.LastWriteTime) {
        WriteInitialMessage $dest
        Write-Output "   - '$source'"
    	Copy-Item $source -destination $dest -Force
    }
}

function DetermineTargetFolder([string] $folder) {
    $data = Get-Item $folder

    return $data.Name
}

$target = DetermineTargetFolder $TargetDir
$projDir = EnsureTrailingSlash $ProjectDir

$deployFolder = "$($projDir)Deploy\$target\"

if ([System.IO.Directory]::Exists($deployFolder) -eq $false) {
    [System.IO.Directory]::CreateDirectory($deployFolder)
}

$dlls = [System.IO.Directory]::GetFiles($TargetDir, "*.dll")


CopyFileIfDifferent "$($projDir)PSScripts\defaultColors.csv" $deployFolder 
CopyFileIfDifferent "$($projDir)PSScripts\_CustomGitPrompt.ps1" $deployFolder 
CopyFileIfDifferent "$($projDir)PSScripts\gitsh-LibUtils.ps1" $deployFolder
CopyFileIfDifferent "$($projDir)PSScripts\Start-GitShell.ps1" $deployFolder

$dlls | % {
    CopyFileIfDifferent $_ $deployFolder
}
