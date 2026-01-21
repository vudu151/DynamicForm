# TÓM TẮT: CHUYỂN TỪ DataJson SANG FormDataValue

## THAY ĐỔI KIẾN TRÚC

### Trước đây:
- **FormData** lưu tất cả dữ liệu trong 1 trường `DataJson` (JSON format)
- Ví dụ: `{"HoTen": "Nguyen Van A", "NamSinh": "1990", "DiaChi": "Ha Noi"}`

### Bây giờ:
- **FormData** chỉ lưu thông tin header (FormVersionId, ObjectId, ObjectType, Status, ...)
- **FormDataValue** lưu từng giá trị field thành 1 bản ghi riêng
- Ví dụ: Form có 3 field → 3 bản ghi FormDataValue

---

## CẤU TRÚC MỚI

### 1. FormData (Header)
- `Id` (Guid, PK)
- `FormVersionId` (Guid, FK)
- `ObjectId` (string)
- `ObjectType` (string)
- `CreatedDate`, `CreatedBy`
- `ModifiedDate`, `ModifiedBy`
- `Status` (int)
- **Đã xóa**: `DataJson`

### 2. FormDataValue (Detail - MỚI)
- `Id` (Guid, PK)
- `FormDataId` (Guid, FK → FormData)
- `FormFieldId` (Guid, FK → FormField)
- `FieldValue` (string, max 4000) - Giá trị của field
- `DisplayOrder` (int) - Thứ tự (cho repeat section)
- `SectionCode` (string, nullable) - Mã section (cho repeat section)
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
  1. Tạo FormData header
  2. Với mỗi field trong data:
     - Tìm FormFieldId từ FieldCode
     - Tạo FormDataValue:
       * FormDataId = formData.Id
       * FormFieldId = field.Id
       * FieldValue = value.ToString()
       * DisplayOrder = 0 (hoặc index nếu là array)

Result:
  - 1 bản ghi FormData
  - 3 bản ghi FormDataValue (1 cho mỗi field)
```

### 2. Get FormData
```
Process:
  1. Query FormData với Include(Values).ThenInclude(FormField)
  2. Group FormDataValues theo FieldCode
  3. Với repeat section (MaxOccurs > 1):
     - Tạo array từ các FormDataValue có cùng FieldCode
  4. Với field thường:
     - Lấy giá trị cuối cùng

Result:
  Dictionary<string, object> data
  {
    "HoTen": "Nguyen Van A",
    "NamSinh": "1990",
    "DiaChi": "Ha Noi"
  }
```

### 3. Update FormData
```
Process:
  1. Xóa tất cả FormDataValues cũ
  2. Tạo lại FormDataValues mới (giống Create)
```

### 4. Repeat Section (MaxOccurs > 1)
```
Input: 
  {
    "DanhSachThuoc": ["Thuoc 1", "Thuoc 2", "Thuoc 3"]
  }

Process:
  - Tạo 3 FormDataValue:
    * FormDataValue 1: FieldValue = "Thuoc 1", DisplayOrder = 0
    * FormDataValue 2: FieldValue = "Thuoc 2", DisplayOrder = 1
    * FormDataValue 3: FieldValue = "Thuoc 3", DisplayOrder = 2

Get:
  - Group theo FieldCode và DisplayOrder
  - Tạo array: ["Thuoc 1", "Thuoc 2", "Thuoc 3"]
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

**FormData (1 bản ghi):**
```
Id: 123e4567-e89b-12d3-a456-426614174000
FormVersionId: ...
ObjectId: "KB001"
ObjectType: "KhamBenh"
Status: 0
```

**FormDataValues (3 bản ghi):**
```
1. Id: ..., FormDataId: 123e..., FormFieldId: (HoTen), FieldValue: "Nguyen Van A"
2. Id: ..., FormDataId: 123e..., FormFieldId: (NamSinh), FieldValue: "1990"
3. Id: ..., FormDataId: 123e..., FormFieldId: (DiaChi), FieldValue: "Ha Noi"
```

---

## LỢI ÍCH

1. **Query dễ dàng hơn**: Có thể query trực tiếp field value mà không cần parse JSON
2. **Index tốt hơn**: Có thể index trên FormFieldId, FieldValue
3. **Report dễ hơn**: Có thể aggregate, group by field
4. **Performance**: Query field cụ thể nhanh hơn (không cần parse toàn bộ JSON)

---

## MIGRATION

1. **Tạo bảng FormDataValues**: Chạy script `MigrateToFormDataValue.sql`
2. **Migrate dữ liệu cũ** (nếu có): Cần parse DataJson và tạo FormDataValue records
3. **Xóa cột DataJson**: Sau khi migrate xong

---

## LƯU Ý

1. **Dữ liệu cũ**: Nếu có dữ liệu trong DataJson, cần migrate trước khi xóa cột
2. **Repeat Section**: Cần xử lý đặc biệt cho MaxOccurs > 1
3. **FieldValue size**: Max 4000 characters, nếu cần lớn hơn có thể dùng NVARCHAR(MAX)
