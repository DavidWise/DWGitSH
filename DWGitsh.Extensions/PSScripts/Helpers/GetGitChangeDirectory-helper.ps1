param(
    [parameter(Position=0)]
    [string] $NameOrAlias,
    [switch] $log,
    [switch] $logOnly,
    [switch] $last,
    [switch] $list
)

# this exists because some of the things that need to be done aqpparently cannot be done from within the custom cmdlet


function Action_Last([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($last.IsPresent -and [string]::IsNullOrEmpty($result.TargetDirectory) -eq $false) {
        Set-Location $result.TargetDirectory    
    }
}

function Action_List([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($list.IsPresent -and $cmdOutput.ListData -ne $null ) {
        $cmdOutput.ListData | % {
            Write-text "   $($_.Ordinal)" -colorgroup gcd-list-number -ForceColorReset -NoNewLine
            Write-text "   $($_.Directory)" -colorgroup gcd-list-name -ForceColorReset -NoNewLine

            if ($_.HasAlias) {
                Write-text "   $($_.Alias)" -colorgroup gcd-list-name -ForceColorReset -NoNewLine
            }

            if ($_.HasBranch) {
                $colorGroup = ",gcd-list-branch,"
                Write-text "   (|$($_.LastBranch)|)" $colorGroup -ForceColorReset -NoNewLine -TextSplit "|"
            }

            write-text " "
        }
    }
}

function Action_NameOrAlias([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ([string]::IsNullOrEmpty($NameOrAlias)) { return }
    if ([string]::IsNullOrEmpty($result.TargetDirectory) -eq $false) {
        Set-Location $result.TargetDirectory
        return
    }

    # do more if needed
}

function BuildCommandArgs() {
    $cmdArgs = @{}

    if ([string]::IsNullOrEmpty($NameOrAlias) -eq $false) { $cmdArgs.Add("NameOrAlias", $NameOrAlias) }
    if ($log.IsPresent) { $cmdArgs.Add("Log", $true) }
    if ($logOnly.IsPresent) { $cmdArgs.Add("LogOnly", $true) }
    if ($list.IsPresent) { $cmdArgs.Add("List", $true) }
    if ($last.IsPresent) { $cmdArgs.Add("Last", $true) }

    $cmdArgs
}

function ProcessResult([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($cmdOutput -eq $null) { return }

    Action_Last $cmdOutput
    Action_List $cmdOutput
    Action_NameOrAlias $cmdOutput
}

$passedArgs = BuildCommandArgs

$result = Get-GitChangeDirectory @passedArgs

ProcessResult $result

