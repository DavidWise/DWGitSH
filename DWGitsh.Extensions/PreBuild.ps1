param(
    [string] $ProjectDir
)

# Meant to be called from the Pre-Build Event
# powershell.exe -NoLogo -NoProfile -File $(SolutionDir)PreBuild.ps1 -ProjecDir $(SolutionDir)

$propPath = "$ProjectDir\Properties\AssemblyInfo.cs"

$data = Get-Content -Path $propPath -ErrorAction SilentlyContinue

if ($data -eq $null) { return }

$version = [string]::Format("{0:yyyy}.{0:MM}.{0:dd}.{0:HHmm}", [DateTime]::Now)

$lines = $data.Split("`r`n".ToCharArray())
$changed = $false

$result = $lines | % { 
    $line = "$_"
    if ($line.StartsWith("[assembly: AssemblyFileVersion(")) {
        "[assembly: AssemblyFileVersion(""$version"")]"
        $changed = $true
    } elseif ($line.StartsWith("[assembly: AssemblyVersion(")) {
        "[assembly: AssemblyVersion(""$version"")]"
        $changed = $true

    } else {
        $_
    }
}

if ($changed -eq $true) {
    $outData  = [string]::Join("`r`n", $result)

    Set-Content $propPath -Value $outData

    Write-Output "* Assembly Version set to: $version"

}