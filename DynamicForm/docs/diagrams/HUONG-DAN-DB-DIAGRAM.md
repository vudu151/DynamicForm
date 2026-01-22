# 🗄️ HƯỚNG DẪN PASTE VÀO DB DIAGRAM

## 🎯 Công cụ hỗ trợ

Có 2 file DBML đã được tạo sẵn để paste vào các công cụ vẽ database diagram:

1. **`00-Database-ERD.dbml`** - ERD đầy đủ với tất cả các trường và chi tiết
2. **`00-Database-ERD-Simple.dbml`** - ERD đơn giản, dễ xem

## 🚀 Cách sử dụng với dbdiagram.io (Khuyến nghị)

### Bước 1: Mở dbdiagram.io
Truy cập: **https://dbdiagram.io/**

### Bước 2: Tạo diagram mới
- Click "Create New Diagram"
- Hoặc đăng nhập để lưu diagram

### Bước 3: Paste code DBML
1. Mở file `.dbml` (ví dụ: `00-Database-ERD.dbml`)
2. Copy toàn bộ code (Ctrl+A, Ctrl+C)
3. Paste vào editor của dbdiagram.io (Ctrl+V)
4. Diagram sẽ tự động render!

### Bước 4: Xem và chỉnh sửa
- Diagram sẽ hiển thị tất cả các bảng và quan hệ
- Có thể kéo thả để sắp xếp lại
- Có thể chỉnh sửa code DBML trực tiếp

### Bước 5: Export
- Click "Export" → Chọn format (PNG, PDF, SQL, PostgreSQL, MySQL, etc.)
- Download file về máy

## 📋 Các công cụ khác hỗ trợ DBML

### 1. dbdiagram.io (Khuyến nghị)
- ✅ Online, không cần cài đặt
- ✅ Hỗ trợ DBML format
- ✅ Export nhiều format
- ✅ Có thể share và collaborate

### 2. MySQL Workbench
- Import SQL script (cần convert DBML sang SQL)
- Hoặc vẽ thủ công

### 3. SQL Server Management Studio (SSMS)
- Sử dụng Database Diagram tool
- Hoặc import SQL script

### 4. Draw.io / diagrams.net
- Import SQL hoặc vẽ thủ công
- Không hỗ trợ DBML trực tiếp

## 🔄 Convert DBML sang SQL (nếu cần)

Nếu công cụ của bạn chỉ hỗ trợ SQL, có thể:
1. Paste DBML vào dbdiagram.io
2. Export sang SQL format
3. Sử dụng SQL script đó

## 📝 Format DBML

DBML (Database Markup Language) là format text-based để mô tả database schema:

```dbml
Table Users {
  Id int [pk, increment]
  Name varchar(100) [not null]
  Email varchar(255) [unique]
  
  Note: 'Users table description'
}

Table Posts {
  Id int [pk, increment]
  UserId int [ref: > Users.Id]
  Title varchar(200) [not null]
}
```

## ✅ Checklist

- [ ] Đã mở file `.dbml`
- [ ] Đã copy toàn bộ code
- [ ] Đã mở dbdiagram.io
- [ ] Đã paste code vào editor
- [ ] Diagram đã render thành công
- [ ] Đã kiểm tra các quan hệ
- [ ] Đã export nếu cần

## 🎨 Tùy chỉnh trong dbdiagram.io

### Thay đổi màu sắc
Thêm vào đầu file DBML:
```dbml
Project {
  database_type: 'SQL Server'
  Note: 'DynamicForm System Database'
}
```

### Thêm notes
```dbml
Table Forms {
  ...
  Note: 'Forms table - Main form definitions'
}
```

### Tùy chỉnh quan hệ
Quan hệ đã được định nghĩa sẵn trong DBML:
- `ref: >` = Foreign Key
- `ref: <` = Reverse relationship
- `ref: -` = One-to-one

## 🔗 Liên kết hữu ích

- **dbdiagram.io**: https://dbdiagram.io/
- **DBML Documentation**: https://dbml.dbdiagram.io/home
- **DBML Syntax**: https://dbml.dbdiagram.io/docs

## ❓ Troubleshooting

### Diagram không render
- ✅ Kiểm tra code có đầy đủ không
- ✅ Kiểm tra syntax DBML có đúng không
- ✅ Thử refresh trang

### Quan hệ không hiển thị
- ✅ Kiểm tra cú pháp `ref: >` hoặc `ref: <`
- ✅ Đảm bảo table name đúng

### Export không hoạt động
- ✅ Đăng nhập vào dbdiagram.io
- ✅ Thử export sang format khác

---

**Chúc bạn vẽ database diagram thành công! 🎉**
