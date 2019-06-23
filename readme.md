
Light-duty and highly customizable alternative to git bash since Windows users expect certain behaviors that git bash does not conform to, most notably copy-paste. 
Also, git bash performs horribly on systems that use a file monitoring package. 

Keep in mind that this only makes the command-line itself faster, not git, so visual tools, like GitExtensions, Github Desktop and SourceTree will still run very slowly on monitored systems.

![Default prompt][defaultPrompt]


## Features
- Faster basic response time than git bash
- Flexible prompt
  - Prompt becomes git bash like prompt when in a folder under a git repository
  - Shows ahead/behind on prompt
  - Shows staged/unstaged (mostly - see known issues)
  - Allows user-customized prompt
  - Switches to standard powershell prompt when not in a git repo folder
  - Highlights when git repo is still on `develop` or `master` branches
  - Highlights path when in a system folder
  - Highlights relative path from git repo root when deeper in the repo
  
- Colors can be customized 
- Color groups can be created and/or customized
- Git Prompt can be customized as desired
- cmdlets:
  - **Get-GitDetails** - returns detailed information about the current repository in a form that can be used in subsequent PowerShell commands
  - **Write-Text** - like Write-Host except that it can use the color groups defined for the shell, including the custom colors.  Custom color groups are also supported
  - **Get-GitLog** - returns the git log as an object that can be manipulated in powershell.  Supports `-After` and `-Before` date restrictions as well as the built-in options that git log offers.
  - **Get-GitConfig** - return an object that contains key information from the config as well as a list of all of the config values in a Dictionary<string, string>
  - **Get-GitshVersion** - returns the current version number.  This is also shown when starting the shell

##### Supports staging and ahead-behind
![sample with staging and ahead-behind][defaultPromptWithStaging]

# Setup
_(Yeah, I know this needs simplification)_
1. Clone the Repo
2. Build it in Visual Studio
3. Copy the contents of the `DWGitsh.Extensions/Deploy` folder to a more permanent location on your local drive
4. Start PowerShell and type 
    ```PowerShell
    if (!(Test-Path -Path $profile.CurrentUserAllHosts))
        { New-Item -Type File -Path $profile.CurrentUserAllHosts -Force }

    ise $profile.CurrentUserAllHosts
    ```
5. In ISE, enter:

   ```set-Alias gitsh C:\path\to\your\copy\of\Start-GitShell.ps1```

6. Save it
7. Close ISE
8. Close PowerShell
9. Start PowerShell
10. Type "gitsh" and you should see something like this : 

        DWGitshell Extensions - prompt will change when in a folder under git
        using DWGitsh.Extensions v2019.6.1.1505

    _if you do not see the above, it likely means that something is wrong with the Set-Alias command above_

11. Change directory to anywhere that is under a git repo and the prompt should change to something similar to Git Bash

# Usage
1. Start powershell
2. Type `gitsh` 
3. Change directory to a location under a git repo

That's it.  the prompt will change to the latest git status whenever you do anything in PowerShell.  

For reference, whenever the prompt needs to query github, it will display the current command in a black bar at the top of the powershell console window

# Customization
There are a few ways to customize this to the way you work.  Each way relies on a custom file to be created and placed in a specific location.  The reason for this is so that your customizations are not lost when you copy in in future updates.

### Custom Color Groups
The pre-defined colors are broken into groups and defined in the `defaultColors.csv`.  In order to make custom colors:

- Go to the directory where you copied the /Deploy files to in the installation
- Copy `defaultColors.csv` to a file named `customColors.csv`
- Edit that file and then change the colors to what you want

All colors use the [ConsoleColor](https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netframework-4.7.2) enum so those are the only valid values

### Custom Prompt
Should the default prompt not suit you, you can override the prompt by creating a custom powershell script that renders the prompt you do want.  To do this you will need: 

 - a file named "CustomGitPrompt.ps1"
 - the file placed in the same folder as "Start-Gitshell.ps1"
 - a function named "ShowGitPrompt that takes one parameter" _(while not technically needed, it is a good place to start)_

The custom script has access to all functions in "gitsh-LibUtils.ps1" and can be called from your code.

Additionally, if you create functions in the custom script that have the same name as a function in "gitsh-LibUtils.ps1" then the custom function will override the original allowing only specific functions to be overridden

There is a sample start file named "_CustomGitPrompt.ps1" that can be used as a starting point

## Related Git Tips
### Shortcuts / Aliases
This utility does not support the shortcuts that are normally provided by GitBash.  However, those can easily be added to your local git config 

```
git config --global alias.st status
git config --global alias.co checkout
```

afterwards, you can just run the command below to get the status:

``git st``

### Default Text Editor
If you are like me and use a custom text editor, this will come in handy. I use Notepad++ but this works for whichever one you use, just change the path and arguments accordingly
```
git config --global core.editor "'C:/Program Files/Notepad++/notepad++.exe' -multiInst -notabbar -nosession -noPlugin"
```
After that, any time git would normally launch its default editor or the Git Extensions editor, it will launch your custom editor

# NOTE:
The custom script is loaded and executed every time you press enter in powershell so avoid any heavy-duty code or long-running tasks


# Future enhancements
- a GCD command that tracks the directories used and allows quick switching based on part of the directory name or via switches like -Previous, basically pushing and popping directories off the stack
- (Other dreams here)


### Known Issues
- There are some caching quirks that I'm still working through regarding staging counts so those numbers might be off occasionally
- if an there is an error parsing the git status response, the set-prompt will not work, resulting in a prompt of "PS >".  I've only seen this twice so far.
- interactive debugging only works in .Net Framework 4.8


[defaultPrompt]: images/default_command_line.png
[defaultPromptWithStaging]: images/default_ahead_staging.png
