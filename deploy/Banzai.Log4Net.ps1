properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\Banzai.sln"
	$ProjectPath = "$BaseDir\src\Banzai.Log4Net\Banzai.Log4Net.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "Banzai.Log4Net"
}

. .\common.ps1
