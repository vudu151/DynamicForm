# TÀI LIỆU API - DYNAMIC FORM

> **Mục tiêu**: Mô tả chi tiết các API endpoints của hệ thống DynamicForm
>
> **Base URL**: `http://localhost:5144/api`
>
> **Format**: JSON
>
> **Authentication**: (Chưa implement - sẽ bổ sung sau)

---

## 1. API QUẢN LÝ FORM

### 1.1. API lấy danh sách tất cả Form

**Mô tả**: API được sử dụng để lấy danh sách tất cả các form trong hệ thống.

**Method**: `GET`

**URL**: `/api/forms`

**Input Parameters**: Không có

**Output Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của form (dùng cho API) |
| Code | `string` | Mã định danh duy nhất của form (VD: PHIEU_KHAM_BENH) |
| Name | `string` | Tên hiển thị của form |
| Description | `string?` | Mô tả form (có thể null) |
| Status | `int` | Trạng thái: 0=Draft, 1=Active, 2=Inactive |
| CurrentVersionId | `Guid?` | PublicId của version đang active (có thể null) |
| CreatedDate | `DateTime` | Ngày tạo form |
| CreatedBy | `string` | Người tạo form |

**Ví dụ Response**:
```json
[
  {
    "id": "82b82eb9-468e-4822-8aa7-8fe408a79a50",
    "code": "PHIEU_DANG_KY_KHAM_BENH",
    "name": "Phiếu đăng ký khám bệnh",
    "description": "Phiếu đăng ký khám bệnh cho bệnh nhân",
    "status": 1,
    "currentVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
    "createdDate": "2024-01-15T10:00:00Z",
    "createdBy": "admin"
  }
]
```

---

### 1.2. API lấy Form theo ID

**Mô tả**: API được sử dụng để lấy thông tin chi tiết của một form theo PublicId.

**Method**: `GET`

**URL**: `/api/forms/{id}`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| id | `Guid` | PublicId của form (trong URL path) |

**Output Parameters**: Giống như 1.1 (trả về 1 object FormDto)

**Ví dụ Response**:
```json
{
  "id": "82b82eb9-468e-4822-8aa7-8fe408a79a50",
  "code": "PHIEU_DANG_KY_KHAM_BENH",
  "name": "Phiếu đăng ký khám bệnh",
  "description": "Phiếu đăng ký khám bệnh cho bệnh nhân",
  "status": 1,
  "currentVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
  "createdDate": "2024-01-15T10:00:00Z",
  "createdBy": "admin"
}
```

---

### 1.3. API lấy Form theo Code

**Mô tả**: API được sử dụng để lấy thông tin form theo mã code (thường dùng để lấy form active).

**Method**: `GET`

**URL**: `/api/forms/code/{code}`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| code | `string` | Mã code của form (trong URL path, VD: PHIEU_DANG_KY_KHAM_BENH) |

**Output Parameters**: Giống như 1.1 (trả về 1 object FormDto)

---

### 1.4. API tạo Form mới

**Mô tả**: API được sử dụng để tạo một form mới trong hệ thống.

**Method**: `POST`

**URL**: `/api/forms`

**Input Parameters** (Request Body):

| Field | Type | Description |
|-------|------|-------------|
| Code | `string` | Mã định danh duy nhất của form (bắt buộc, không được trùng) |
| Name | `string` | Tên hiển thị của form (bắt buộc) |
| Description | `string?` | Mô tả form (tùy chọn) |
| Status | `int` | Trạng thái: 0=Draft, 1=Active, 2=Inactive (mặc định: 0) |
| CreatedBy | `string` | Người tạo form (bắt buộc) |

**Output Parameters**: Giống như 1.1 (trả về FormDto đã tạo)

**Ví dụ Request**:
```json
{
  "code": "PHIEU_KHAM_BENH",
  "name": "Phiếu khám bệnh",
  "description": "Phiếu khám bệnh tổng quát",
  "status": 0,
  "createdBy": "admin"
}
```

**Lỗi có thể xảy ra**:
- `400 Bad Request`: Form code already exists
- `400 Bad Request`: Form code is required

---

### 1.5. API ngưng kích hoạt Form

**Mô tả**: API được sử dụng để ngưng kích hoạt một form (chuyển status sang Inactive).

**Method**: `POST`

**URL**: `/api/forms/{formId}/deactivate`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| formId | `Guid` | PublicId của form (trong URL path) |

**Output Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| message | `string` | Thông báo kết quả: "Form deactivated successfully" |

---

## 2. API QUẢN LÝ VERSION

