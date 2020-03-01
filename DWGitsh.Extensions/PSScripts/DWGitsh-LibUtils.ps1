
$script:sessionVars = @{}
$script:sessionVars.Session = @{}
$script:sessionVars.Paths = @{}


function InitEnv([string] $basePath) {

    if ($script:sessionVars.Initialized -eq $true) { return }
    AddDWGitshAliases

    if ($script:sessionVars.ModuleLoaded -ne $true) {
        $needsModule = (get-module | where {$_.Name -eq "DWGitsh.Extensions" }) -eq $null
        $libPath = [IO.Path]::Combine($basePath, "DWGitsh.Extensions.dll")

        if ($needsModule -eq $true) {
            Import-Module $libPath -force -ErrorAction SilentlyContinue
            $verInfo = Get-GitshVersion

            Write-Host "Using $($verInfo.Name) v$($verInfo.text)" -ForegroundColor Green
        }
        $script:sessionVars.ModuleLoaded = $true
    }

    $pathIdx = $env:Path.IndexOf($basePath, [stringcomparison]::InvariantCultureIgnoreCase)
    if ($pathIdx -lt 0) {
         $env:Path = "$($env:Path);$basePath"
    }

	$script:sessionVars.OriginalBackgroundColor = [Console]::BackgroundColor;

	$script:sessionVars.CustomGitPromptScript = $null
	$custPath = [IO.Path]::Combine($basePath, "CustomGitPrompt.ps1")
	if (Test-Path $custPath) { 
		$script:sessionVars.CustomGitPromptScript = $custPath 
	}

    $script:sessionVars.Initialized = $true
}


function AddDWGitshAliases() {
    
    if ((get-alias gcd) -eq $null) {
        set-alias gcd Get-GitChangeDirectory -scope Global
    }
}


function InitializeGitsh([string] $basePath, [bool] $NoColor) {
    $script:sessionVars.Paths.ScriptPath = $basePath
    $script:sessionVars.Session.NoColor = $NoColor

    InitEnv $basePath
}


function ShowGeneralPrompt() {
    $curDir = (Get-Location).Path
    $newPrompt = "PS $curDir>"

    $isSysFolder = $curDir.StartsWith($env:SystemRoot, [System.StringComparison]::InvariantCultureIgnoreCase)

    if ($isSysFolder) {
        write-text $newPrompt -ColorGroup sysFolder -NoNewline -ForceColorReset
    } else {
        write-text $newPrompt -colorgroup default -NoNewline -ForceColorReset
    }
}

function ShowGitFragment_UserName($data) {
    if ($data.User -ne $null) {
        write-text "$($data.User.Mailbox) " -NoNewline -colorgroup userId
    }
}

function ShowGitFragment_Branch($data) {
   if ([string]::IsNullOrWhiteSpace($data.Branch) -eq $false) {
		$colorGroup = ",branch,,"
	    $branchName = $data.Branch
		if ($data.Branch -eq "develop" -or $data.Branch -eq "master") { $colorGroup = "branchAlert,branchAlert,branchAlert," }
		Write-Text "( |$branchName| )|" $colorGroup -NoNewLine -TextSplit "|" -ForceColorReset
		write-text " " -NoNewLine
    }
}

function ShowGitFragment_Detached($data) {
   if ($data.IsDetached -eq $true) {
		$colorGroup = "detachedAt,detachedAt,detachedAt,"
		Write-Text "Detached at |$($data.DetachedAt)||" $colorGroup -NoNewLine -TextSplit "|" -ForceColorReset
		write-text " " -NoNewLine
    }
}

function ShowGitFragment_Path($data) {
    $curDir = (Get-Location).Path

	# Current path with Repo Highlight
    $rootLen = $data.root.length
    if ($curDir.Length -lt $rootLen) { $rootLen = $curDir.Length }
    write-text  "$($curDir.Substring(0, $rootLen))" -ColorGroup basePath -NoNewline
    if ($curDir.Length -gt $rootLen) {
        write-text "$($curDir.Substring($data.root.length))" -colorGroup fullPath -NoNewline
    }
	write-text " " -NoNewLine
}

function ShowGitFragment_AheadBehind($data) {
    if ($data.Ahead -gt 0) {
		Write-Text "|Ahead: |$($data.Ahead) " ",abLabel,abValue"  -TextSplit "|" -NoNewLine
    }

    if ($data.Behind -gt 0) {
		Write-Text "|Behind: |$($data.Behind) " ",abLabel,abValue" -TextSplit "|" -NoNewLine
    }
}

function ShowGitFragment_Staging($data) {
	if ($data.Staged -gt 0 -or $data.Unstaged -gt 0) {
		Write-Text "|Stage: |$($data.Unstaged)|/|$($data.Staged) " ",stageLabel,stageDown,default,stageUp" -TextSplit "|" -NoNewLine
    }
}

function ShowGitFragment_Prompt($data) {
	write-text "$" -colorGroup promptToken -NoNewline -ForceColorReset
}

function ShowGitPrompt($data) {

    write-text

	ShowGitFragment_Path $data

	ShowGitFragment_UserName $data

	ShowGitFragment_Branch $data

	ShowGitFragment_Detached $data

	ShowGitFragment_AheadBehind $data

	ShowGitFragment_Staging $data

    write-text 

    ShowGitFragment_Prompt $data
}


function RenderOutput($data) {
    if ($data.HasGit) { 
        ShowGitPrompt $data
    } else {
        ShowGeneralPrompt
    }
}


function ShowPrompt() {
    $result = Get-GitDetails

    RenderOutput $result
}

#InitializeGitsh $PSScriptRoot $false
#ShowPrompt