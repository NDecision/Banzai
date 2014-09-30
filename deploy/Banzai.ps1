properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\Banzai.sln"
	$ProjectPath = "$BaseDir\src\Banzai\Banzai.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "Banzai"
}

. .\common.ps1
