# 🗄️ HƯỚNG DẪN VẼ DATABASE ERD DIAGRAM

## 📊 Tổng quan

Hệ thống DynamicForm có **7 bảng chính** với các quan hệ phức tạp. Có 2 file ERD diagram:

1. **`00-Database-ERD.puml`** - ERD đầy đủ với tất cả các trường
2. **`00-Database-ERD-Simple.puml`** - ERD đơn giản, chỉ hiển thị các trường quan trọng

## 🚀 Cách sử dụng

### Bước 1: Chọn file ERD

- **ERD đầy đủ**: Dùng khi cần xem chi tiết tất cả các trường, constraints, và notes
- **ERD đơn giản**: Dùng khi cần xem nhanh cấu trúc và quan hệ giữa các bảng

### Bước 2: Render diagram

#### Cách 1: Copy trực tiếp (Nhanh nhất)

1. Mở file `.puml` (ví dụ: `00-Database-ERD.puml`)
2. Copy toàn bộ code (Ctrl+A, Ctrl+C)
3. Mở http://www.plantuml.com/plantuml/uml/
4. Paste code vào ô text
5. Click "Submit" để xem sơ đồ

#### Cách 2: Sử dụng PowerShell Script

```powershell
cd "d:\ONENET\5.Test Performance\DynamicForm\DynamicForm\docs\diagrams"
.\generate-url.ps1 00-Database-ERD.puml
```

#### Cách 3: Sử dụng Web Tool

1. Mở `generate-plantuml-url.html` trong trình duyệt
2. Load file `00-Database-ERD.puml` vào editor
3. Click "Mở PlantUML Web" để xem

## 📋 Cấu trúc Database

### 7 Bảng chính

1. **Forms** - Bảng form chính
2. **FormVersions** - Quản lý version của form
3. **FormFields** - Định nghĩa các field trong form
4. **FieldValidations** - Validation rules cho field
5. **FieldConditions** - Conditional logic cho field
6. **FieldOptions** - Options cho Select field
7. **FormDataValues** - Dữ liệu đã submit

### Quan hệ chính

```
Forms (1) ──< (N) FormVersions
  │                      │
  │                      ├──< (N) FormFields ──< (N) FieldValidations
  │                      │                    └──< (N) FieldConditions
  │                      │                    └──< (N) FieldOptions
  │                      │                    └──< (N) FormDataValues
  │                      │
  │                      └──< (N) FormDataValues
  │
  └──> (1) FormVersions (CurrentVersionId - Optional)
```

## 🔑 Điểm quan trọng

### 1. Primary Keys
- Tất cả bảng đều có `Id` (INT, IDENTITY) làm Primary Key
- Tất cả bảng đều có `PublicId` (GUID, UNIQUE) để expose ra API

### 2. Foreign Keys
- **Forms → FormVersions**: Quan hệ 1:N (FormId) và 1:1 (CurrentVersionId - optional)
- **FormVersions → FormFields**: Quan hệ 1:N (Cascade delete)
- **FormVersions → FormDataValues**: Quan hệ 1:N (Restrict delete)
- **FormFields → FieldValidations/Conditions/Options**: Quan hệ 1:N (Cascade delete)
- **FormFields → FormFields**: Self-referencing (ParentFieldId - optional)

### 3. SubmissionId (Đặc biệt)
- **KHÔNG có Foreign Key constraint**
- Tự quản lý, dùng để nhóm các FormDataValue của cùng 1 submission
- Tự động tăng khi tạo submission mới

### 4. Delete Behaviors
- **Cascade**: Xóa parent → tự động xóa children
  - FormVersions → FormFields
  - FormFields → FieldValidations, FieldConditions, FieldOptions
  
- **Restrict**: Không cho xóa parent nếu còn children
  - Forms → FormVersions
  - FormVersions → FormDataValues
  - FormFields → FormDataValues
  
- **SetNull**: Xóa parent → set foreign key về NULL
  - FormVersions → Forms.CurrentVersionId
  
- **NoAction**: Không có action tự động
  - FormFields → FormFields (ParentFieldId)

## 📝 Export sang các định dạng khác

### PNG/SVG (Command Line)

```bash
# Export sang PNG
plantuml -tpng 00-Database-ERD.puml

# Export sang SVG
plantuml -tsvg 00-Database-ERD.puml
```

### Online Export

1. Mở diagram trên PlantUML Web Server
2. Click vào format muốn export (PNG, SVG, ASCII Art)
3. Download file

## 🎨 Tùy chỉnh Diagram

### Thêm màu sắc

Thêm vào đầu file:
```plantuml
skinparam entity {
  BackgroundColor #E1F5FE
  BorderColor #01579B
}
```

### Thay đổi layout

Thêm vào đầu file:
```plantuml
skinparam linetype ortho
skinparam packageStyle rectangle
```

### Ẩn các trường không cần thiết

Chỉnh sửa entity để chỉ hiển thị các trường quan trọng.

## ❓ Troubleshooting

### Lỗi: "Cannot decode"
- ✅ Kiểm tra code có đầy đủ `@startuml` và `@enduml`
- ✅ Kiểm tra encoding file là UTF-8
- ✅ Copy lại toàn bộ code

### Diagram quá lớn, khó xem
- ✅ Sử dụng `00-Database-ERD-Simple.puml` thay vì file đầy đủ
- ✅ Chỉnh sửa để ẩn các trường không cần thiết

### Quan hệ không hiển thị đúng
- ✅ Kiểm tra cú pháp PlantUML entity relationship
- ✅ Xem documentation: https://plantuml.com/class-diagram

## 🔗 Tài liệu tham khảo

- PlantUML Entity Relationship: https://plantuml.com/class-diagram
- Database Schema: `../DATABASE-SCHEMA-DETAILED.md`
- ERD Verification: `../ERD-VERIFICATION.md`

---

**Chúc bạn vẽ ERD thành công! 🎉**
