# TỔNG HỢP: CÁC BẢNG VÀ LOGIC HIỆN TẠI

## 📊 CÁC BẢNG CÒN LẠI (7 bảng chính)

### 1. **Forms** - Bảng Form chính
**Mục đích**: Lưu thông tin form (metadata cấp cao)

**Các trường chính**:
- `Id` (Guid, PK)
- `Code` (string, unique) - Mã form duy nhất
- `Name` (string) - Tên form
- `Description` (string, nullable)
- `Status` (int) - 0=Draft, 1=Active, 2=Inactive
- `CurrentVersionId` (Guid, nullable, FK) - Version đang active
- `CreatedDate`, `CreatedBy`, `ModifiedDate`, `ModifiedBy`

**Quan hệ**:
- 1 Form → Nhiều FormVersion (Versions)
- 1 Form → 1 FormVersion (CurrentVersion)

---

### 2. **FormVersions** - Bảng Version của Form
**Mục đích**: Quản lý versioning của form (mỗi form có thể có nhiều version)

**Các trường chính**:
- `Id` (Guid, PK)
- `FormId` (Guid, FK → Forms)
- `Version` (string) - Số version (ví dụ: "1.0", "2.0")
- `Status` (int) - **0=Draft, 1=Published, 2=Archived**
- `PublishedDate`, `PublishedBy` - Khi nào và ai publish
- `CreatedDate`, `CreatedBy`
- `ChangeLog` (string, nullable) - Ghi chú thay đổi

**Quan hệ**:
- 1 FormVersion → 1 Form (Form)
- 1 FormVersion → Nhiều FormField (Fields)
- 1 FormVersion → Nhiều FormData (FormData)

**Logic Status**:
- **Draft (0)**: Version đang được thiết kế, chưa publish
- **Published (1)**: Version đã publish, có thể điền dữ liệu
- **Archived (2)**: Version cũ, không còn dùng

---

### 3. **FormFields** - Bảng Field của Form
**Mục đích**: Định nghĩa các field trong form (metadata chi tiết)

**Các trường chính**:
- `Id` (Guid, PK)
- `FormVersionId` (Guid, FK → FormVersions)
- `FieldCode` (string) - Mã field duy nhất trong version
- `FieldType` (int) - 1=Text, 2=Number, 3=Date, 4=Select, 10=Textarea
- `Label` (string) - Nhãn hiển thị
- `DisplayOrder` (int) - Thứ tự hiển thị
- `IsRequired` (bool) - Bắt buộc?
- `IsVisible` (bool) - Hiển thị?
- `DefaultValue`, `Placeholder`, `HelpText`, `CssClass`
- `PropertiesJson` (string, nullable) - JSON cho properties động
- `ParentFieldId` (Guid, nullable, FK) - Field cha (cho nested/repeater)
- `MinOccurs`, `MaxOccurs` (int, nullable) - Số lần xuất hiện (cho repeat section)
- `SectionCode` (string, nullable) - Mã section (cho repeat section)

**Quan hệ**:
- 1 FormField → 1 FormVersion (FormVersion)
- 1 FormField → 1 FormField (ParentField) - Cho nested fields
- 1 FormField → Nhiều FormField (ChildFields)
- 1 FormField → Nhiều FieldValidation (Validations)
- 1 FormField → Nhiều FieldCondition (Conditions)
- 1 FormField → Nhiều FieldOption (Options)

---

### 4. **FieldValidations** - Bảng Validation Rules
**Mục đích**: Lưu các rule validation cho field

**Các trường chính**:
- `Id` (Guid, PK)
- `FieldId` (Guid, FK → FormFields)
- `RuleType` (int) - 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex
- `RuleValue` (string) - Giá trị rule
- `ErrorMessage` (string) - Thông báo lỗi
- `Priority` (int) - Độ ưu tiên
- `IsActive` (bool) - Có active?

**Quan hệ**:
- 1 FieldValidation → 1 FormField (Field)

---

### 5. **FieldConditions** - Bảng Conditional Logic
**Mục đích**: Lưu điều kiện hiển thị/ẩn field (conditional logic)

