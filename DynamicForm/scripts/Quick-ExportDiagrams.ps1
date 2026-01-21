# Quick script để export diagrams và mở preview
# Usage: .\Quick-ExportDiagrams.ps1

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$docsFile = Join-Path $projectRoot "docs\04-BUSINESS-ARCHITECTURE.md"
$outputDir = Join-Path $projectRoot "exports\diagrams"

Write-Host "`n=== Mermaid Diagrams Exporter ===" -ForegroundColor Cyan
Write-Host "Input: $docsFile" -ForegroundColor Gray
Write-Host "Output: $outputDir`n" -ForegroundColor Gray

# Chạy export script
& "$scriptDir\Export-MermaidDiagrams.ps1" -InputFile $docsFile -OutputDir $outputDir

# Mở thư mục output
if (Test-Path $outputDir) {
    Write-Host "`n[ACTION] Opening output directory..." -ForegroundColor Yellow
    Start-Process explorer.exe -ArgumentList $outputDir
}

Write-Host "`n[INFO] To export PNG/SVG:" -ForegroundColor Cyan
Write-Host "   1. Open any .html file in browser to preview" -ForegroundColor White
Write-Host "   2. Copy code from .mmd file" -ForegroundColor White
Write-Host "   3. Paste into https://mermaid.live and export PNG/SVG" -ForegroundColor White
Write-Host "`n[INFO] Or use Mermaid CLI (if installed):" -ForegroundColor Cyan
Write-Host "   cd $outputDir" -ForegroundColor White
Write-Host "   mmdc -i *.mmd -o *.png" -ForegroundColor White
