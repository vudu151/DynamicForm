# Script chạy Backend API
Write-Host "=== BACKEND API STARTING ===" -ForegroundColor Green
Write-Host "API will be available at:" -ForegroundColor Yellow
Write-Host "  - HTTPS: https://localhost:7220" -ForegroundColor Cyan
Write-Host "  - HTTP:  http://localhost:5144" -ForegroundColor Cyan
Write-Host "  - Swagger: https://localhost:7220/swagger" -ForegroundColor Cyan
Write-Host ""
cd D:\Documents\DynamicForm\DynamicForm.API
dotnet run