### 2.1. API lấy danh sách Version của Form

**Mô tả**: API được sử dụng để lấy danh sách tất cả các version của một form.

**Method**: `GET`

**URL**: `/api/forms/{formId}/versions`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| formId | `Guid` | PublicId của form (trong URL path) |

**Output Parameters** (Array of FormVersionDto):

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của version |
| FormId | `Guid` | PublicId của form |
| Version | `string` | Số version (VD: "1", "1.0.0", "2") |
| IsActive | `bool` | Version có đang active không |
| CreatedDate | `DateTime` | Ngày tạo version |
| CreatedBy | `string` | Người tạo version |
| ApprovedDate | `DateTime?` | Ngày duyệt (có thể null) |
| ApprovedBy | `string?` | Người duyệt (có thể null) |
| ChangeLog | `string?` | Mô tả thay đổi của version (có thể null) |

**Ví dụ Response**:
```json
[
  {
    "id": "c7ee902f-0761-408c-b57e-797f97558529",
    "formId": "82b82eb9-468e-4822-8aa7-8fe408a79a50",
    "version": "2",
    "isActive": true,
    "createdDate": "2024-01-20T10:00:00Z",
    "createdBy": "admin",
    "approvedDate": "2024-01-20T11:00:00Z",
    "approvedBy": "admin",
    "changeLog": "Create version 2"
  }
]
```

---

### 2.2. API tạo Version mới

**Mô tả**: API được sử dụng để tạo một version mới cho form. Nếu version đã tồn tại, hệ thống sẽ tự động tăng version lên.

**Method**: `POST`

**URL**: `/api/forms/{formId}/versions`

**Input Parameters** (Request Body):

| Field | Type | Description |
|-------|------|-------------|
| FormId | `Guid` | PublicId của form (phải khớp với formId trong URL) |
| Version | `string` | Số version (VD: "1", "1.0.0"). Nếu trùng, hệ thống tự động tăng |
| IsActive | `bool` | Version có active ngay không (mặc định: false) |
| CreatedBy | `string` | Người tạo version (bắt buộc) |
| ChangeLog | `string?` | Mô tả thay đổi (tùy chọn) |

**Output Parameters**: Giống như 2.1 (trả về FormVersionDto đã tạo, version có thể đã được tự động tăng)

**Ví dụ Request**:
```json
{
  "formId": "82b82eb9-468e-4822-8aa7-8fe408a79a50",
  "version": "1",
  "isActive": false,
  "createdBy": "admin",
  "changeLog": "Create version 1"
}
```

**Lưu ý**: 
- Nếu version "1" đã tồn tại, hệ thống sẽ tự động tạo version "2" (hoặc số tiếp theo)
- Nếu version "1.0.0" đã tồn tại, hệ thống sẽ tự động tạo version "1.0.1"

---

### 2.3. API kích hoạt Version

**Mô tả**: API được sử dụng để kích hoạt một version, làm cho nó trở thành version active của form.

**Method**: `POST`

**URL**: `/api/forms/versions/{versionId}/activate`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| versionId | `Guid` | PublicId của version (trong URL path) |

**Output Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| message | `string` | Thông báo kết quả: "Version activated successfully" |

**Lưu ý**: Khi kích hoạt một version, các version khác của cùng form sẽ tự động bị deactivate.

---

## 3. API QUẢN LÝ METADATA

### 3.1. API lấy Metadata theo Form Code

**Mô tả**: API được sử dụng để lấy metadata (cấu trúc form) của form theo code. Trả về version active hoặc version mới nhất.

**Method**: `GET`

**URL**: `/api/forms/code/{code}/metadata`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| code | `string` | Mã code của form (trong URL path) |

**Output Parameters** (FormMetadataDto):

| Field | Type | Description |
|-------|------|-------------|
| Form | `FormDto` | Thông tin form (giống 1.1) |
| Version | `FormVersionDto` | Thông tin version (giống 2.1) |
| Fields | `List<FormFieldDto>` | Danh sách các field của form |

