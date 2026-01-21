# Script tự động extract và export Mermaid diagrams từ Markdown
# Usage: .\Export-MermaidDiagrams.ps1 -InputFile "docs\04-BUSINESS-ARCHITECTURE.md" -OutputDir "exports\diagrams"

param(
    [Parameter(Mandatory=$true)]
    [string]$InputFile,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputDir = "exports\diagrams",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("png", "svg", "pdf")]
    [string]$Format = "png"
)

# Tạo thư mục output nếu chưa có
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-Host "Created directory: $OutputDir" -ForegroundColor Green
}

# Đọc file markdown
$content = Get-Content $InputFile -Raw

# Pattern để tìm mermaid code blocks
$pattern = '```mermaid\s*\n(.*?)\n```'
$matches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)

Write-Host "Found $($matches.Count) Mermaid diagrams" -ForegroundColor Cyan

$diagramIndex = 1
foreach ($match in $matches) {
    $mermaidCode = $match.Groups[1].Value.Trim()
    
    # Tạo tên file dựa trên diagram type hoặc index
    $diagramType = "diagram"
    if ($mermaidCode -match 'graph\s+(TB|LR|RL|BT)') {
        $diagramType = "flowchart"
    } elseif ($mermaidCode -match 'sequenceDiagram') {
        $diagramType = "sequence"
    } elseif ($mermaidCode -match 'classDiagram') {
        $diagramType = "class"
    } elseif ($mermaidCode -match 'stateDiagram') {
        $diagramType = "state"
    }
    
    $outputFile = Join-Path $OutputDir "$diagramType-$diagramIndex.mmd"
    $mermaidCode | Out-File -FilePath $outputFile -Encoding UTF8
    
    Write-Host "  [$diagramIndex] Exported: $outputFile" -ForegroundColor Yellow
    
    # Tạo HTML file để preview
    $htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <title>Mermaid Diagram $diagramIndex</title>
    <script type="module">
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
        mermaid.initialize({ startOnLoad: true, theme: 'default' });
    </script>
</head>
<body>
    <div class="mermaid">
$mermaidCode
    </div>
</body>
</html>
"@
    
    $htmlFile = Join-Path $OutputDir "$diagramType-$diagramIndex.html"
    $htmlContent | Out-File -FilePath $htmlFile -Encoding UTF8
    
    Write-Host "       Preview: $htmlFile" -ForegroundColor Gray
    
    $diagramIndex++
}

Write-Host "`n[SUCCESS] Export completed!" -ForegroundColor Green
Write-Host "[INFO] Output directory: $OutputDir" -ForegroundColor Cyan
Write-Host "`n[INFO] Next steps:" -ForegroundColor Yellow
Write-Host "   1. Open HTML files in browser to preview" -ForegroundColor White
Write-Host "   2. Use Mermaid Live Editor (https://mermaid.live) to export PNG/SVG" -ForegroundColor White
Write-Host "   3. Or install mermaid-cli: npm install -g @mermaid-js/mermaid-cli" -ForegroundColor White
Write-Host "      Then run: mmdc -i $OutputDir\*.mmd -o $OutputDir\*.png" -ForegroundColor White
