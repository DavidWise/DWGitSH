<#
Sample custom git prompt.  To use this, you must have : 
 - a file named "CustomGitPrompt.ps1"
 - the file in the same folder as "Start-Gitshell.ps1"
 - a function named "ShowGitPrompt that takes one parameter"

	the custom script has access to all functions in "gitsh-LibUtils.ps1"

	also, if you create functions in the custom script that have the same name as a function in "gitsh-LibUtils.ps1" then the custom function will override
	allowing only specific functions to be overriden
#>

function ShowGitPrompt($data) {
	write-text

	ShowGitFragment_UserName $data

	ShowGitFragment_Path $data

	ShowGitFragment_Branch $data

	ShowGitFragment_AheadBehind $data

	ShowGitFragment_Staging $data

    write-text 

	ShowGitFragment_Prompt $data
}