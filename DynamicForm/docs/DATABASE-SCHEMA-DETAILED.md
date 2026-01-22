# TÀI LIỆU CHI TIẾT DATABASE SCHEMA - DYNAMIC FORM

> **Mục tiêu**: Mô tả chi tiết cấu trúc database, các bảng, trường, và quan hệ giữa các bảng
>
> **Database**: DatabaseNew (SQL Server)
>
> **Tổng số bảng**: 7 bảng

---

## TỔNG QUAN QUAN HỆ

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

---

## 1. BẢNG FORMS

### 1.1. Mô tả

Bảng chính chứa thông tin định nghĩa form. Mỗi form có thể có nhiều version, nhưng chỉ có 1 version đang active tại một thời điểm.

### 1.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ (dùng trong database) |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API, GUID) |
| **Code** | `NVARCHAR(50)` | NOT NULL, UNIQUE | Mã định danh duy nhất của form (VD: PHIEU_KHAM_BENH) |
| **Name** | `NVARCHAR(200)` | NOT NULL | Tên hiển thị của form |
| **Description** | `NVARCHAR(500)` | NULL | Mô tả form (có thể để trống) |
| **Status** | `INT` | NOT NULL, DEFAULT 0 | Trạng thái: 0=Draft, 1=Active, 2=Inactive |
| **CurrentVersionId** | `INT` | NULL, FOREIGN KEY | ID của version đang active (tham chiếu FormVersions.Id) |
| **CreatedDate** | `DATETIME2` | NOT NULL, DEFAULT GETUTCDATE() | Ngày giờ tạo form |
| **CreatedBy** | `NVARCHAR(100)` | NOT NULL, DEFAULT '' | Người tạo form |
| **ModifiedDate** | `DATETIME2` | NULL | Ngày giờ sửa đổi lần cuối |
| **ModifiedBy** | `NVARCHAR(100)` | NULL | Người sửa đổi lần cuối |

### 1.3. Indexes

- `IX_Forms_PublicId` (UNIQUE): Index cho PublicId để query nhanh trong API
- `IX_Forms_Code` (UNIQUE): Index cho Code để đảm bảo unique và query nhanh
- `IX_Forms_Status`: Index cho Status để filter theo trạng thái

### 1.4. Quan hệ

- **One-to-Many với FormVersions**: Một form có nhiều version (qua `FormVersions.FormId`)
  - Foreign Key: `FormVersions.FormId` → `Forms.Id`
  - Delete Behavior: `Restrict` (không cho xóa Form nếu còn Version)
  
- **One-to-One với FormVersions**: Một form có 1 version đang active (qua `Forms.CurrentVersionId`)
  - Foreign Key: `Forms.CurrentVersionId` → `FormVersions.Id`
  - Delete Behavior: `SetNull` (khi xóa Version, CurrentVersionId tự động set về NULL)
  - **Lưu ý**: Quan hệ này là optional (có thể NULL)

---

## 2. BẢNG FORMVERSIONS

### 2.1. Mô tả

Bảng chứa các version của form. Mỗi form có thể có nhiều version để quản lý lịch sử thay đổi. Chỉ có 1 version được kích hoạt (active) tại một thời điểm.

### 2.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **FormId** | `INT` | NOT NULL, FOREIGN KEY | ID của form (tham chiếu Forms.Id) |
| **Version** | `NVARCHAR(20)` | NOT NULL | Số version (VD: "1", "1.0.0", "2") |
| **Status** | `INT` | NOT NULL, DEFAULT 0 | Trạng thái: 0=Draft, 1=Published, 2=Archived |
| **IsActive** | `BIT` | NOT NULL, DEFAULT 0 | **Deprecated**: Dùng Status thay thế. Giữ lại để tương thích ngược |
| **CreatedDate** | `DATETIME2` | NOT NULL, DEFAULT GETUTCDATE() | Ngày giờ tạo version |
| **CreatedBy** | `NVARCHAR(100)` | NOT NULL, DEFAULT '' | Người tạo version |
| **PublishedDate** | `DATETIME2` | NULL | Ngày giờ publish version (khi Status = 1) |
| **PublishedBy** | `NVARCHAR(100)` | NULL | Người publish version |
| **ApprovedDate** | `DATETIME2` | NULL | **Deprecated**: Dùng PublishedDate thay thế |
| **ApprovedBy** | `NVARCHAR(100)` | NULL | **Deprecated**: Dùng PublishedBy thay thế |
| **ChangeLog** | `NVARCHAR(1000)` | NULL | Mô tả thay đổi của version này |

