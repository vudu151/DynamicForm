# XÁC NHẬN ERD DIAGRAM

## ✅ KIỂM TRA CÁC QUAN HỆ

### 1. Forms → FormVersions

**Quan hệ 1: One-to-Many (FormId)**
- ✅ **Forms** (1) → **FormVersions** (N)
- Foreign Key: `FormVersions.FormId` → `Forms.Id`
- Delete Behavior: `Restrict` (không cho xóa Form nếu còn Version)

**Quan hệ 2: One-to-One (CurrentVersionId)**
- ✅ **Forms** (1) → **FormVersions** (1) - Optional
- Foreign Key: `Forms.CurrentVersionId` → `FormVersions.Id`
- Delete Behavior: `SetNull` (khi xóa Version, CurrentVersionId = null)
- **Lưu ý**: Quan hệ này là optional (nullable)

---

### 2. FormVersions → FormFields

**Quan hệ: One-to-Many**
- ✅ **FormVersions** (1) → **FormFields** (N)
- Foreign Key: `FormFields.FormVersionId` → `FormVersions.Id`
- Delete Behavior: `Cascade` (xóa Version → xóa tất cả Fields)

---

### 3. FormVersions → FormDataValues

**Quan hệ: One-to-Many**
- ✅ **FormVersions** (1) → **FormDataValues** (N)
- Foreign Key: `FormDataValues.FormVersionId` → `FormVersions.Id`
- Delete Behavior: `Restrict` (không cho xóa Version nếu còn Data)

---

### 4. FormFields → FieldValidations

**Quan hệ: One-to-Many**
- ✅ **FormFields** (1) → **FieldValidations** (N)
- Foreign Key: `FieldValidations.FieldId` → `FormFields.Id`
- Delete Behavior: `Cascade` (xóa Field → xóa tất cả Validations)

---

### 5. FormFields → FieldConditions

**Quan hệ: One-to-Many**
- ✅ **FormFields** (1) → **FieldConditions** (N)
- Foreign Key: `FieldConditions.FieldId` → `FormFields.Id`
- Delete Behavior: `Cascade` (xóa Field → xóa tất cả Conditions)

---

### 6. FormFields → FieldOptions

**Quan hệ: One-to-Many**
- ✅ **FormFields** (1) → **FieldOptions** (N)
- Foreign Key: `FieldOptions.FieldId` → `FormFields.Id`
- Delete Behavior: `Cascade` (xóa Field → xóa tất cả Options)

---

### 7. FormFields → FormDataValues

**Quan hệ: One-to-Many**
- ✅ **FormFields** (1) → **FormDataValues** (N)
- Foreign Key: `FormDataValues.FormFieldId` → `FormFields.Id`
- Delete Behavior: `Restrict` (không cho xóa Field nếu còn Data)

---

### 8. FormFields → FormFields (Self-referencing)

**Quan hệ: One-to-Many (Parent-Child)**
- ✅ **FormFields** (1) → **FormFields** (N) - Optional
- Foreign Key: `FormFields.ParentFieldId` → `FormFields.Id`
- Delete Behavior: `NoAction` (để tránh cascade path conflict)
- **Mục đích**: Hỗ trợ nested fields / repeat sections

---

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. SubmissionId trong FormDataValues

**KHÔNG có Foreign Key constraint!**

- `SubmissionId` là `INT` tự quản lý
- Dùng để nhóm các `FormDataValue` của cùng 1 lần submit form
- Không có bảng `Submissions` riêng
- **Trong ERD, không nên vẽ FK từ FormDataValues đến bảng Submissions** (vì không tồn tại)

### 2. CurrentVersionId trong Forms

- Là **optional** (nullable)
- Có thể `null` nếu form chưa có version nào được kích hoạt
- Khi xóa version được reference, `CurrentVersionId` sẽ tự động set về `null`

### 3. ParentFieldId trong FormFields

- Là **optional** (nullable)
- Dùng cho nested fields / repeat sections
- Delete behavior là `NoAction` (không cascade) để tránh lỗi SQL Server về multiple cascade paths

---

## ✅ TÓM TẮT CÁC BẢNG

| Bảng | Số lượng | Mô tả |
|------|----------|-------|
| Forms | 1 | Bảng chính chứa thông tin form |
| FormVersions | 1 | Bảng chứa các version của form |
| FormFields | 1 | Bảng chứa các field trong version |
| FormDataValues | 1 | Bảng chứa dữ liệu đã submit |
| FieldValidations | 1 | Bảng chứa validation rules |
| FieldConditions | 1 | Bảng chứa conditional logic |
| FieldOptions | 1 | Bảng chứa options cho Select field |
| **Tổng** | **7 bảng** | |

---

## ✅ CHECKLIST ERD

- [x] Forms có quan hệ One-to-Many với FormVersions (qua FormId)
- [x] Forms có quan hệ One-to-One với FormVersions (qua CurrentVersionId) - Optional
- [x] FormVersions có quan hệ One-to-Many với FormFields
- [x] FormVersions có quan hệ One-to-Many với FormDataValues
- [x] FormFields có quan hệ One-to-Many với FieldValidations
- [x] FormFields có quan hệ One-to-Many với FieldConditions
- [x] FormFields có quan hệ One-to-Many với FieldOptions
- [x] FormFields có quan hệ One-to-Many với FormDataValues
- [x] FormFields có quan hệ Self-referencing (ParentFieldId) - Optional
- [x] SubmissionId trong FormDataValues **KHÔNG có FK** (tự quản lý)

---

## 📝 KẾT LUẬN

**ERD của bạn đã ĐÚNG** với thiết kế database hiện tại! 

Tất cả 7 bảng và các quan hệ đều khớp với code. Chỉ cần lưu ý:
1. **SubmissionId** không có FK constraint (không vẽ line đến bảng Submissions)
2. **CurrentVersionId** là optional (có thể null)
3. **ParentFieldId** là optional và dùng NoAction delete behavior
