# PowerShell Script để generate PlantUML URL
# Sử dụng: .\generate-url.ps1 [filename.puml]

param(
    [Parameter(Mandatory=$false)]
    [string]$FileName = ""
)

$PlantUMLBaseUrl = "http://www.plantuml.com/plantuml/uml/"

function Encode-PlantUML {
    param([string]$Code)
    
    # PlantUML sử dụng deflate compression + base64 encoding
    # PowerShell đơn giản: sử dụng encodeURIComponent tương đương
    # Thực tế PlantUML web server tự động decode nếu paste trực tiếp
    
    # Trả về code để user copy và paste vào web
    return $Code
}

function Open-PlantUMLWeb {
    param([string]$Code)
    
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "PlantUML URL Generator" -ForegroundColor Yellow
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    if ([string]::IsNullOrWhiteSpace($Code)) {
        Write-Host "❌ Không có code PlantUML!" -ForegroundColor Red
        return
    }
    
    # Mở trình duyệt
    Start-Process $PlantUMLBaseUrl
    
    Write-Host "✅ Đã mở PlantUML Web Server trong trình duyệt" -ForegroundColor Green
    Write-Host "`n📋 Hướng dẫn:" -ForegroundColor Yellow
    Write-Host "1. Copy code PlantUML bên dưới" -ForegroundColor White
    Write-Host "2. Paste vào ô text trên trang PlantUML" -ForegroundColor White
    Write-Host "3. Click 'Submit' để xem sơ đồ`n" -ForegroundColor White
    
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "CODE PLANTUML:" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host $Code -ForegroundColor White
    Write-Host "`n========================================`n" -ForegroundColor Cyan
    
    # Copy vào clipboard
    $Code | Set-Clipboard
    Write-Host "✅ Đã copy code vào clipboard!" -ForegroundColor Green
    Write-Host "   Bạn có thể paste trực tiếp vào PlantUML Web Server`n" -ForegroundColor Gray
}

# Main script
if ([string]::IsNullOrWhiteSpace($FileName)) {
    # Hiển thị menu
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "PlantUML Diagram Generator" -ForegroundColor Yellow
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    Write-Host "Chọn diagram để mở:" -ForegroundColor Yellow
    Write-Host "1. 01-Tao-Thiet-Ke-Form.puml" -ForegroundColor White
    Write-Host "2. 02-Dien-Luu-Du-Lieu.puml" -ForegroundColor White
    Write-Host "3. 03-Xem-Sua-Du-Lieu.puml" -ForegroundColor White
    Write-Host "4. 04-Tao-Version-Moi.puml" -ForegroundColor White
    Write-Host "5. 05-Validate-Du-Lieu.puml" -ForegroundColor White
    Write-Host "6. 06-Tong-Hop.puml" -ForegroundColor White
    Write-Host "0. Thoát`n" -ForegroundColor White
    
    $choice = Read-Host "Nhập số (1-6) hoặc đường dẫn file .puml"
    
    $files = @{
        "1" = "01-Tao-Thiet-Ke-Form.puml"
        "2" = "02-Dien-Luu-Du-Lieu.puml"
        "3" = "03-Xem-Sua-Du-Lieu.puml"
        "4" = "04-Tao-Version-Moi.puml"
        "5" = "05-Validate-Du-Lieu.puml"
        "6" = "06-Tong-Hop.puml"
    }
    
    if ($files.ContainsKey($choice)) {
        $FileName = $files[$choice]
    } elseif ($choice -eq "0") {
        exit
    } else {
        $FileName = $choice
    }
}

# Đọc file
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$filePath = Join-Path $scriptPath $FileName

if (-not (Test-Path $filePath)) {
    Write-Host "`n❌ Không tìm thấy file: $filePath" -ForegroundColor Red
    Write-Host "Vui lòng kiểm tra lại đường dẫn file.`n" -ForegroundColor Yellow
    exit 1
}

try {
    $code = Get-Content $filePath -Raw -Encoding UTF8
    Open-PlantUMLWeb -Code $code
} catch {
    Write-Host "`n❌ Lỗi khi đọc file: $_" -ForegroundColor Red
    exit 1
}
