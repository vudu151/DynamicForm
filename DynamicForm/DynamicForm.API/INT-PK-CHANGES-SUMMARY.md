# TÓM TẮT THAY ĐỔI: INT PK + GUID PublicId

## ✅ ĐÃ HOÀN THÀNH

### 1. Models
- ✅ Form: Id (int), PublicId (Guid), CurrentVersionId (int?)
- ✅ FormVersion: Id (int), PublicId (Guid), FormId (int)
- ✅ FormField: Id (int), PublicId (Guid), FormVersionId (int), ParentFieldId (int?)
- ✅ FieldValidation: Id (int), PublicId (Guid), FieldId (int)
- ✅ FieldCondition: Id (int), PublicId (Guid), FieldId (int)
- ✅ FieldOption: Id (int), PublicId (Guid), FieldId (int)
- ✅ FormDataValue: Id (int), PublicId (Guid), SubmissionId (int), FormVersionId (int), FormFieldId (int)

### 2. ApplicationDbContext
- ✅ Index unique cho PublicId trên tất cả bảng
- ✅ Config IDENTITY cho INT PK
- ✅ Foreign keys dùng INT

### 3. Services (MỘT PHẦN)
- ✅ GetFormByIdAsync: Map PublicId → Id
- ✅ GetVersionsByFormIdAsync: Map PublicId → Id
- ✅ GetFormMetadataByVersionIdAsync: Map PublicId → Id
- ✅ GetFormMetadataAsync: Return PublicId trong DTOs
- ⏳ UpdateFormMetadataByVersionIdAsync: Cần cập nhật logic map PublicId
- ⏳ CreateFormAsync: Tạo PublicId mới
- ⏳ CreateVersionAsync: Tạo PublicId mới
- ⏳ ActivateVersionAsync: Map PublicId → Id
- ⏳ FormDataService: Tất cả methods cần cập nhật

## ⏳ CẦN LÀM TIẾP

### 1. FormService - Các methods còn lại
- UpdateFormMetadataByVersionIdAsync: Map PublicId trong request.Fields
- CreateFormAsync: Tạo PublicId mới
- CreateVersionAsync: Tạo PublicId mới, map FormId
- ActivateVersionAsync: Map PublicId → Id

### 2. FormDataService - Tất cả methods
- GetFormDataAsync: Map PublicId → SubmissionId
- GetFormDataByObjectAsync: Query bằng PublicId
- CreateFormDataAsync: 
  - Map FormVersionId (PublicId → Id)
  - Tạo SubmissionId mới (INT)
  - Tạo PublicId cho FormDataValue
- UpdateFormDataAsync: Map PublicId → SubmissionId

### 3. SubmissionId Logic
Cần quyết định cách tạo SubmissionId (INT):
- **Option 1**: Dùng SEQUENCE trong SQL Server
- **Option 2**: Query MAX(SubmissionId) + 1 (có race condition)
- **Option 3**: Tạo bảng Submission riêng với INT PK

### 4. Controllers
- Không cần thay đổi (đã nhận Guid từ API)

## LƯU Ý QUAN TRỌNG

1. **DTOs giữ nguyên Guid**: DTOs vẫn dùng Guid (đây là PublicId)
2. **Mapping**: Services cần map PublicId ↔ Id khi query
3. **SubmissionId**: Cần logic tạo SubmissionId mới (INT)
