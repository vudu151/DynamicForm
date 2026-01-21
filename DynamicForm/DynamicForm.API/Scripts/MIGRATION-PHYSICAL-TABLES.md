# MIGRATION: CHUYỂN SANG PHYSICAL TABLES

## TÓM TẮT THAY ĐỔI

### Core Principle
- **Form metadata is dynamic**, nhưng **business data phải lưu trong physical tables**
- **FormVersion = Database Schema Version**
- Khi FormVersion được **Publish**, system tự động generate physical tables
- **JSON chỉ dùng cho audit/logging**, KHÔNG dùng để query/report

---

## THAY ĐỔI MODELS

### 1. FormVersion
- ✅ Thêm `Status` (0=Draft, 1=Published, 2=Archived)
- ✅ Thêm `PublishedDate`, `PublishedBy`
- ✅ Giữ `IsActive`, `ApprovedDate`, `ApprovedBy` (backward compatibility)
- ✅ Thêm navigation property `PhysicalTables`

### 2. FormPhysicalTable (đã có)
- ✅ Lưu thông tin bảng vật lý tương ứng với FormVersion
- ✅ TableName theo rule: `FORM_{FORM_CODE}_V{VERSION}`

### 3. FormFieldColumnMap (đã có)
- ✅ Map giữa FormField và Column trong physical table
- ✅ Lưu ColumnName và DataType

---

## THAY ĐỔI SERVICES

### 1. PhysicalTableService
- ✅ `EnsurePhysicalTablesForVersionAsync`: Generate physical tables khi publish
- ✅ `InsertDataAsync`: Insert vào physical tables thay vì JSON
- ✅ Naming rule: `FORM_{FORM_CODE}_V{VERSION}` (main table)
- ✅ Naming rule: `FORM_{FORM_CODE}_V{VERSION}_{SECTION_CODE}` (child table)
- ✅ FieldType mapping: text→NVARCHAR(255), number→DECIMAL(18,2), date→DATETIME2, select→NVARCHAR(100)
- ✅ IsRequired → NOT NULL constraint

### 2. FormService
- ✅ `ActivateVersionAsync` → Publish version (Status = 1)
- ✅ Khi publish → Generate physical tables
- ✅ Archive các version cũ (Status = 2)

### 3. FormDataService
- ✅ `CreateFormDataAsync`: 
  - Validate dữ liệu
  - Check FormVersion.Status = Published
  - Insert vào physical table
  - Lưu JSON vào FormData (chỉ để audit, không query)

---

## DATABASE MIGRATION SCRIPT

```sql
-- 1. Add Status column to FormVersions
ALTER TABLE FormVersions
ADD Status INT NOT NULL DEFAULT 0; -- 0=Draft, 1=Published, 2=Archived

-- 2. Add PublishedDate, PublishedBy
ALTER TABLE FormVersions
ADD PublishedDate DATETIME2 NULL,
    PublishedBy NVARCHAR(100) NULL;

-- 3. Migrate existing data: IsActive = true → Status = 1
UPDATE FormVersions
SET Status = 1, PublishedDate = ApprovedDate, PublishedBy = ApprovedBy
WHERE IsActive = 1;

-- 4. Create index on Status
CREATE INDEX IX_FormVersions_Status ON FormVersions(Status);
CREATE INDEX IX_FormVersions_FormId_Status ON FormVersions(FormId, Status);

-- 5. FormPhysicalTables và FormFieldColumnMaps đã có sẵn trong schema
```

---

## WORKFLOW MỚI

### 1. Tạo Form Version (Draft)
```
Admin → Create FormVersion (Status = 0 = Draft)
→ Define FormFields
→ Status vẫn là Draft
```

### 2. Publish FormVersion
```
Admin → Publish FormVersion
→ Status = 1 (Published)
→ System tự động generate physical tables:
   - FORM_{FORM_CODE}_V{VERSION} (main table)
   - FORM_{FORM_CODE}_V{VERSION}_{SECTION_CODE} (child tables nếu có)
→ Lưu metadata vào FormPhysicalTables, FormFieldColumnMaps
```

### 3. Insert Data
```
User → Submit FormData
→ Validate dữ liệu
→ Check FormVersion.Status = Published
→ Insert vào physical table (không phải JSON)
→ Lưu JSON vào FormData (audit only)
```

### 4. Query/Report
```
→ Query trực tiếp trên physical tables
→ KHÔNG query JSON
→ High performance, có thể index
```

---

## LƯU Ý

1. **Backward Compatibility**: 
   - Giữ `IsActive`, `ApprovedDate`, `ApprovedBy` để không break code cũ
   - Map `IsActive = true` → `Status = 1`

2. **FormData.DataJson**:
   - Vẫn lưu JSON để audit/logging
   - KHÔNG dùng để query/report
   - Có thể xóa sau nếu không cần

3. **Old Data**:
   - Dữ liệu cũ vẫn trong FormData.DataJson
   - Có thể migrate sau nếu cần

---

## TESTING CHECKLIST

- [ ] Tạo FormVersion (Draft) → Status = 0
- [ ] Publish FormVersion → Status = 1, generate physical tables
- [ ] Insert data vào published version → Insert vào physical table
- [ ] Insert data vào draft version → Error (phải Published)
- [ ] Query data từ physical table → OK
- [ ] Tạo version mới → Generate tables mới, tables cũ giữ nguyên
