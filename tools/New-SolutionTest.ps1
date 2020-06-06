
$defaultLocation = Join-Path -Path $PSScriptRoot -ChildPath ".."
Set-Location $defaultLocation
Get-ChildItem -Path . -Recurse -Filter *.nupkg -ErrorAction SilentlyContinue | ForEach-Object { $_ | Remove-Item -Force -ErrorAction SilentlyContinue }

dotnet pack 'CleanArchitecture.Templates.csproj'

Set-Location $PSScriptRoot | Out-Null
$nupkg = Get-ChildItem -Path "../bin/Debug" -Filter CleanArchitecture.Templates.*.nupkg -ErrorAction Stop | Select-Object -First 1

dotnet new -i $nupkg.FullName

$path = Join-Path -Path "." -ChildPath "Test_$(Get-Date -Format "yyyyMMdd_HHmmss")"

New-Item -Path $path -ItemType Directory | Out-Null
Set-Location $path | Out-Null
$solutionName = 'MyApplication'

# Template Names: ca-sln, ca-sln-sql
dotnet new ca-sln-sql --name $solutionName --app-name $solutionName --secure-port 44399 --port 34399
dotnet restore "$solutionName/$solutionName.sln"
dotnet build "$solutionName/$solutionName.sln" -c Release --no-restore
$testProjects = Get-ChildItem -Path . -Recurse -Filter *Tests.csproj -ErrorAction SilentlyContinue
$testProjects | ForEach-Object { dotnet test "$($_.FullName)" --no-restore --no-build --configuration Release }
Set-Location ../.. | Out-Null