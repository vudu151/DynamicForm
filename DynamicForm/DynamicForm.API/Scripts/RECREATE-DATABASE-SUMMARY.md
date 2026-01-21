# TÓM TẮT: TẠO LẠI DATABASE VỚI INT PK + GUID PublicId

## 📋 FILE SCRIPT

**File:** `RecreateDatabaseWithIntPk.sql`

## ⚠️ LƯU Ý QUAN TRỌNG

**Script này sẽ XÓA TẤT CẢ DỮ LIỆU hiện có trong database!**

Chỉ chạy khi:
- Database đang ở môi trường development/test
- Hoặc đã backup dữ liệu cũ
- Hoặc muốn tạo lại database từ đầu

## 🔄 QUY TRÌNH

### Bước 1: Backup (Nếu cần)
```sql
-- Backup database nếu có dữ liệu quan trọng
BACKUP DATABASE DynamicFormDb TO DISK = 'C:\Backup\DynamicFormDb.bak';
```

### Bước 2: Chạy Script
```sql
-- Chạy file: RecreateDatabaseWithIntPk.sql
USE DynamicFormDb;
GO
-- ... (nội dung script)
```

## 📊 CẤU TRÚC MỚI

### 8 Bảng được tạo:

1. **Forms**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `Code`, `Name`, `Description`, `Status`
   - `CurrentVersionId`: INT (FK → FormVersions)

2. **FormVersions**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FormId`: INT (FK → Forms)
   - `Version`, `Status`, `PublishedDate`, `PublishedBy`

3. **FormFields**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FormVersionId`: INT (FK → FormVersions)
   - `FieldCode`, `FieldType`, `Label`, `DisplayOrder`
   - `ParentFieldId`: INT (FK → FormFields, nullable)

4. **FieldValidations**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FieldId`: INT (FK → FormFields)
   - `RuleType`, `RuleValue`, `ErrorMessage`

5. **FieldConditions**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FieldId`: INT (FK → FormFields)
   - `ConditionType`, `Expression`, `ActionsJson`

6. **FieldOptions**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FieldId`: INT (FK → FormFields)
   - `Value`, `Label`, `DisplayOrder`

7. **Submissions** (MỚI)
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `FormVersionId`: INT (FK → FormVersions)
   - `ObjectId`, `ObjectType`, `Status`
   - `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`

8. **FormDataValues**
   - `Id`: INT (IDENTITY, PK)
   - `PublicId`: GUID (UNIQUE, INDEXED)
   - `SubmissionId`: INT (FK → Submissions)
   - `FormVersionId`: INT (FK → FormVersions)
   - `FormFieldId`: INT (FK → FormFields)
   - `ObjectId`, `ObjectType`, `FieldValue`
   - `DisplayOrder`, `SectionCode`, `Status`

## 🗑️ CÁC BẢNG ĐÃ XÓA

Script sẽ xóa các bảng cũ (nếu có):
- `FormData` (đã thay bằng `Submissions` + `FormDataValues`)
- `FormDataHistory` (không còn dùng)
- `FormPermissions` (không còn dùng)
- `FormPhysicalTables` (không còn dùng)
- `FormFieldColumnMap` (không còn dùng)

## ✅ ĐẶC ĐIỂM

### 1. Primary Keys
- Tất cả bảng dùng **INT** với **IDENTITY(1,1)**
- Tự động tăng, query/report nhanh hơn

### 2. Public IDs
- Tất cả bảng có **PublicId** (GUID, UNIQUE, INDEXED)
- Dùng cho public API (không đoán được)
- Index unique để query nhanh

### 3. Foreign Keys
- Tất cả foreign keys dùng **INT** (thay vì GUID)
- Join nhanh hơn, index hiệu quả hơn

### 4. Indexes
- Index unique cho `PublicId` trên tất cả bảng
- Index cho các cột thường query: `Status`, `CreatedDate`, `ObjectId`, etc.
- Composite indexes cho các query phức tạp

## 🚀 SAU KHI CHẠY SCRIPT

1. Database đã sẵn sàng với cấu trúc mới
2. Có thể bắt đầu tạo dữ liệu mới
3. API sẽ tự động tạo `PublicId` mới cho mỗi bản ghi

## 📝 VÍ DỤ SỬ DỤNG

### Tạo Form mới
```sql
INSERT INTO Forms (Code, Name, Status, CreatedBy)
VALUES ('FORM001', 'Form Khám Bệnh', 0, 'Admin');
-- Id sẽ tự động tăng (1, 2, 3, ...)
-- PublicId sẽ tự động tạo (GUID)
```

### Query bằng PublicId
```sql
-- API nhận PublicId (GUID) từ client
SELECT * FROM Forms WHERE PublicId = '...guid...';

-- Internal query dùng Id (INT) - nhanh hơn
SELECT * FROM Forms WHERE Id = 1;
```

## 🎯 LỢI ÍCH

1. **Performance**: INT index nhỏ hơn, join nhanh hơn
2. **Bảo mật**: GUID không đoán được cho public API
3. **Báo cáo**: Query/aggregate nhanh hơn với INT
4. **Maintainability**: Cấu trúc rõ ràng, dễ maintain
