param(
    [parameter(Position=0)]
    [string] $NameOrAlias,
    [switch] $log,
    [switch] $logOnly,
    [switch] $last,
    [switch] $list,
    [string] $Alias
)

# this exists because some of the things that need to be done aqpparently cannot be done from within the custom cmdlet


function Action_Last([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($last.IsPresent -and [string]::IsNullOrEmpty($result.TargetDirectory) -eq $false) {
        Set-Location $result.TargetDirectory    
    }
}


function RenderRepoList($listData) {
    write-text " "
    Write-text "Recent Repositories:" -colorgroup gcd-list-title
    write-text " "
    $listData | % {
        Write-text "   $($_.Ordinal)" -colorgroup gcd-list-number -ForceColorReset -NoNewLine
        Write-text "   $($_.Directory)" -colorgroup gcd-list-name -ForceColorReset -NoNewLine

        if ($_.HasAlias) {
            Write-text "   $($_.Alias)" -colorgroup gcd-list-alias -ForceColorReset -NoNewLine
        }

        if ($_.HasBranch) {
            $colorGroup = ",gcd-list-branch,"
            Write-text "   (|$($_.LastBranch)|)" $colorGroup -ForceColorReset -NoNewLine -TextSplit "|"
        }

        write-text " "
    }
    write-text " "
}


function Action_List([DWGitsh.Extensions.Models.GitChangeDirectoryInfo] $cmdOutput) {
    if ($cmdOutput.Options.List -eq $false -or  $cmdOutput.ListData -eq $null ) { return }

    RenderRepoList $cmdOutput.ListData


    if ($cmdOutput.PromptForListSelector -eq $true) {
        $done = $false
        $listItems = $cmdOutput.ListData

        while (-not $done) {
            Write-text "Enter a number, an alias or part of a path: " -colorgroup gcd-list-prompt -ForceColorReset -NoNewLine
            $userChoice = Read-host

            if ($userChoice -eq $null -or $userChoice -eq "" -or $userChoice -eq "q" -or $userChoice -eq "exit" -or $userChoice -eq "quit") { 
                $done = $true
            } else {
                $matchingItems = [DWGitsh.Extensions.Commands.Git.ChangeDirectory.GetGitChangeDirectoryCommand]::ResolveMatches($userChoice, $listItems)

                $matchCount = ($matchingItems | Measure-Object| Select Count).Count

                if ($matchCount -eq 1) {
                    $matchedItem = $matchingItems | Select -First 1
                    Set-Location $matchedItem.Directory 
                    $done = $true
				} else {
                    $listItems = $matchingItems
                    RenderRepoList $listItems
				}
            }
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
    if ([string]::IsNullOrEmpty($Alias) -eq $false) { $cmdArgs.Add("Alias", $Alias) }

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

