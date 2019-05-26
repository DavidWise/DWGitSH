# Unit tests (requires PESTER - https://github.com/pester/Pester)

$scriptsFolder = split-path -parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

. $scriptsFolder\gitsh-LibUtils.ps1

function Write-Text {
    # dummy function
}
function Get-GitshVersion {
    # dummy function
}
function Get-GitDetails {
    # dummy function
}
function Get-Module {
    return @(@{ Name="gitsh.Extensions"}) 
}



Describe 'LibUtils-tests' {
    Mock Write-Host {}
    Mock Write-Output {}
    Mock Write-Text {}
    Mock Get-GitshVersion { return  @{Name="Library loaded from Test"; Text="1.2.3"} }
    Mock Import-Module {}

    BeforeEach {
        $script:sessionVars = @{}
        $script:sessionVars.Session = @{}
        $script:sessionVars.Paths = @{}
    }


    context 'InitializeGitsh' {
        Mock InitEnv {}
        it 'Init with NoColors is true' {
            InitializeGitsh $scriptsFolder $true

            $script:sessionVars.Paths.ScriptPath | Should -BeExactly $scriptsFolder
            $script:sessionVars.Session.NoColor | Should -BeTrue
            Assert-MockCalled InitEnv -Exactly 1 -ParameterFilter {$basePath -eq $scriptsFolder} -Scope It
        }

        it 'Init with NoColors is false' {
            InitializeGitsh $scriptsFolder $false

            $script:sessionVars.Paths.ScriptPath | Should -BeExactly $scriptsFolder
            $script:sessionVars.Session.NoColor | Should -BeFalse
            Assert-MockCalled InitEnv -Exactly 1 -ParameterFilter {$basePath -eq $scriptsFolder} -Scope It
        }
    }



    Context 'InitEnv' {
        Mock Get-Module { return @(@{ Name="gitsh.Extensions"}) }
        Mock Test-Path { return $false }


        function ValidateSessionVars() {
            $script:sessionVars.ModuleLoaded | Should -BeTrue
            $script:sessionVars.Initialized | Should -BeTrue
            $script:sessionVars.OriginalBackgroundColor | Should -BeExactly $([Console]::BackgroundColor)
            $env:Path.Split(';') | Should -Contain $scriptsFolder
        }


        it 'Basic Init loads module if needed' {
            Mock Get-Module { return @() }
            
            InitEnv $scriptsFolder
            Assert-MockCalled Get-Module -Exactly 1
            Assert-MockCalled Import-Module -Exactly 1 -scope it -ParameterFilter {$Name.EndsWith("Gitsh.Extensions.dll")}
            Assert-MockCalled Write-Host -Exactly 1 -ParameterFilter {"$object".IndexOf("Library loaded from Test") -gt 0}

            ValidateSessionVars

            $script:sessionVars.CustomGitPromptScript | Should -BeNullOrEmpty    
        }	

        it 'Basic Init skips module load if already loaded' {
            Mock Get-Module { return @(@{ Name="gitsh.Extensions"}) }
            
            InitEnv $scriptsFolder
            Assert-MockCalled Get-Module -Exactly 1 -scope it
            Assert-MockCalled Import-Module -Exactly 0 -scope it
            Assert-MockCalled Write-Host -Exactly 0 -Scope It

            ValidateSessionVars

            $script:sessionVars.CustomGitPromptScript | Should -BeNullOrEmpty
        }


        it 'Init flags custom prompt script' {
            Mock Test-Path { return $true }

            InitEnv $scriptsFolder

            ValidateSessionVars

            $script:sessionVars.CustomGitPromptScript | Should -Be "$scriptsFolder\CustomGitPrompt.ps1"
        }


        it 'All Initialization is skipped if already initialized' {
            $script:sessionVars.Initialized = $true

            InitEnv $scriptsFolder

            Assert-MockCalled Get-Module -Exactly 0 -scope it
            Assert-MockCalled Import-Module -Exactly 0 -scope it
            Assert-MockCalled Write-Host -Exactly 0 -Scope It
            Assert-MockCalled Test-Path -Exactly 0 -Scope It

        }
    }


    Context 'RenderOutput' {
        $testData = @{HasGit = $false}

        it 'Calls general prompt if no git' {
            $testData.HasGit = $false
            Mock ShowGitPrompt {}
            Mock ShowGeneralPrompt {}

            RenderOutput $testData

            Assert-MockCalled ShowGeneralPrompt -Exactly 1 -Scope It # -ParameterFilter {$data -eq $testData}
            Assert-MockCalled ShowGitPrompt -Exactly 0 -Scope It 
        }

        it 'Calls git prompt if dir has git' {
            $testData.HasGit = $true
            Mock ShowGitPrompt {}
            Mock ShowGeneralPrompt {}

            RenderOutput $testData

            Assert-MockCalled ShowGitPrompt -Exactly 1 -Scope It -ParameterFilter {$data -eq $testData}
            Assert-MockCalled ShowGeneralPrompt  -Exactly 0 -Scope It 
        }
    }
    

    Context 'ShowPrompt' {
        $testData = @{HasGit = $false}

        it 'showPrompt passes data correctly' {
            Mock Get-GitDetails { return $testData }
            Mock RenderOutput {}

            ShowPrompt

            Assert-MockCalled Get-GitDetails -Exactly 1 -Scope It
            Assert-MockCalled RenderOutput -Exactly 1 -Scope It -ParameterFilter {$data -eq $testData}

        }
    }
}