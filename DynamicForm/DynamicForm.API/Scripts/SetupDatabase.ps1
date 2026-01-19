# =============================================
# Script PowerShell để setup Database
# =============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Dynamic Form Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$createDbScript = Join-Path $scriptPath "CreateDatabase.sql"
$insertDataScript = Join-Path $scriptPath "InsertSampleData.sql"

$serverName = "(localdb)\mssqllocaldb"
$databaseName = "DynamicFormDb"
$utf8CodePage = 65001

Write-Host "Step 1: Creating database schema..." -ForegroundColor Yellow
try {
    # IMPORTANT: Force UTF-8 to avoid Vietnamese text corruption when running via sqlcmd
    $output = sqlcmd -S $serverName -f $utf8CodePage -b -i $createDbScript -o "$scriptPath\CreateDatabase_Output.txt" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database schema created successfully!" -ForegroundColor Green
    } else {
        Write-Host "✗ Error creating database schema" -ForegroundColor Red
        Write-Host $output
        exit 1
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Step 2: Inserting sample data..." -ForegroundColor Yellow
try {
    # IMPORTANT: Force UTF-8 to avoid Vietnamese text corruption when running via sqlcmd
    $output = sqlcmd -S $serverName -d $databaseName -f $utf8CodePage -b -i $insertDataScript -o "$scriptPath\InsertSampleData_Output.txt" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Sample data inserted successfully!" -ForegroundColor Green
    } else {
        Write-Host "✗ Error inserting sample data" -ForegroundColor Red
        Write-Host $output
        exit 1
    }
} catch {
    Write-Host "✗ Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database setup completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Database: $databaseName" -ForegroundColor White
Write-Host "Server: $serverName" -ForegroundColor White
Write-Host ""
Write-Host "You can now run the API:" -ForegroundColor Yellow
Write-Host "  cd DynamicForm.API" -ForegroundColor White
Write-Host "  dotnet run" -ForegroundColor White
Write-Host ""