### 2.3. Indexes

- `IX_FormVersions_PublicId` (UNIQUE): Index cho PublicId
- `IX_FormVersions_FormId_Version` (UNIQUE): Composite unique index đảm bảo version unique trong một form
- `IX_FormVersions_FormId_Status`: Composite index để query version theo form và status
- `IX_FormVersions_Status`: Index cho Status

### 2.4. Quan hệ

- **Many-to-One với Forms**: Nhiều version thuộc về một form
  - Foreign Key: `FormVersions.FormId` → `Forms.Id`
  - Delete Behavior: `Restrict` (không cho xóa Form nếu còn Version)
  
- **One-to-Many với FormFields**: Một version có nhiều field
  - Foreign Key: `FormFields.FormVersionId` → `FormVersions.Id`
  - Delete Behavior: `Cascade` (xóa Version → xóa tất cả Fields)
  
- **One-to-Many với FormDataValues**: Một version có nhiều dữ liệu đã submit
  - Foreign Key: `FormDataValues.FormVersionId` → `FormVersions.Id`
  - Delete Behavior: `Restrict` (không cho xóa Version nếu còn Data)

---

## 3. BẢNG FORMFIELDS

### 3.1. Mô tả

Bảng chứa định nghĩa các field (trường) trong một version của form. Mỗi field có thể có validation rules, conditions, và options (nếu là Select).

### 3.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **FormVersionId** | `INT` | NOT NULL, FOREIGN KEY | ID của version (tham chiếu FormVersions.Id) |
| **FieldCode** | `NVARCHAR(50)` | NOT NULL | Mã field (VD: HO_TEN, TUOI, LOAI_MAU) |
| **FieldType** | `INT` | NOT NULL | Loại field: 1=Text, 2=Number, 3=Date, 6=Select, 10=TextArea |
| **Label** | `NVARCHAR(200)` | NOT NULL | Nhãn hiển thị của field |
| **DisplayOrder** | `INT` | NOT NULL | Thứ tự hiển thị (số nhỏ = hiển thị trước) |
| **IsRequired** | `BIT` | NOT NULL, DEFAULT 0 | Field có bắt buộc nhập không |
| **IsVisible** | `BIT` | NOT NULL, DEFAULT 1 | Field có hiển thị không |
| **DefaultValue** | `NVARCHAR(500)` | NULL | Giá trị mặc định |
| **Placeholder** | `NVARCHAR(200)` | NULL | Placeholder text hiển thị trong input |
| **HelpText** | `NVARCHAR(500)` | NULL | Text hướng dẫn cho người dùng |
| **CssClass** | `NVARCHAR(200)` | NULL | CSS class để tùy chỉnh style |
| **PropertiesJson** | `NVARCHAR(MAX)` | NULL | JSON chứa các thuộc tính động (VD: {"colSpan": 6}) |
| **ParentFieldId** | `INT` | NULL, FOREIGN KEY | ID của field cha (tham chiếu FormFields.Id) - dùng cho nested/repeat sections |
| **MinOccurs** | `INT` | NULL | Số lần xuất hiện tối thiểu (cho repeat section) |
| **MaxOccurs** | `INT` | NULL | Số lần xuất hiện tối đa (cho repeat section, nếu > 1 thì là repeat) |
| **SectionCode** | `NVARCHAR(50)` | NULL | Mã section để nhóm các field (cho repeat section) |

### 3.3. Indexes

- `IX_FormFields_PublicId` (UNIQUE): Index cho PublicId
- `IX_FormFields_FormVersionId_FieldCode` (UNIQUE): Composite unique index đảm bảo FieldCode unique trong một version
- `IX_FormFields_FormVersionId_DisplayOrder`: Composite index để query và sort theo DisplayOrder
- `IX_FormFields_ParentFieldId`: Index cho ParentFieldId để query nested fields

### 3.4. Quan hệ

- **Many-to-One với FormVersions**: Nhiều field thuộc về một version
  - Foreign Key: `FormFields.FormVersionId` → `FormVersions.Id`
  - Delete Behavior: `Cascade` (xóa Version → xóa tất cả Fields)
  
- **One-to-Many với FieldValidations**: Một field có nhiều validation rule
  - Foreign Key: `FieldValidations.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Validations)
  
- **One-to-Many với FieldConditions**: Một field có nhiều condition
  - Foreign Key: `FieldConditions.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Conditions)
  
