
#expected usage:

# Function prompt {"$(c:\dev\bin\get-gitinfo.ps1)> "}

function FixTrailingSlash([string] $itemPath) {
    $result = $itemPath

    if (-not [string]::IsNullOrWhiteSpace($itemPath)) {
        $result = $itemPath.TrimEnd("\ ".ToCharArray())

        if ([string]::IsNullOrWhiteSpace($result)) {
            $result = $null
        } else {
            $result = "$result\"
        }
    }
    $result
}


function GetGitCachedItems([string] $gitDir) {
    $cachePath = [IO.Path]::Combine($env:APPDATA,"gitsh.cache")
    if (![IO.Directory]::Exists($cachePath)) {
        $f = New-Item $cachePath -ItemType Directory
    }


    $filename = $gitDir.Replace("\","_").Replace(":","").Replace(" ","_").Replace(".git","")
    $configPath = [IO.Path]::Combine($cachePath,"$($filename).config")

    $cfg = $null
    if([IO.File]::Exists($configPath) -eq $false) {
        $cfg = Invoke-Expression "git config --list"
        $result = [IO.File]::WriteAllLines($configPath, $cfg)
    } else {
        $cfg = [IO.File]::ReadAllLines($configPath)
    }

    $parsed = @{}
    $cfg | % {
        $line = "$_"
        $pos = $line.IndexOf("=")
        if ($pos -gt 0) {
            $key = $line.Substring(0,$pos)
            $value = $line.Substring($pos+1)

            $parsed[$key] = $value
        }
    }


    $user = New-Object psobject
    $user | Add-Member -MemberType NoteProperty -Name "Name" -Value $parsed["user.name"]
    $user | Add-Member -MemberType NoteProperty -Name "Email" -Value $parsed["user.email"]

    $obj = New-Object psobject
    $obj | Add-Member -MemberType NoteProperty -Name "User" -Value $user

    $obj
}


function GetPathToRepository() {
    $curDir = new-object System.IO.DirectoryInfo (Get-Location)
    $done = $false
    while(-not $done) {
        $path = [System.IO.Path]::Combine($curDir.FullName, ".git")

        if ([System.IO.Directory]::Exists($path)) { return $path }

        $curDir = ([System.IO.DirectoryInfo]$curDir).Parent

        $done = ($curDir -eq $null)
    }

    $null
}


function GetBranch([string]$gitDir) {
    $path = [System.IO.Path]::Combine($gitDir, "HEAD")
    $item = Get-Content $path -ErrorAction SilentlyContinue
    if ($item -ne $null) { 

        $pos = $item.LastIndexOf("/")
        if ($pos -gt 0) {
            $branch = $item.Substring($pos+1)
            return $branch
        }
    }

    return ""
}


function BuildResult() {
    $dir = GetPathToRepository

    $obj = New-Object psobject

    $obj | Add-Member -MemberType NoteProperty -Name "IsValid" -Value ($dir -ne $null)
    $obj | Add-Member -MemberType NoteProperty -Name "Path" -Value "$dir"
    $obj | Add-Member -MemberType NoteProperty -Name "Branch" -Value (GetBranch $dir)
    $obj | Add-Member -MemberType NoteProperty -Name "RelativePathToGitRoot" -Value ""
    $obj | Add-Member -MemberType NoteProperty -Name "Config" -Value $null

    if ($obj.IsValid) {
        $tempCurPath = FixTrailingSlash (Get-Location).Path
        $tempGitPath = FixTrailingSlash (new-object System.IO.DirectoryInfo $dir).Parent.FullName

        if ($tempCurPath -eq $tempGitPath) {
            
        } else {
            $obj.RelativePathToGitRoot = (Resolve-Path $tempGitPath -Relative)
        }
    }

    if ($obj.IsValid) {
        $obj.Config = GetGitCachedItems $obj.Path
    }

    $obj
}


function WritePair([string] $key, [string] $value, [ConsoleColor] $valueColor = [ConsoleColor]::Green) {
    if ([string]::IsNullOrWhiteSpace($key) -or [string]::IsNullOrWhiteSpace($value)) { return }

    write-host "$($key): " -ForegroundColor cyan -NoNewline
    write-host $value -ForegroundColor $valueColor -NoNewline
}


function RenderOutput($data) {
    $curDir = (Get-Location).Path
    $pathColor = [ConsoleColor]::White

    if ($data.IsValid) { 
        $pathColor = [ConsoleColor]::Yellow

        write-host 
        WritePair "branch" $data.Branch

        if ($data.Config -ne $null -and $data.Config.User -ne $null) {
            WritePair "   user" $data.Config.User.Email
        }

        WritePair "   root" $data.RelativePathToGitRoot

        write-host 
    }
    write-host "$curDir>" -ForegroundColor $pathColor -NoNewline
}


function ShowPrompt() {
    $result = BuildResult $dir

    RenderOutput $result
}

#ShowPrompt