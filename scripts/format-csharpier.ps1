$ErrorActionPreference = "Stop"

$repoRoot = git rev-parse --show-toplevel
Set-Location $repoRoot

dotnet tool restore | Out-Host
dotnet csharpier format . | Out-Host
