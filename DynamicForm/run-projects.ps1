# =============================================
# Script chạy cả 2 project (API và Web)
# =============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Dynamic Form Projects" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$apiPath = Join-Path $PSScriptRoot "DynamicForm.API"
$webPath = Join-Path $PSScriptRoot "DynamicForm.Web"

# Kiểm tra project paths
if (-not (Test-Path $apiPath)) {
    Write-Host "Error: API project not found at $apiPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $webPath)) {
    Write-Host "Error: Web project not found at $webPath" -ForegroundColor Red
    exit 1
}

Write-Host "Starting Backend API..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiPath'; Write-Host 'Backend API starting...' -ForegroundColor Green; dotnet run" -WindowStyle Normal

Write-Host "Waiting 3 seconds before starting Web..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

Write-Host "Starting Frontend Web..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$webPath'; Write-Host 'Frontend Web starting...' -ForegroundColor Green; dotnet run" -WindowStyle Normal

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Both projects are starting!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backend API:" -ForegroundColor White
Write-Host "  - HTTPS: https://localhost:7220" -ForegroundColor Gray
Write-Host "  - HTTP:  http://localhost:5144" -ForegroundColor Gray
Write-Host "  - Swagger: https://localhost:7220/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host "Frontend Web:" -ForegroundColor White
Write-Host "  - HTTPS: https://localhost:7228" -ForegroundColor Gray
Write-Host "  - HTTP:  http://localhost:5198" -ForegroundColor Gray
Write-Host "  - Forms: https://localhost:7228/Forms" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
