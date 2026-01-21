# TÓM TẮT: CHỈ DÙNG FormDataValue, BỎ FormData

## THAY ĐỔI KIẾN TRÚC

### Trước đây:
- **FormData** (header) + **FormDataValue** (detail)
- FormData lưu: FormVersionId, ObjectId, ObjectType, Status, CreatedDate, ...
- FormDataValue lưu: FormDataId (FK), FormFieldId, FieldValue

### Bây giờ:
- **CHỈ FormDataValue** (độc lập)
- FormDataValue lưu tất cả: SubmissionId, FormVersionId, ObjectId, ObjectType, Status, FormFieldId, FieldValue, ...

---

## CẤU TRÚC MỚI

### FormDataValue (DUY NHẤT)
- `Id` (Guid, PK)
- `SubmissionId` (Guid) - **KEY ĐỂ NHÓM** các values của cùng 1 lần submit
- `FormVersionId` (Guid, FK → FormVersions)
- `FormFieldId` (Guid, FK → FormFields)
- `ObjectId` (string) - ID của object liên quan
- `ObjectType` (string) - Loại object
- `FieldValue` (string, max 4000) - Giá trị của field
- `DisplayOrder` (int) - Thứ tự (cho repeat section)
- `SectionCode` (string, nullable) - Mã section
- `Status` (int) - 0=Draft, 1=Submitted, 2=Approved
- `CreatedDate`, `CreatedBy`
- `ModifiedDate`, `ModifiedBy`

---

## LOGIC MỚI

### 1. Create FormData
```
Input: Dictionary<string, object> data
  {
    "HoTen": "Nguyen Van A",
    "NamSinh": "1990",
    "DiaChi": "Ha Noi"
  }

Process:
  1. Tạo SubmissionId mới (Guid.NewGuid())
  2. Với mỗi field trong data:
     - Tìm FormFieldId từ FieldCode
     - Tạo FormDataValue:
       * SubmissionId = submissionId (CÙNG CHO TẤT CẢ)
       * FormVersionId = request.FormVersionId
       * FormFieldId = field.Id
       * ObjectId = request.ObjectId
       * ObjectType = request.ObjectType
       * FieldValue = value.ToString()
       * DisplayOrder = 0 (hoặc index nếu là array)
       * Status = 0 (Draft)

Result:
  - 3 bản ghi FormDataValue (1 cho mỗi field)
  - Tất cả có cùng SubmissionId
```

### 2. Get FormData
```
Input: SubmissionId (id)

Process:
  1. Query FormDataValues WHERE SubmissionId = id
  2. Include FormVersion và FormField
  3. Group FormDataValues theo FieldCode
  4. Với repeat section (MaxOccurs > 1):
     - Tạo array từ các FormDataValue có cùng FieldCode
  5. Với field thường:
     - Lấy giá trị cuối cùng

Result:
  FormDataDto với:
  - Id = SubmissionId
  - FormVersionId, ObjectId, ObjectType từ FormDataValue đầu tiên
  - Data = Dictionary<string, object>
```

### 3. Get FormData By Object
```
Input: ObjectId, ObjectType

Process:
  1. Query FormDataValues WHERE ObjectId = @objectId AND ObjectType = @objectType
  2. OrderBy CreatedDate DESC
  3. Lấy SubmissionId đầu tiên (mới nhất)
  4. Gọi GetFormDataAsync(submissionId)

Result:
  FormDataDto của submission mới nhất
```

### 4. Update FormData
```
Input: SubmissionId (id), new data

Process:
  1. Query FormDataValues WHERE SubmissionId = id
  2. Xóa tất cả FormDataValues cũ
  3. Tạo lại FormDataValues mới với cùng SubmissionId
  4. Giữ nguyên CreatedDate, CreatedBy (từ values cũ)
  5. Set ModifiedDate, ModifiedBy = now

Result:
  FormDataDto với data mới
```

---

## VÍ DỤ

### Form: Khám bệnh
**Fields:**
- HoTen (FieldCode: "HoTen")
- NamSinh (FieldCode: "NamSinh")
- DiaChi (FieldCode: "DiaChi")

**User nhập:**
- HoTen: "Nguyen Van A"
- NamSinh: "1990"
- DiaChi: "Ha Noi"

**Database:**

**FormDataValues (3 bản ghi - CÙNG SubmissionId):**
```
SubmissionId: 123e4567-e89b-12d3-a456-426614174000 (CÙNG CHO TẤT CẢ)

1. Id: ..., SubmissionId: 123e..., FormVersionId: ..., FormFieldId: (HoTen), 
   ObjectId: "KB001", ObjectType: "KhamBenh", FieldValue: "Nguyen Van A", Status: 0

2. Id: ..., SubmissionId: 123e..., FormVersionId: ..., FormFieldId: (NamSinh), 
   ObjectId: "KB001", ObjectType: "KhamBenh", FieldValue: "1990", Status: 0

3. Id: ..., SubmissionId: 123e..., FormVersionId: ..., FormFieldId: (DiaChi), 
   ObjectId: "KB001", ObjectType: "KhamBenh", FieldValue: "Ha Noi", Status: 0
```

---

## LỢI ÍCH

1. **Đơn giản hơn**: Chỉ 1 bảng thay vì 2 bảng
2. **Query dễ dàng**: Query trực tiếp FormDataValue, không cần join FormData
3. **Index tốt**: Có thể index trên SubmissionId, ObjectId, ObjectType, FormFieldId
4. **Report dễ**: Có thể aggregate, group by field trực tiếp
5. **Performance**: Query field cụ thể nhanh hơn (không cần join)

---

## INDEXES QUAN TRỌNG

1. **IX_FormDataValues_SubmissionId** - Để group các values của cùng submission
2. **IX_FormDataValues_ObjectId_ObjectType_FormVersionId** - Để query theo object
3. **IX_FormDataValues_FormFieldId** - Để query theo field
4. **IX_FormDataValues_SubmissionId_FormFieldId_DisplayOrder** - Để query và order
5. **IX_FormDataValues_CreatedDate** - Để lấy submission mới nhất
6. **IX_FormDataValues_Status** - Để filter theo status

---

## MIGRATION

1. **Tạo bảng FormDataValues mới**: Chạy script `MigrateToFormDataValueOnly.sql`
2. **Migrate dữ liệu cũ** (nếu có): 
   - Lấy FormData → Tạo SubmissionId mới
   - Update FormDataValues: Set SubmissionId, FormVersionId, ObjectId, ObjectType, Status từ FormData
3. **Xóa bảng FormData**: Sau khi migrate xong

---

## LƯU Ý

1. **SubmissionId**: Mỗi lần submit tạo SubmissionId mới, tất cả FormDataValues của cùng submission có cùng SubmissionId
2. **Update**: Có thể update trực tiếp (xóa cũ, tạo mới với cùng SubmissionId) hoặc tạo SubmissionId mới để track history
3. **Query**: Luôn query theo SubmissionId để lấy tất cả values của cùng submission
4. **ObjectId + ObjectType**: Dùng để query submission mới nhất của object
