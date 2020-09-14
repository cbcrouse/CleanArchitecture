
$defaultLocation = Join-Path -Path $PSScriptRoot -ChildPath ".."
Push-Location $defaultLocation
Get-ChildItem -Path . -Recurse -Filter *.nupkg -ErrorAction SilentlyContinue | ForEach-Object { $_ | Remove-Item -Force -ErrorAction SilentlyContinue }

dotnet pack 'CleanArchitecture.Templates.csproj'

Set-Location $PSScriptRoot | Out-Null
$nupkg = Get-ChildItem -Path "../bin/Debug" -Filter CleanArchitecture.Templates.*.nupkg -ErrorAction Stop | Select-Object -First 1

# Clear out VS Template Files
$templateEngineDir = Join-Path -Path $env:USERPROFILE -ChildPath ".templateengine"

if (Test-Path -Path $templateEngineDir) {
    Get-ChildItem -Path $templateEngineDir -Recurse | ForEach-Object { Remove-Item -Path $_.FullName -Recurse -Verbose }
}

dotnet new -u "CleanArchitecture.Templates"
dotnet new -i $nupkg.FullName

$path = Join-Path -Path "." -ChildPath "Test_$(Get-Date -Format "yyyyMMdd_HHmmss")"

New-Item -Path $path -ItemType Directory | Out-Null
Set-Location $path | Out-Null
$solutionName = 'MyApplication'

# Template Names: ca-sln, ca-sln-sql
dotnet new ca-sln-sql --name $solutionName
dotnet restore "$solutionName/$solutionName.sln"
dotnet build "$solutionName/$solutionName.sln" -c Release --no-restore
$testProjects = Get-ChildItem -Path . -Recurse -Filter *Tests.csproj -ErrorAction SilentlyContinue
$testProjects | ForEach-Object { dotnet test "$($_.FullName)" --no-restore --no-build --configuration Release }
Pop-Location