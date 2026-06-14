param(
  [string]$Project = "ProductWebApp\ProductWebApp.csproj",
  [int]$StartPort = 5262
)

$ErrorActionPreference = "Stop"

Set-StrictMode -Version Latest

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $root $Project

Write-Host "== HeatBalanceSystem: dev run ==" -ForegroundColor Cyan

Write-Host "Запуск без PostgreSQL (SQLite-файл по умолчанию)." -ForegroundColor Yellow
Write-Host "Если нужен PostgreSQL: в ProductWebApp/appsettings.json поставить Database:Provider=Postgres и применить миграции dotnet-ef." -ForegroundColor Yellow

Write-Host "Подбираю свободный порт..." -ForegroundColor Yellow
$port = $StartPort
while (Test-NetConnection -ComputerName 127.0.0.1 -Port $port -InformationLevel Quiet) {
  $port++
}

$urls = "http://127.0.0.1:$port"
Write-Host "Run web app on $urls" -ForegroundColor Yellow
dotnet run --project $projectPath --urls $urls