**FormFieldDto**:

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của field |
| FormVersionId | `Guid` | PublicId của version |
| FieldCode | `string` | Mã field (VD: HO_TEN, TUOI) |
| FieldType | `int` | Loại field: 1=Text, 2=Number, 3=Date, 6=Select, 10=TextArea |
| Label | `string` | Nhãn hiển thị |
| DisplayOrder | `int` | Thứ tự hiển thị |
| IsRequired | `bool` | Field có bắt buộc không |
| IsVisible | `bool` | Field có hiển thị không |
| DefaultValue | `string?` | Giá trị mặc định |
| Placeholder | `string?` | Placeholder text |
| HelpText | `string?` | Text hướng dẫn |
| PropertiesJson | `string?` | JSON chứa các thuộc tính động (VD: {"colSpan": 6}) |
| Validations | `List<FieldValidationDto>` | Danh sách validation rules |
| Conditions | `List<FieldConditionDto>` | Danh sách điều kiện hiển thị |
| Options | `List<FieldOptionDto>` | Danh sách options (cho Select) |

**FieldValidationDto**:

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của validation |
| FieldId | `Guid` | PublicId của field |
| RuleType | `int` | Loại rule: 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex |
| RuleValue | `string?` | Giá trị rule (VD: "18" cho Min, "100" cho Max) |
| ErrorMessage | `string` | Thông báo lỗi |
| Priority | `int` | Độ ưu tiên (số nhỏ = ưu tiên cao) |
| IsActive | `bool` | Rule có active không |

**FieldOptionDto** (cho Select field):

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của option |
| FieldId | `Guid` | PublicId của field |
| Value | `string` | Giá trị option |
| Label | `string` | Nhãn hiển thị |
| DisplayOrder | `int` | Thứ tự hiển thị |
| IsDefault | `bool` | Option có phải mặc định không |

---

### 3.2. API lấy Metadata theo Version ID

**Mô tả**: API được sử dụng để lấy metadata của một version cụ thể.

**Method**: `GET`

**URL**: `/api/forms/versions/{versionId}/metadata`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| versionId | `Guid` | PublicId của version (trong URL path) |

**Output Parameters**: Giống như 3.1 (FormMetadataDto)

---

### 3.3. API cập nhật Metadata

**Mô tả**: API được sử dụng để cập nhật metadata (các field) của một version.

**Method**: `PUT`

**URL**: `/api/forms/versions/{versionId}/metadata`

**Input Parameters** (Request Body):

| Field | Type | Description |
|-------|------|-------------|
| ChangeLog | `string?` | Mô tả thay đổi (tùy chọn) |
| Fields | `List<FormFieldDto>` | Danh sách fields mới (xem 3.1 cho cấu trúc FormFieldDto) |

**Output Parameters**: Giống như 3.1 (FormMetadataDto đã cập nhật)

**Ví dụ Request**:
```json
{
  "changeLog": "Thêm field mới: Số điện thoại",
  "fields": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "formVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
      "fieldCode": "SO_DIEN_THOAI",
      "fieldType": 1,
      "label": "Số điện thoại",
      "displayOrder": 1,
      "isRequired": false,
      "isVisible": true,
      "placeholder": "Nhập số điện thoại",
      "validations": [],
      "conditions": [],
      "options": []
    }
  ]
}
```

**Lưu ý**: 
- Field có `Id = Guid.Empty` sẽ được tạo mới
- Field có `Id` đã tồn tại sẽ được cập nhật
- Field không có trong danh sách sẽ bị xóa

---

## 4. API QUẢN LÝ DỮ LIỆU FORM

### 4.1. API tạo dữ liệu Form

**Mô tả**: API được sử dụng để tạo một submission mới (lưu dữ liệu đã điền vào form).

**Method**: `POST`

**URL**: `/api/formdata`

**Input Parameters** (Request Body):

| Field | Type | Description |
|-------|------|-------------|
| FormVersionId | `Guid` | PublicId của version form (bắt buộc) |
| ObjectId | `string` | ID của đối tượng liên quan (VD: DangKyId, BenhNhanId) |
| ObjectType | `string` | Loại đối tượng (VD: PHIEU_KHAM, DANG_KY_KHAM) |
| Data | `Dictionary<string, object>` | Dữ liệu form, key là FieldCode, value là giá trị |

**Output Parameters** (FormDataDto):

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | PublicId của một FormDataValue record |
| FormVersionId | `Guid` | PublicId của version |
| ObjectId | `string` | ID đối tượng |
| ObjectType | `string` | Loại đối tượng |
| Data | `Dictionary<string, object>` | Dữ liệu form (key = FieldCode) |
| CreatedDate | `DateTime` | Ngày tạo |
| CreatedBy | `string` | Người tạo |
| ModifiedDate | `DateTime?` | Ngày sửa (có thể null) |
| ModifiedBy | `string?` | Người sửa (có thể null) |
| Status | `int` | Trạng thái: 0=Draft, 1=Submitted, 2=Approved |

