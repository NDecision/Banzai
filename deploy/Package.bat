@echo off

powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\archive.ps1; exit $error.Count}"


powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\package.ps1 -PackageName 'Banzai'; exit $error.Count}"


powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\package.ps1 -PackageName 'Banzai.Autofac'; exit $error.Count}"


powershell -NoProfile -ExecutionPolicy unrestricted -Command "& {.\package.ps1 -PackageName 'Banzai.Log4Net'; exit $error.Count}"


pause