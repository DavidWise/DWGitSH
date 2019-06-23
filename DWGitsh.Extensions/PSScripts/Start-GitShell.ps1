param(
    [switch] $NoColor
)
#  attempted to make it look like the Bash window... doesnt work well in powershell as other default colors for 
#  commands are not designed around a black backrground and are nearly unreadable

$Global:gitshData = @{
    ScriptRoot = $PSScriptRoot;
    NoColor = $NoColor.IsPresent;
    UtilPath = [IO.Path]::Combine($PSScriptRoot, "DWGitsh-LibUtils.ps1")
}

write-host "DWGitshell Extensions - prompt will change when in a folder under git" -ForegroundColor Cyan

Function Global:prompt {
    . $Global:gitshData.UtilPath

    InitializeGitsh $Global:gitshData.ScriptRoot $Global:gitshData.NoColor

	if ($script:sessionVars.CustomGitPromptScript -ne $null) {
		. $script:sessionVars.CustomGitPromptScript
	}

    ShowPrompt
    " " 
}

