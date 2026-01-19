# Script chạy Frontend Web
Write-Host "=== FRONTEND WEB STARTING ===" -ForegroundColor Green
Write-Host "Web will be available at:" -ForegroundColor Yellow
Write-Host "  - HTTPS: https://localhost:7228" -ForegroundColor Cyan
Write-Host "  - HTTP:  http://localhost:5198" -ForegroundColor Cyan
Write-Host "  - Forms: https://localhost:7228/Forms" -ForegroundColor Cyan
Write-Host ""
Start-Sleep -Seconds 3
cd D:\Documents\DynamicForm\DynamicForm.Web
dotnet run