**Ví dụ Request**:
```json
{
  "formVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
  "objectId": "DK001",
  "objectType": "PHIEU_DANG_KY_KHAM_BENH",
  "data": {
    "HO_TEN": "Nguyễn Văn A",
    "TUOI": 30,
    "LOAI_MAU": "A"
  }
}
```

**Lỗi có thể xảy ra**:
- `400 Bad Request`: Validation failed (xem 4.4)
- `400 Bad Request`: Form version not found

---

### 4.2. API lấy dữ liệu Form theo Submission ID

**Mô tả**: API được sử dụng để lấy dữ liệu đã lưu theo SubmissionId.

**Method**: `GET`

**URL**: `/api/formdata/{submissionId}`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| submissionId | `int` | SubmissionId (trong URL path, không phải PublicId) |

**Output Parameters**: Giống như 4.1 (FormDataDto)

---

### 4.3. API lấy dữ liệu Form theo Object

**Mô tả**: API được sử dụng để lấy dữ liệu form mới nhất của một đối tượng cụ thể.

**Method**: `GET`

**URL**: `/api/formdata/object/{objectId}/{objectType}/{formVersionPublicId}`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| objectId | `string` | ID đối tượng (trong URL path) |
| objectType | `string` | Loại đối tượng (trong URL path) |
| formVersionPublicId | `Guid` | PublicId của version (trong URL path) |

**Output Parameters**: Giống như 4.1 (FormDataDto)

---

### 4.4. API cập nhật dữ liệu Form

**Mô tả**: API được sử dụng để cập nhật dữ liệu đã lưu của một submission.

**Method**: `PUT`

**URL**: `/api/formdata/{submissionId}`

**Input Parameters**:

| Field | Type | Description |
|-------|------|-------------|
| submissionId | `int` | SubmissionId (trong URL path) |

**Request Body**: Giống như 4.1 (CreateFormDataRequest)

**Output Parameters**: Giống như 4.1 (FormDataDto đã cập nhật)

**Lỗi có thể xảy ra**:
- `404 Not Found`: Submission not found
- `400 Bad Request`: Validation failed

---

### 4.5. API validate dữ liệu Form

**Mô tả**: API được sử dụng để validate dữ liệu form trước khi lưu (không lưu vào database).

**Method**: `POST`

**URL**: `/api/formdata/validate`

**Input Parameters** (Request Body):

| Field | Type | Description |
|-------|------|-------------|
| FormVersionId | `Guid` | PublicId của version form (bắt buộc) |
| Data | `Dictionary<string, object>` | Dữ liệu cần validate (key = FieldCode) |

**Output Parameters** (ValidationResultDto):

| Field | Type | Description |
|-------|------|-------------|
| IsValid | `bool` | Dữ liệu có hợp lệ không |
| Errors | `List<ValidationErrorDto>` | Danh sách lỗi (nếu có) |

**ValidationErrorDto**:

| Field | Type | Description |
|-------|------|-------------|
| FieldCode | `string` | Mã field bị lỗi |
| Message | `string` | Thông báo lỗi |

**Ví dụ Request**:
```json
{
  "formVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
  "data": {
    "HO_TEN": "",
    "TUOI": 15
  }
}
```

**Ví dụ Response** (có lỗi):
```json
{
  "isValid": false,
  "errors": [
    {
      "fieldCode": "HO_TEN",
      "message": "Trường này là bắt buộc"
    },
    {
      "fieldCode": "TUOI",
      "message": "Tuổi phải lớn hơn hoặc bằng 18"
    }
  ]
}
```

---

### 4.6. API lấy danh sách dữ liệu Form

**Mô tả**: API được sử dụng để lấy danh sách các submission đã lưu, có thể filter theo form version, object type, object id.

**Method**: `GET`

**URL**: `/api/formdata/list`

**Input Parameters** (Query String):

| Field | Type | Description |
|-------|------|-------------|
| formVersionId | `Guid?` | PublicId của version (tùy chọn, filter theo version) |
| objectType | `string?` | Loại đối tượng (tùy chọn, filter theo object type) |
| objectId | `string?` | ID đối tượng (tùy chọn, filter theo object id) |

**Output Parameters** (Array of FormDataListItemDto):