**Các trường chính**:
- `Id` (Guid, PK)
- `FieldId` (Guid, FK → FormFields)
- `ConditionType` (int) - Loại điều kiện
- `Expression` (string) - Biểu thức điều kiện
- `ActionsJson` (string) - JSON cho actions (show/hide, enable/disable)
- `Priority` (int) - Độ ưu tiên

**Quan hệ**:
- 1 FieldCondition → 1 FormField (Field)

---

### 6. **FieldOptions** - Bảng Options cho Select/Dropdown
**Mục đích**: Lưu các option cho field type Select

**Các trường chính**:
- `Id` (Guid, PK)
- `FieldId` (Guid, FK → FormFields)
- `Value` (string) - Giá trị option
- `Label` (string) - Nhãn hiển thị
- `DisplayOrder` (int) - Thứ tự hiển thị
- `IsDefault` (bool) - Mặc định?

**Quan hệ**:
- 1 FieldOption → 1 FormField (Field)

---

### 7. **FormData** - Bảng Dữ liệu Form (JSON Storage)
**Mục đích**: Lưu dữ liệu đã điền vào form (lưu dạng JSON)

**Các trường chính**:
- `Id` (Guid, PK)
- `FormVersionId` (Guid, FK → FormVersions) - Version nào được dùng
- `ObjectId` (string) - ID của object liên quan (ví dụ: KhamBenhId)
- `ObjectType` (string) - Loại object (ví dụ: "KhamBenh", "DieuTri")
- `DataJson` (string) - **Dữ liệu dạng JSON** (ví dụ: `{"field1": "value1", "field2": "value2"}`)
- `CreatedDate`, `CreatedBy`
- `ModifiedDate`, `ModifiedBy`
- `Status` (int) - 0=Draft, 1=Submitted, 2=Approved

**Quan hệ**:
- 1 FormData → 1 FormVersion (FormVersion)

**Lưu ý**: 
- Dữ liệu được lưu dạng JSON trong `DataJson`
- Không có physical tables nữa
- Query/report sẽ query trực tiếp từ JSON

---

## 🔄 LOGIC FLOW HIỆN TẠI

### **1. TẠO FORM (Designer Flow)**

```
1. Admin tạo Form mới
   → POST /api/forms
   → Tạo record trong bảng Forms
   → Status = 0 (Draft)

2. Admin tạo FormVersion
   → POST /api/forms/{formId}/versions
   → Tạo record trong bảng FormVersions
   → Status = 0 (Draft)

3. Admin định nghĩa FormFields
   → PUT /api/forms/versions/{versionId}/metadata
   → Tạo/update records trong bảng FormFields
   → Có thể thêm FieldValidations, FieldConditions, FieldOptions

4. Admin Publish Version
   → POST /api/forms/versions/{versionId}/activate
   → FormVersion.Status = 1 (Published)
   → FormVersion.PublishedDate = DateTime.Now
   → Form.CurrentVersionId = versionId
   → Form.Status = 1 (Active)
   → Các version khác của form → Status = 2 (Archived)
```

**Lưu ý**: 
- Chỉ có thể điền dữ liệu vào version đã Published (Status = 1)
- Version Draft (Status = 0) không thể điền dữ liệu

---

### **2. ĐIỀN FORM (User Flow)**

```
1. User lấy metadata form
   → GET /api/forms/code/{code}/metadata
   → Trả về Form + FormVersion + FormFields + Validations + Conditions + Options
   → Frontend render form động dựa trên metadata

2. User điền dữ liệu và submit
   → POST /api/formdata
   → Validate dữ liệu:
      - Check FormVersion.Status = 1 (Published)
      - Validate theo FieldValidations
      - Check IsRequired, Min, Max, Range, Regex
   → Nếu valid:
      - Tạo record trong bảng FormData
      - Lưu dữ liệu vào DataJson (JSON format)
      - FormData.Status = 0 (Draft)

3. User update dữ liệu
   → PUT /api/formdata/{id}
   → Validate lại
   → Update DataJson
   → Update ModifiedDate, ModifiedBy
```

**Lưu ý**:
- Dữ liệu được lưu dạng JSON trong `FormData.DataJson`
- Không có physical tables
- Query/report sẽ query từ JSON

---

### **3. VALIDATION LOGIC**

