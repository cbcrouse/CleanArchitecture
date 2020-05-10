
Set-Location (Join-Path -Path $($PSScriptRoot) -ChildPath "..") | Out-Null
Get-ChildItem -Path . -Recurse -Filter *.nupkg -ErrorAction SilentlyContinue | ForEach-Object { $_ | Remove-Item -Force -ErrorAction SilentlyContinue }

dotnet pack

Set-Location $($PSScriptRoot) | Out-Null
$nupkg = Get-ChildItem -Path "../bin/Debug" -Filter CleanArchitecture.Templates.*.nupkg -ErrorAction Stop | Select-Object -First 1

dotnet new -i $nupkg.FullName

$path = Join-Path -Path "." -ChildPath "Test_$(Get-Date -Format "yyyyMMdd_HHmmss")"

New-Item -Path $path -ItemType Directory | Out-Null
Set-Location $path | Out-Null
$solutionName = 'MyApplication'

dotnet new ca-sln --name $($solutionName) --app-name $($solutionName) --secure-port 44399 --port 30356

Set-Location $($solutionName) | Out-Null

dotnet restore "$($solutionName).sln"
dotnet build "$($solutionName).sln"
dotnet test "$($solutionName).sln"

Set-Location .. | Out-Null
Set-Location .. | Out-Null