| Field | Type | Description |
|-------|------|-------------|
| SubmissionId | `int` | SubmissionId (dùng để lấy chi tiết) |
| FormVersionId | `Guid` | PublicId của version |
| FormVersionName | `string` | Tên version (VD: "2") |
| FormName | `string` | Tên form |
| FormCode | `string` | Mã form |
| ObjectId | `string` | ID đối tượng |
| ObjectType | `string` | Loại đối tượng |
| CreatedDate | `DateTime` | Ngày tạo |
| CreatedBy | `string` | Người tạo |
| ModifiedDate | `DateTime?` | Ngày sửa (có thể null) |
| ModifiedBy | `string?` | Người sửa (có thể null) |
| Status | `int` | Trạng thái: 0=Draft, 1=Submitted, 2=Approved |
| FieldCount | `int` | Số lượng field trong submission |

**Ví dụ Request**:
```
GET /api/formdata/list?formVersionId=c7ee902f-0761-408c-b57e-797f97558529&objectType=PHIEU_KHAM
```

**Ví dụ Response**:
```json
[
  {
    "submissionId": 1,
    "formVersionId": "c7ee902f-0761-408c-b57e-797f97558529",
    "formVersionName": "2",
    "formName": "Phiếu đăng ký khám bệnh",
    "formCode": "PHIEU_DANG_KY_KHAM_BENH",
    "objectId": "DK001",
    "objectType": "PHIEU_DANG_KY_KHAM_BENH",
    "createdDate": "2024-01-21T10:00:00Z",
    "createdBy": "admin",
    "modifiedDate": null,
    "modifiedBy": null,
    "status": 1,
    "fieldCount": 3
  }
]
```

---

## 5. MÃ LỖI VÀ XỬ LÝ

### 5.1. HTTP Status Codes

| Status Code | Mô tả |
|-------------|-------|
| 200 OK | Request thành công |
| 201 Created | Đã tạo thành công (POST) |
| 400 Bad Request | Request không hợp lệ (validation error, duplicate, etc.) |
| 404 Not Found | Không tìm thấy resource |
| 500 Internal Server Error | Lỗi server |

### 5.2. Format Error Response

Tất cả lỗi trả về theo format:

```json
{
  "error": "Mô tả lỗi",
  "detail": "Chi tiết lỗi (nếu có)"
}
```

**Ví dụ**:
```json
{
  "error": "Version already exists: 1"
}
```

hoặc

```json
{
  "error": "Form code already exists: PHIEU_KHAM",
  "detail": "Cannot insert duplicate key..."
}
```

---

## 6. GHI CHÚ QUAN TRỌNG

### 6.1. PublicId vs Internal ID

- **PublicId** (`Guid`): Dùng trong API, exposed ra ngoài
- **Internal ID** (`int`): Dùng trong database, không expose ra API
- Tất cả API endpoints sử dụng **PublicId** (Guid) cho form, version, field
- Chỉ **SubmissionId** sử dụng `int` (không có PublicId)

### 6.2. Version Auto-increment

Khi tạo version mới:
- Nếu version đã tồn tại, hệ thống tự động tăng version
- Hỗ trợ số đơn giản (1, 2, 3...) và semantic version (1.0.0, 1.0.1...)

### 6.3. Form Code và Version

- **Form Code**: Phải unique trong toàn hệ thống
- **Version**: Phải unique trong một form (nhưng có thể trùng giữa các form khác nhau)

### 6.4. Metadata Update

Khi cập nhật metadata:
- Field mới: `Id = Guid.Empty` hoặc `00000000-0000-0000-0000-000000000000`
- Field cũ: Giữ nguyên `Id` (PublicId)
- Field không có trong danh sách: Sẽ bị xóa

---

## 7. VÍ DỤ LUỒNG SỬ DỤNG

### 7.1. Tạo Form và Version mới

```
1. POST /api/forms
   → Tạo form mới

2. POST /api/forms/{formId}/versions
   → Tạo version "1" cho form

3. PUT /api/forms/versions/{versionId}/metadata
   → Thiết kế các field

4. POST /api/forms/versions/{versionId}/activate
   → Kích hoạt version để sử dụng
```

### 7.2. Điền và lưu dữ liệu Form

```
1. GET /api/forms/code/{code}/metadata
   → Lấy metadata để render form

2. POST /api/formdata/validate
   → Validate dữ liệu trước khi lưu (optional)

3. POST /api/formdata
   → Lưu dữ liệu đã điền
```

### 7.3. Xem và sửa dữ liệu đã lưu

```
1. GET /api/formdata/list?formVersionId={versionId}
   → Lấy danh sách submissions

2. GET /api/formdata/{submissionId}
   → Lấy chi tiết một submission

3. PUT /api/formdata/{submissionId}
   → Cập nhật dữ liệu
```

---

**Tài liệu này được tạo tự động từ codebase. Cập nhật lần cuối: 2024-01-21**
