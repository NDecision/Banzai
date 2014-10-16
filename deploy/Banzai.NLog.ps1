properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\Banzai.sln"
	$ProjectPath = "$BaseDir\src\Banzai.NLog\Banzai.NLog.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "Banzai.NLog"
}

. .\common.ps1
