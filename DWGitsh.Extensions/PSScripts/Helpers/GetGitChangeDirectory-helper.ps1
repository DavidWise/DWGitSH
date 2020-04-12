param(
    [switch] $last,
    [switch] $list
)


function Action_Last([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($last.IsPresent -and [string]::IsNullOrEmpty($result.TargetDirectory) -eq $false) {
        Set-Location $result.TargetDirectory    
    }
}

function Action_List([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($list.IsPresent -and $cmdOutput.ListData -ne $null ) {
        $cmdOutput.ListData | % {
            Write-text "   $($_.Directory)" -colorgroup fullpath -ForceColorReset
            Write-text "   $($_.Directory)" -colorgroup fullpath -ForceColorReset
        }
    }
}

# this exists because some of the things that need to be done aqpparently cannot be done from within the custom cmdlet

$cmdArgs = @{}

if ($list.IsPresent) { $cmdArgs.Add("List", $true) }
if ($last.IsPresent) { $cmdArgs.Add("Last", $true) }

$result = Get-GitChangeDirectory @cmdArgs
if ($result -ne $null) {
    Action_Last $result
    Action_List $result
}