- **One-to-Many với FieldOptions**: Một field có nhiều option (cho Select field)
  - Foreign Key: `FieldOptions.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Options)
  
- **One-to-Many với FormDataValues**: Một field có nhiều giá trị đã submit
  - Foreign Key: `FormDataValues.FormFieldId` → `FormFields.Id`
  - Delete Behavior: `Restrict` (không cho xóa Field nếu còn Data)
  
- **Self-referencing (One-to-Many)**: Một field có thể có nhiều field con (nested fields)
  - Foreign Key: `FormFields.ParentFieldId` → `FormFields.Id`
  - Delete Behavior: `NoAction` (để tránh cascade path conflict)
  - **Lưu ý**: Quan hệ này là optional (có thể NULL)

---

## 4. BẢNG FIELDVALIDATIONS

### 4.1. Mô tả

Bảng chứa các quy tắc validation cho field. Một field có thể có nhiều validation rule, mỗi rule có priority để xác định thứ tự kiểm tra.

### 4.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **FieldId** | `INT` | NOT NULL, FOREIGN KEY | ID của field (tham chiếu FormFields.Id) |
| **RuleType** | `INT` | NOT NULL | Loại rule: 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex, 6=Email, etc. |
| **RuleValue** | `NVARCHAR(500)` | NULL | Giá trị rule (VD: "18" cho Min, "100" cho Max, regex pattern) |
| **ErrorMessage** | `NVARCHAR(500)` | NOT NULL | Thông báo lỗi hiển thị khi validation fail |
| **Priority** | `INT` | NOT NULL, DEFAULT 0 | Độ ưu tiên (số nhỏ = ưu tiên cao, kiểm tra trước) |
| **IsActive** | `BIT` | NOT NULL, DEFAULT 1 | Rule có active không (có thể tạm tắt) |

### 4.3. Indexes

- `IX_FieldValidations_PublicId` (UNIQUE): Index cho PublicId
- `IX_FieldValidations_FieldId`: Index cho FieldId để query nhanh

### 4.4. Quan hệ

- **Many-to-One với FormFields**: Nhiều validation rule thuộc về một field
  - Foreign Key: `FieldValidations.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Validations)

---

## 5. BẢNG FIELDCONDITIONS

### 5.1. Mô tả

Bảng chứa các điều kiện logic cho field (show/hide, enable/disable dựa trên giá trị field khác). Một field có thể có nhiều condition, mỗi condition có priority.

### 5.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **FieldId** | `INT` | NOT NULL, FOREIGN KEY | ID của field (tham chiếu FormFields.Id) |
| **ConditionType** | `INT` | NOT NULL | Loại condition: 1=Show, 2=Hide, 3=Enable, 4=Disable |
| **Expression** | `NVARCHAR(1000)` | NOT NULL | Biểu thức điều kiện (dạng JSON, VD: {"field": "TUOI", "operator": ">=", "value": 18}) |
| **ActionsJson** | `NVARCHAR(MAX)` | NULL | JSON chứa các action khi condition thỏa mãn |
| **Priority** | `INT` | NOT NULL, DEFAULT 0 | Độ ưu tiên (số nhỏ = ưu tiên cao) |

### 5.3. Indexes

- `IX_FieldConditions_PublicId` (UNIQUE): Index cho PublicId
- `IX_FieldConditions_FieldId`: Index cho FieldId để query nhanh

### 5.4. Quan hệ

