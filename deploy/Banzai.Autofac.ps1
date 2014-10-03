properties {
	$BaseDir = Resolve-Path "..\"
	$SolutionFile = "$BaseDir\src\Banzai.sln"
	$ProjectPath = "$BaseDir\src\Banzai.Autofac\Banzai.Autofac.csproj"	
	$ArchiveDir = "$BaseDir\Deploy\Archive"
	
	$NuGetPackageName = "Banzai.Autofac"
}

. .\common.ps1
