properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\Banzai.sln"
	$ProjectPath = "$BaseDir\src\Banzai.Ninject\Banzai.Ninject.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "Banzai.Ninject"
}

. .\common.ps1
