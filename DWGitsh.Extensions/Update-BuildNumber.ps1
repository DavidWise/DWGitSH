param(
    [string] $ProjectDir = $PSScriptRoot
)

Write-Output "ZZZ: $ProjectDir"
set-location $ProjectDir
# Meant to be called from the Pre-Build Event
# powershell.exe -NoLogo -NoProfile -File $(SolutionDir)PreBuild.ps1 -ProjecDir $(SolutionDir)

$propPath = "Properties\AssemblyInfo.cs"


function BuildVersion() {
    $timeNow = [DateTime]::Now

    $verText = [string]::Format("{0:yyyy}.{0:MM}.{0:dd}.{0:HHmm}", $timeNow)
    $verParts = $verText.Split('.')

    $obj = New-Object psobject
    $obj | Add-Member -MemberType NoteProperty -Name "Text" -Value $verText
    $obj | Add-Member -MemberType NoteProperty -Name "Major" -Value $verParts[0]
    $obj | Add-Member -MemberType NoteProperty -Name "Minor" -Value $verParts[1]
    $obj | Add-Member -MemberType NoteProperty -Name "Revision" -Value $verParts[2]
    $obj | Add-Member -MemberType NoteProperty -Name "Build" -Value $verParts[3]

    $obj

}


function AddVersionToAssemblyInfo($version) {

	$data = Get-Content -Path $propPath -ErrorAction SilentlyContinue

	if ($data -eq $null) { return }

	$lines = $data.Split("`r`n".ToCharArray())
	$changed = $false
    $versionText = $version.Text

	$result = $lines | % { 
		$line = "$_"
		if ($line.StartsWith("[assembly: AssemblyFileVersion(")) {
			"[assembly: AssemblyFileVersion(""$versionText"")]"
			$changed = $true
		} elseif ($line.StartsWith("[assembly: AssemblyVersion(")) {
			"[assembly: AssemblyVersion(""$versionText"")]"
			$changed = $true

		} else {
			$_
		}
	}

	if ($changed -eq $true) {
		$outData  = [string]::Join("`r`n", $result)

		Set-Content $propPath -Value $outData

		Write-Output "* Assembly Version set to: $versionText"

	}
}

function GetNode([System.Xml.XmlDocument] $xmlDocument, [System.Xml.XmlElement] $parentNode, [string] $nodeName) {
    $node = $parentNode.SelectSingleNode($nodeName)
    if ($node -eq $null) {
        $node = $xmlDocument.CreateElement($nodeName)
        $parentNode.AppendChild($versionNode)
        $node = $parentNode.SelectSingleNode($nodeName)
    }
    $node
}


function AddVersionToProjectFile($version) {
    $projectFiles = [System.IO.Directory]::GetFiles($ProjectDir, "*.*proj", [System.IO.SearchOption]::TopDirectoryOnly)
    if ($projectFiles -eq $null -or $projectFiles.length -lt 1) {
        throw [System.Exception]  "No project files found to modify"
    }

    if ($projectFiles.length -gt 1) {
        throw [System.Exception]  "Too many project files found to modify"
    }


    $projFile =$projectFiles[0]

    $doc = [xml](Get-Content $projFile)

    #using xml
    $groupNode = $doc.SelectSingleNode("/Project/PropertyGroup[TargetFrameworks|TargetFramework]")
    if ($groupNode -eq $null) { 
        throw [System.Exception] "Unable to identify proper parent node for version in Project file '$projFile'"
    }

    $versionNode = GetNode $doc $groupNode "Version"
    $versionNode.InnerText = $version.Text

    $versionNode = GetNode $doc $groupNode "AssemblyVersion"
    $versionNode.InnerText = $version.Text

    $versionNode = GetNode $doc $groupNode "FileVersion"
    $versionNode.InnerText = $version.Text

	Write-Output "ZZZ: $($version.Text)"

    $doc.Save($projFile)
}

$versionInfo = BuildVersion


if ([System.IO.File]::Exists($propPath)) { 
    AddVersionToAssemblyInfo $versionInfo
} else {
    AddVersionToProjectFile $versionInfo
}