```
Khi submit form data:

1. Check FormVersion.Status = Published (1)
   → Nếu không → Error: "Version must be Published"

2. Validate từng field:
   - Check IsRequired → Nếu null/empty → Error
   - Check FieldValidations:
     * RuleType = 2 (Min) → Check value >= min
     * RuleType = 3 (Max) → Check value <= max
     * RuleType = 4 (Range) → Check min <= value <= max
     * RuleType = 5 (Regex) → Check pattern match

3. Nếu có lỗi → Return ValidationResultDto với danh sách errors
4. Nếu không có lỗi → Save vào FormData
```

---

### **4. VERSIONING LOGIC**

```
Khi tạo version mới:

1. FormVersion mới → Status = 0 (Draft)
2. Có thể edit metadata (FormFields) thoải mái
3. Khi Publish:
   - Version mới → Status = 1 (Published)
   - Version cũ (nếu đang Published) → Status = 2 (Archived)
   - Form.CurrentVersionId = version mới

4. Dữ liệu cũ:
   - FormData vẫn giữ FormVersionId cũ
   - Không bị ảnh hưởng
   - Có thể query theo FormVersionId để lấy dữ liệu version cũ
```

**Lưu ý**:
- Mỗi FormData gắn với 1 FormVersion cụ thể
- Khi version mới được publish, dữ liệu cũ vẫn giữ nguyên
- Có thể query dữ liệu theo version

---

### **5. QUERY/REPORT LOGIC**

```
Hiện tại (JSON Storage):

1. Query FormData:
   → SELECT * FROM FormData WHERE FormVersionId = @versionId
   → Deserialize DataJson để lấy dữ liệu

2. Query theo Object:
   → SELECT * FROM FormData WHERE ObjectId = @objectId AND ObjectType = @objectType

3. Query JSON fields:
   → Có thể dùng SQL Server JSON functions:
     * JSON_VALUE(DataJson, '$.fieldCode')
     * JSON_QUERY(DataJson, '$.fieldCode')
     * OPENJSON(DataJson)

4. Report:
   → Query FormData
   → Parse JSON
   → Aggregate/Group by fields
```

**Lưu ý**:
- Performance có thể chậm hơn physical tables
- Cần index trên FormVersionId, ObjectId, ObjectType
- Có thể dùng SQL Server JSON functions để query

---

## 📋 TÓM TẮT QUAN HỆ BẢNG

```
Forms (1)
  └── FormVersions (N)
        ├── FormFields (N)
        │     ├── FieldValidations (N)
        │     ├── FieldConditions (N)
        │     └── FieldOptions (N)
        └── FormData (N)
              └── DataJson (JSON storage)
```

---

## 🎯 CÁC API ENDPOINTS

### **FormsController**
- `GET /api/forms` - Lấy tất cả forms
- `GET /api/forms/{id}` - Lấy form theo ID
- `GET /api/forms/code/{code}` - Lấy form theo Code
- `GET /api/forms/code/{code}/metadata` - Lấy metadata form (để render UI)
- `GET /api/forms/{formId}/versions` - Lấy tất cả versions của form
- `GET /api/forms/versions/{versionId}/metadata` - Lấy metadata theo version
- `PUT /api/forms/versions/{versionId}/metadata` - Update metadata (FormFields)
- `POST /api/forms` - Tạo form mới
- `POST /api/forms/{formId}/versions` - Tạo version mới
- `POST /api/forms/versions/{versionId}/activate` - Publish version
- `POST /api/forms/{formId}/deactivate` - Deactivate form

### **FormDataController**
- `GET /api/formdata/{id}` - Lấy form data theo ID
- `GET /api/formdata/object/{objectId}/{objectType}` - Lấy form data theo Object
- `POST /api/formdata` - Tạo form data mới (submit form)
- `PUT /api/formdata/{id}` - Update form data
- `POST /api/formdata/validate` - Validate form data (không lưu)

---

## ⚠️ LƯU Ý QUAN TRỌNG

1. **JSON Storage**: Dữ liệu được lưu dạng JSON, không có physical tables
2. **Versioning**: Mỗi FormData gắn với 1 FormVersion cụ thể
3. **Publish Required**: Chỉ có thể điền dữ liệu vào version đã Published
4. **Validation**: Validate cả client-side và server-side
5. **Query Performance**: Query JSON có thể chậm hơn physical tables, cần index phù hợp
