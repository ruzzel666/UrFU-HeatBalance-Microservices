param(
  [int]$WebPort = 5262
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

Write-Host "== HeatBalanceSystem: microservices ==" -ForegroundColor Cyan

$services = @(
  @{ Name = "Auth";           Project = "src\Auth\HeatBalance.Auth\HeatBalance.Auth.csproj";                     Port = 5001 },
  @{ Name = "Gateway";        Project = "src\Gateway\HeatBalance.Gateway\HeatBalance.Gateway.csproj";             Port = 5000 },
  @{ Name = "Dataset";        Project = "src\Services\HeatBalance.DatasetService\HeatBalance.DatasetService.csproj"; Port = 5100 },
  @{ Name = "ConveyorKiln";   Project = "src\Services\HeatBalance.ConveyorKilnService\HeatBalance.ConveyorKilnService.csproj"; Port = 5101 },
  @{ Name = "ChamberKiln";    Project = "src\Services\HeatBalance.ChamberKilnService\HeatBalance.ChamberKilnService.csproj"; Port = 5102 },
  @{ Name = "ElectricKiln";   Project = "src\Services\HeatBalance.ElectricKilnService\HeatBalance.ElectricKilnService.csproj"; Port = 5103 },
  @{ Name = "DrumDryer";      Project = "src\Services\HeatBalance.DrumDryerService\HeatBalance.DrumDryerService.csproj"; Port = 5104 },
  @{ Name = "Run";            Project = "src\Services\HeatBalance.RunService\HeatBalance.RunService.csproj";       Port = 5200 },
  @{ Name = "Report";         Project = "src\Services\HeatBalance.ReportService\HeatBalance.ReportService.csproj"; Port = 5300 },
  @{ Name = "Web BFF";        Project = "ProductWebApp\ProductWebApp.csproj";                                     Port = $WebPort }
)

Write-Host "Сборка решения..." -ForegroundColor Yellow
dotnet build HeatBalanceSystem.sln

$jobs = @()
foreach ($svc in $services) {
  $projectPath = Join-Path $root $svc.Project
  $urls = "http://127.0.0.1:$($svc.Port)"
  Write-Host "Запуск $($svc.Name) на $urls" -ForegroundColor Green

  $envBlock = @{}
  if ($svc.Name -eq "Web BFF") {
    $envBlock["ASPNETCORE_ENVIRONMENT"] = "Microservices"
  }

  $job = Start-Job -ScriptBlock {
    param($ProjectPath, $Urls, $EnvironmentName)
    if ($EnvironmentName) { $env:ASPNETCORE_ENVIRONMENT = $EnvironmentName }
    dotnet run --project $ProjectPath --urls $Urls
  } -ArgumentList $projectPath, $urls, $(if ($svc.Name -eq "Web BFF") { "Microservices" } else { $null })

  $jobs += $job
  Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "Все сервисы запущены." -ForegroundColor Cyan
Write-Host "Web UI:      http://127.0.0.1:$WebPort" -ForegroundColor White
Write-Host "Gateway:     http://127.0.0.1:5000" -ForegroundColor White
Write-Host "Auth/OAuth:  http://127.0.0.1:5001" -ForegroundColor White
Write-Host "Admin:       admin@example.com / Admin123$" -ForegroundColor White
Write-Host ""
Write-Host "Нажмите Ctrl+C для остановки всех сервисов." -ForegroundColor Yellow

try {
  while ($true) { Start-Sleep -Seconds 5 }
}
finally {
  Write-Host "Остановка сервисов..." -ForegroundColor Yellow
  $jobs | Stop-Job -ErrorAction SilentlyContinue
  $jobs | Remove-Job -Force -ErrorAction SilentlyContinue
}