- **Many-to-One với FormFields**: Nhiều condition thuộc về một field
  - Foreign Key: `FieldConditions.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Conditions)

---

## 6. BẢNG FIELDOPTIONS

### 6.1. Mô tả

Bảng chứa các option cho Select field (dropdown, radio, checkbox). Một Select field có thể có nhiều option.

### 6.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **FieldId** | `INT` | NOT NULL, FOREIGN KEY | ID của field (tham chiếu FormFields.Id) |
| **Value** | `NVARCHAR(100)` | NOT NULL | Giá trị option (value khi submit) |
| **Label** | `NVARCHAR(200)` | NOT NULL | Nhãn hiển thị của option |
| **DisplayOrder** | `INT` | NOT NULL | Thứ tự hiển thị (số nhỏ = hiển thị trước) |
| **IsDefault** | `BIT` | NOT NULL, DEFAULT 0 | Option có phải mặc định không (chỉ 1 option có thể là default) |

### 6.3. Indexes

- `IX_FieldOptions_PublicId` (UNIQUE): Index cho PublicId
- `IX_FieldOptions_FieldId_DisplayOrder`: Composite index để query và sort options theo DisplayOrder

### 6.4. Quan hệ

- **Many-to-One với FormFields**: Nhiều option thuộc về một field
  - Foreign Key: `FieldOptions.FieldId` → `FormFields.Id`
  - Delete Behavior: `Cascade` (xóa Field → xóa tất cả Options)

---

## 7. BẢNG FORMDATAVALUES

### 7.1. Mô tả

Bảng chứa dữ liệu đã submit của form. Mỗi field value là một bản ghi riêng. Các FormDataValue có cùng SubmissionId thuộc về cùng một lần submit form.

### 7.2. Các trường

| Tên trường | Kiểu dữ liệu | Ràng buộc | Mô tả |
|------------|--------------|-----------|-------|
| **Id** | `INT` | PRIMARY KEY, IDENTITY(1,1) | Khóa chính nội bộ |
| **PublicId** | `UNIQUEIDENTIFIER` | NOT NULL, UNIQUE, INDEXED | Khóa công khai (dùng cho API) |
| **SubmissionId** | `INT` | NOT NULL | **Tự quản lý, KHÔNG có FK constraint**. Dùng để nhóm các values của cùng 1 submission |
| **FormVersionId** | `INT` | NOT NULL, FOREIGN KEY | ID của version (tham chiếu FormVersions.Id) |
| **FormFieldId** | `INT` | NOT NULL, FOREIGN KEY | ID của field (tham chiếu FormFields.Id) |
| **ObjectId** | `NVARCHAR(100)` | NOT NULL | ID của đối tượng liên quan (VD: DangKyId, BenhNhanId, KhamBenhId) |
| **ObjectType** | `NVARCHAR(50)` | NOT NULL | Loại đối tượng (VD: PHIEU_KHAM, DANG_KY_KHAM, BENH_NHAN) |
| **FieldValue** | `NVARCHAR(4000)` | NULL | Giá trị của field (lưu dạng string, có thể parse theo FieldType) |
| **DisplayOrder** | `INT` | NOT NULL, DEFAULT 0 | Thứ tự hiển thị (dùng cho repeat section - MaxOccurs > 1) |
| **SectionCode** | `NVARCHAR(50)` | NULL | Mã section (dùng cho repeat section) |
| **Status** | `INT` | NOT NULL, DEFAULT 0 | Trạng thái: 0=Draft, 1=Submitted, 2=Approved |
| **CreatedDate** | `DATETIME2` | NOT NULL, DEFAULT GETUTCDATE() | Ngày giờ tạo |
| **CreatedBy** | `NVARCHAR(100)` | NOT NULL, DEFAULT '' | Người tạo |
| **ModifiedDate** | `DATETIME2` | NULL | Ngày giờ sửa đổi lần cuối |
| **ModifiedBy** | `NVARCHAR(100)` | NULL | Người sửa đổi lần cuối |

### 7.3. Indexes

- `IX_FormDataValues_PublicId` (UNIQUE): Index cho PublicId
- `IX_FormDataValues_SubmissionId`: Index để group các values của cùng submission
- `IX_FormDataValues_ObjectId_ObjectType_FormVersionId`: Composite index để query theo object
- `IX_FormDataValues_FormVersionId`: Index cho FormVersionId
- `IX_FormDataValues_FormFieldId`: Index cho FormFieldId
- `IX_FormDataValues_SubmissionId_FormFieldId_DisplayOrder`: Composite index để query và order
- `IX_FormDataValues_CreatedDate`: Index để sort theo ngày tạo
- `IX_FormDataValues_Status`: Index để filter theo status

### 7.4. Quan hệ

- **Many-to-One với FormVersions**: Nhiều data value thuộc về một version
  - Foreign Key: `FormDataValues.FormVersionId` → `FormVersions.Id`
  - Delete Behavior: `Restrict` (không cho xóa Version nếu còn Data)
  
- **Many-to-One với FormFields**: Nhiều data value thuộc về một field
  - Foreign Key: `FormDataValues.FormFieldId` → `FormFields.Id`
  - Delete Behavior: `Restrict` (không cho xóa Field nếu còn Data)

### 7.5. Lưu ý đặc biệt

- **SubmissionId**: Không có Foreign Key constraint. Đây là INT tự quản lý, dùng để nhóm các FormDataValue của cùng một lần submit form. Hệ thống tự động tăng SubmissionId khi tạo submission mới.
- **FieldValue**: Lưu dạng string (NVARCHAR), có thể parse theo FieldType khi cần (VD: Number field lưu "30", Date field lưu "2024-01-21")

---

## TÓM TẮT QUAN HỆ

### Quan hệ One-to-Many

1. **Forms** (1) → **FormVersions** (N) - qua `FormVersions.FormId`
2. **FormVersions** (1) → **FormFields** (N) - qua `FormFields.FormVersionId`
3. **FormVersions** (1) → **FormDataValues** (N) - qua `FormDataValues.FormVersionId`
4. **FormFields** (1) → **FieldValidations** (N) - qua `FieldValidations.FieldId`
5. **FormFields** (1) → **FieldConditions** (N) - qua `FieldConditions.FieldId`
6. **FormFields** (1) → **FieldOptions** (N) - qua `FieldOptions.FieldId`
7. **FormFields** (1) → **FormDataValues** (N) - qua `FormDataValues.FormFieldId`
8. **FormFields** (1) → **FormFields** (N) - qua `FormFields.ParentFieldId` (Self-referencing)

### Quan hệ One-to-One (Optional)

1. **Forms** (1) → **FormVersions** (1) - qua `Forms.CurrentVersionId` (nullable)

### Delete Behaviors

- **Cascade**: Xóa parent → tự động xóa children
  - FormVersions → FormFields
  - FormFields → FieldValidations, FieldConditions, FieldOptions

- **Restrict**: Không cho xóa parent nếu còn children
  - Forms → FormVersions
  - FormVersions → FormDataValues
  - FormFields → FormDataValues

- **SetNull**: Xóa parent → set foreign key về NULL
  - FormVersions → Forms.CurrentVersionId

- **NoAction**: Không có action tự động (dùng cho self-referencing để tránh cascade path conflict)
  - FormFields → FormFields (ParentFieldId)

---

## QUY ƯỚC ĐẶT TÊN

### Primary Key
- Tất cả bảng đều có `Id` (INT, IDENTITY) làm Primary Key

### Public ID
- Tất cả bảng đều có `PublicId` (GUID, UNIQUE, INDEXED) để expose ra API

### Foreign Key
- Tên foreign key: `{TableName}Id` (VD: `FormId`, `FormVersionId`, `FieldId`)
- Kiểu dữ liệu: `INT` (tham chiếu đến `Id` của bảng cha)

### Audit Fields
- `CreatedDate`, `CreatedBy`: Bắt buộc, tự động set khi tạo
- `ModifiedDate`, `ModifiedBy`: Optional, set khi cập nhật

### Status Fields
- Dùng `INT` thay vì `BIT` để có thể mở rộng thêm trạng thái sau này
- Giá trị: 0, 1, 2, ... (tùy theo từng bảng)

---

## VÍ DỤ DỮ LIỆU

### Forms
```
Id: 1
PublicId: 82b82eb9-468e-4822-8aa7-8fe408a79a50
Code: PHIEU_DANG_KY_KHAM_BENH
Name: Phiếu đăng ký khám bệnh
Description: Phiếu đăng ký khám bệnh cho bệnh nhân
Status: 1 (Active)
CurrentVersionId: 2
CreatedDate: 2024-01-15 10:00:00
CreatedBy: admin
```

### FormVersions
```
Id: 2
PublicId: c7ee902f-0761-408c-b57e-797f97558529
FormId: 1
Version: "2"
Status: 1 (Published)
IsActive: true
CreatedDate: 2024-01-20 10:00:00
CreatedBy: admin
PublishedDate: 2024-01-20 11:00:00
PublishedBy: admin
ChangeLog: Create version 2
```

### FormFields
```
Id: 4
PublicId: a1b2c3d4-e5f6-7890-abcd-ef1234567890
FormVersionId: 2
FieldCode: HO_TEN
FieldType: 1 (Text)
Label: Họ và tên
DisplayOrder: 1
IsRequired: true
IsVisible: true
Placeholder: Vui lòng nhập họ và tên
ParentFieldId: NULL
```

### FormDataValues
```
Id: 1
PublicId: 0a763232-xxxx-xxxx-xxxx-xxxxxxxxxxxx
SubmissionId: 1
FormVersionId: 2
FormFieldId: 4
ObjectId: DK001
ObjectType: PHIEU_DANG_KY_KHAM_BENH
FieldValue: "Nguyễn Văn A"
DisplayOrder: 0
Status: 1 (Submitted)
CreatedDate: 2024-01-21 10:00:00
CreatedBy: admin
```

---

**Tài liệu này mô tả đầy đủ cấu trúc database của hệ thống DynamicForm. Cập nhật lần cuối: 2024-01-21**
