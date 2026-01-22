# PlantUML Diagrams - DynamicForm

Thư mục này chứa các file PlantUML (.puml) và script để generate URL cho PlantUML Web Server.

## 📁 Cấu trúc thư mục

```
diagrams/
├── 00-Database-ERD.puml               # Database ERD PlantUML (Chi tiết đầy đủ)
├── 00-Database-ERD-Simple.puml        # Database ERD PlantUML (Đơn giản)
├── 00-Database-ERD.dbml               # Database ERD DBML (Cho dbdiagram.io)
├── 00-Database-ERD-Simple.dbml        # Database ERD DBML (Đơn giản)
├── 01-Tao-Thiet-Ke-Form.puml          # Quy trình tạo và thiết kế form
├── 02-Dien-Luu-Du-Lieu.puml           # Quy trình điền và lưu dữ liệu
├── 03-Xem-Sua-Du-Lieu.puml            # Quy trình xem và sửa dữ liệu
├── 04-Tao-Version-Moi.puml            # Quy trình tạo version mới
├── 05-Validate-Du-Lieu.puml           # Quy trình validate dữ liệu
├── 06-Tong-Hop.puml                   # Quy trình tổng hợp
├── generate-plantuml-url.html         # Web tool để generate URL
├── generate-url.ps1                   # PowerShell script
├── HUONG-DAN-ERD.md                   # Hướng dẫn vẽ ERD với PlantUML
├── HUONG-DAN-DB-DIAGRAM.md            # Hướng dẫn paste vào dbdiagram.io
└── README.md                          # File này
```

## 🚀 Cách sử dụng

### Cách 1: Sử dụng Web Tool (Khuyến nghị)

1. Mở file `generate-plantuml-url.html` trong trình duyệt
2. Click vào nút "Load vào Editor" để load diagram
3. Click "Mở PlantUML Web" để mở PlantUML Web Server
4. Copy code từ editor và paste vào PlantUML Web Server
5. Click "Submit" để xem sơ đồ

### Cách 2: Sử dụng PowerShell Script

```powershell
# Chạy script với menu
.\generate-url.ps1

# Hoặc chỉ định file cụ thể
.\generate-url.ps1 01-Tao-Thiet-Ke-Form.puml
```

### Cách 3: Copy trực tiếp

1. Mở file `.puml` bằng text editor
2. Copy toàn bộ nội dung
3. Truy cập http://www.plantuml.com/plantuml/uml/
4. Paste code vào ô text
5. Click "Submit" để xem sơ đồ

## 📋 Danh sách Diagrams

### Database ERD Diagrams

| File | Format | Mô tả |
|------|--------|------|
| `00-Database-ERD.puml` | PlantUML | Database ERD đầy đủ với tất cả các trường và quan hệ chi tiết |
| `00-Database-ERD-Simple.puml` | PlantUML | Database ERD đơn giản, chỉ hiển thị các trường quan trọng |
| `00-Database-ERD.dbml` | DBML | Database ERD đầy đủ - **Paste vào dbdiagram.io** |
| `00-Database-ERD-Simple.dbml` | DBML | Database ERD đơn giản - **Paste vào dbdiagram.io** |

### Activity Diagrams

| File | Mô tả |
|------|------|
| `01-Tao-Thiet-Ke-Form.puml` | Quy trình Admin tạo form mới, thiết kế metadata và kích hoạt version |
| `02-Dien-Luu-Du-Lieu.puml` | Quy trình User điền form, validate và lưu dữ liệu vào database |
| `03-Xem-Sua-Du-Lieu.puml` | Quy trình xem và cập nhật dữ liệu form đã lưu |
| `04-Tao-Version-Moi.puml` | Quy trình tự động tạo version khi form đã tồn tại |
| `05-Validate-Du-Lieu.puml` | Quy trình chi tiết về validation engine |
| `06-Tong-Hop.puml` | Tổng quan luồng sử dụng hệ thống từ các actor |

## 🔧 Export sang các định dạng khác

### PNG/SVG

Sử dụng PlantUML command line:

```bash
# Cài đặt PlantUML (nếu chưa có)
# Windows: choco install plantuml
# Mac: brew install plantuml
# Linux: apt-get install plantuml

# Export sang PNG
plantuml -tpng *.puml

# Export sang SVG
plantuml -tsvg *.puml
```

### Online Export

1. Mở diagram trên PlantUML Web Server
2. Click vào format muốn export (PNG, SVG, ASCII Art)
3. Download file

## 📝 Ghi chú

- Tất cả các file `.puml` sử dụng UTF-8 encoding
- Các diagram có thể được chỉnh sửa trực tiếp trong text editor
- PlantUML Web Server tự động render diagram khi paste code
- Để preview trong VS Code, cài extension "PlantUML"

## 🔗 Liên kết hữu ích

- PlantUML Web Server: http://www.plantuml.com/plantuml/uml/
- PlantUML Documentation: https://plantuml.com/
- PlantUML Syntax Reference: https://plantuml.com/activity-diagram-beta

---

**Cập nhật lần cuối: 2024-01-21**
