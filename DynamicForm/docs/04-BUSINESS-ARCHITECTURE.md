# SƠ ĐỒ KIẾN TRÚC NGHIỆP VỤ (BUSINESS ARCHITECTURE) - DYNAMIC FORM SYSTEM

## I. TỔNG QUAN NGHIỆP VỤ

### 1.1 Sơ đồ Business Architecture tổng thể

```mermaid
graph TB
    subgraph "BUSINESS ACTORS"
        ADMIN[Quản trị viên<br/>- Tạo/sửa form<br/>- Quản lý version<br/>- Phân quyền]
        DOCTOR[Bác sĩ<br/>- Khám bệnh<br/>- Điền phiếu khám<br/>- Xem bệnh án]
        NURSE[Điều dưỡng<br/>- Chăm sóc<br/>- Điền phiếu chăm sóc<br/>- Theo dõi]
        LAB_TECH[Kỹ thuật viên<br/>- Xét nghiệm<br/>- Điền phiếu xét nghiệm]
        ADMIN_STAFF[Nhân viên hành chính<br/>- Quản lý hồ sơ<br/>- Báo cáo]
    end
    
    subgraph "BUSINESS PROCESSES"
        FORM_DESIGN[Thiết kế Form<br/>- Định nghĩa metadata<br/>- Cấu hình field<br/>- Thiết lập validation]
        FORM_VERSIONING[Quản lý Version<br/>- Tạo version mới<br/>- Duyệt version<br/>- Kích hoạt version]
        FORM_FILLING[Điền Form<br/>- Load form theo version<br/>- Nhập dữ liệu<br/>- Validate dữ liệu]
        FORM_REVIEW[Xem/In Form<br/>- Xem dữ liệu cũ<br/>- In báo cáo<br/>- Export dữ liệu]
    end
    
    subgraph "BUSINESS ENTITIES"
        FORM_ENTITY[Form<br/>- Phiếu khám<br/>- Phiếu chăm sóc<br/>- Bệnh án<br/>- Phiếu xét nghiệm]
        FIELD_ENTITY[Field<br/>- Text, Number<br/>- Date, Select<br/>- Checkbox, Repeater]
        VALIDATION_ENTITY[Validation Rule<br/>- Required<br/>- Range<br/>- Conditional]
        DATA_ENTITY[Form Data<br/>- Dữ liệu đã điền<br/>- Gắn với version<br/>- Audit trail]
    end
    
    subgraph "BUSINESS RULES"
        RULE_VERSION[Versioning Rule<br/>- Immutable version<br/>- Data gắn version<br/>- Không sửa version cũ]
        RULE_VALIDATION[Validation Rule<br/>- Client + Server<br/>- Conditional logic<br/>- Range sinh lý]
        RULE_PERMISSION[Permission Rule<br/>- Role-based access<br/>- Form-level permission<br/>- Field-level permission]
        RULE_AUDIT[Audit Rule<br/>- Log mọi thay đổi<br/>- Track user action<br/>- Compliance]
    end
    
    ADMIN --> FORM_DESIGN
    ADMIN --> FORM_VERSIONING
    
    DOCTOR --> FORM_FILLING
    DOCTOR --> FORM_REVIEW
    
    NURSE --> FORM_FILLING
    NURSE --> FORM_REVIEW
    
    LAB_TECH --> FORM_FILLING
    
    FORM_DESIGN --> FORM_ENTITY
    FORM_DESIGN --> FIELD_ENTITY
    FORM_DESIGN --> VALIDATION_ENTITY
    
    FORM_FILLING --> DATA_ENTITY
    FORM_REVIEW --> DATA_ENTITY
    
    FORM_VERSIONING --> RULE_VERSION
    FORM_FILLING --> RULE_VALIDATION
    FORM_FILLING --> RULE_PERMISSION
    FORM_DESIGN --> RULE_AUDIT
```

## II. LUỒNG NGHIỆP VỤ CHÍNH

### 2.1 Luồng Tạo và Quản lý Form

```mermaid
sequenceDiagram
    participant Admin
    participant FormDesigner
    participant ValidationEngine
    participant VersionManager
    participant Database
    
    Admin->>FormDesigner: Tạo form mới (PHIEU_KHAM)
    FormDesigner->>FormDesigner: Định nghĩa form metadata
    FormDesigner->>FormDesigner: Thêm các field
    FormDesigner->>ValidationEngine: Thiết lập validation rules
    ValidationEngine-->>FormDesigner: Xác nhận rules
    FormDesigner->>VersionManager: Tạo version 1.0
    VersionManager->>Database: Lưu form + version
    Database-->>VersionManager: Success
    VersionManager-->>Admin: Form đã tạo
    
    Note over Admin,Database: Form ở trạng thái Draft
    
    Admin->>VersionManager: Duyệt và kích hoạt version
    VersionManager->>Database: Set IsActive = true
    Database-->>VersionManager: Success
    VersionManager-->>Admin: Form đã active
```

### 2.2 Luồng Điền Form (Bác sĩ/Điều dưỡng)

```mermaid
sequenceDiagram
    participant Doctor
    participant FormRenderer
    participant ValidationEngine
    participant DataService
    participant Database
    
    Doctor->>FormRenderer: Mở form (code: PHIEU_KHAM)
    FormRenderer->>Database: Load form metadata (version active)
    Database-->>FormRenderer: Form + Fields + Validation
    FormRenderer-->>Doctor: Hiển thị form
    
    Doctor->>Doctor: Nhập dữ liệu
    Doctor->>FormRenderer: Submit form
    
    FormRenderer->>ValidationEngine: Validate client-side
    ValidationEngine-->>FormRenderer: Validation result
    
    alt Validation fail
        FormRenderer-->>Doctor: Hiển thị lỗi
    else Validation pass
        FormRenderer->>DataService: POST form data
        DataService->>ValidationEngine: Validate server-side
        ValidationEngine-->>DataService: Validation result
        
        alt Server validation fail
            DataService-->>FormRenderer: 400 Bad Request
            FormRenderer-->>Doctor: Hiển thị lỗi
        else Server validation pass
            DataService->>Database: Lưu FORM_DATA
            Database-->>DataService: Success
            DataService-->>FormRenderer: 201 Created
            FormRenderer-->>Doctor: Thành công
        end
    end
```

### 2.3 Luồng Tạo Version Mới

```mermaid
sequenceDiagram
    participant Admin
    participant VersionManager
    participant Database
    
    Admin->>VersionManager: Yêu cầu tạo version mới
    VersionManager->>Database: Load version hiện tại
    Database-->>VersionManager: Current version (v1.0)
    
    VersionManager->>VersionManager: Copy metadata version cũ
    VersionManager->>VersionManager: Tạo version mới (v2.0)
    
    Admin->>Admin: Chỉnh sửa fields/validation
    
    VersionManager->>Database: Lưu version mới
    Database-->>VersionManager: Success
    
    Note over Database: Version cũ (v1.0) vẫn giữ nguyên<br/>Data cũ gắn với v1.0
    
    Admin->>VersionManager: Kích hoạt version mới
    VersionManager->>Database: Set v2.0 IsActive = true<br/>Set v1.0 IsActive = false
    Database-->>VersionManager: Success
    VersionManager-->>Admin: Version mới đã active
```

### 2.4 Luồng Xem Dữ liệu Form Cũ

```mermaid
sequenceDiagram
    participant Doctor
    participant FormRenderer
    participant DataService
    participant Database
    
    Doctor->>DataService: Xem bệnh án (ObjectId: 12345)
    DataService->>Database: Load FORM_DATA
    Database-->>DataService: Data + FormVersionId
    
    DataService->>Database: Load FormVersion metadata
    Database-->>DataService: Version metadata (v1.0)
    
    DataService->>Database: Load Fields của version đó
    Database-->>DataService: Fields metadata
    
    DataService-->>FormRenderer: Data + Metadata (version cũ)
    FormRenderer->>FormRenderer: Render form theo version cũ
    FormRenderer-->>Doctor: Hiển thị form với dữ liệu cũ
    
    Note over Doctor,Database: Form được render theo đúng<br/>version khi dữ liệu được tạo
```

## III. BUSINESS DOMAIN MODEL

### 3.1 Domain Entities và Relationships

```mermaid
classDiagram
    class Form {
        +Guid Id
        +string Code
        +string Name
        +string Description
        +FormStatus Status
        +Guid? CurrentVersionId
        +DateTime CreatedDate
        +string CreatedBy
        +List~FormVersion~ Versions
        +List~FormPermission~ Permissions
    }
    
    class FormVersion {
        +Guid Id
        +Guid FormId
        +string Version
        +bool IsActive
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime? ApprovedDate
        +string ApprovedBy
        +Form Form
        +List~FormField~ Fields
        +List~FormData~ FormData
    }
    
    class FormField {
        +Guid Id
        +Guid FormVersionId
        +string FieldCode
        +FieldType FieldType
        +string Label
        +int DisplayOrder
        +bool IsRequired
        +bool IsVisible
        +string DefaultValue
        +string Placeholder
        +Dictionary~string,object~ Properties
        +FormVersion FormVersion
        +List~FieldValidation~ Validations
        +List~FieldCondition~ Conditions
    }
    
    class FieldValidation {
        +Guid Id
        +Guid FieldId
        +ValidationRuleType RuleType
        +string RuleValue
        +string ErrorMessage
        +int Priority
        +FormField Field
    }
    
    class FieldCondition {
        +Guid Id
        +Guid FieldId
        +ConditionType Type
        +string Expression
        +Dictionary~string,object~ Actions
        +FormField Field
    }
    
    class FormData {
        +Guid Id
        +Guid FormVersionId
        +string ObjectId
        +string ObjectType
        +string DataJson
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime? ModifiedDate
        +string ModifiedBy
        +FormVersion FormVersion
    }
    
    class FormPermission {
        +Guid Id
        +Guid FormId
        +string RoleCode
        +PermissionType Type
        +bool CanView
        +bool CanEdit
        +bool CanDelete
        +Form Form
    }
    
    class AuditLog {
        +Guid Id
        +string EntityType
        +Guid EntityId
        +string Action
        +string OldValue
        +string NewValue
        +DateTime CreatedDate
        +string CreatedBy
    }
    
    Form "1" --> "*" FormVersion
    FormVersion "1" --> "*" FormField
    FormVersion "1" --> "*" FormData
    FormField "1" --> "*" FieldValidation
    FormField "1" --> "*" FieldCondition
    Form "1" --> "*" FormPermission
```

## IV. BUSINESS RULES VÀ CONSTRAINTS

### 4.1 Business Rules

| Rule ID | Mô tả | Áp dụng cho |
|---------|-------|-------------|
| BR-001 | Form version phải immutable (không sửa được sau khi tạo) | FormVersion |
| BR-002 | Chỉ có 1 version active tại một thời điểm | Form |
| BR-003 | Form data phải gắn với version cụ thể | FormData |
| BR-004 | Validation phải chạy cả client và server | FormField |
| BR-005 | Mọi thay đổi form phải được audit | Form, FormVersion |
| BR-006 | User chỉ có thể edit form data nếu có quyền | FormPermission |
| BR-007 | Form data cũ không thể sửa sau khi version mới active | FormData |
| BR-008 | Field có điều kiện chỉ hiển thị khi condition đúng | FormField |

### 4.2 Business Constraints

```mermaid
graph TB
    subgraph "FORM CONSTRAINTS"
        C1[Form Code phải unique]
        C2[Form phải có ít nhất 1 version]
        C3[Version phải có format: major.minor.patch]
    end
    
    subgraph "FIELD CONSTRAINTS"
        C4[Field Code phải unique trong 1 version]
        C5[Field Order phải > 0]
        C6[Required field phải có validation]
    end
    
    subgraph "DATA CONSTRAINTS"
        C7[Data phải match với version schema]
        C8[Data không thể sửa nếu version đã inactive]
        C9[Data phải pass tất cả validations]
    end
    
    subgraph "VERSION CONSTRAINTS"
        C10[Version mới phải > version cũ]
        C11[Version active không thể xóa]
        C12[Version cũ không thể sửa]
    end
```

## V. BUSINESS SCENARIOS

### 5.1 Scenario: Bộ Y tế ban hành mẫu bệnh án mới

```
1. Admin nhận thông báo về mẫu bệnh án mới từ Bộ Y tế
2. Admin tạo version mới cho form BENH_AN (v2.0)
3. Admin cập nhật các field theo mẫu mới:
   - Thêm field mới: "Mã ICD-11"
   - Sửa field: "Chẩn đoán" thành "Chẩn đoán chính" và "Chẩn đoán phụ"
   - Xóa field cũ: "Mã ICD-10"
4. Admin thiết lập validation mới cho field "Mã ICD-11"
5. Admin duyệt và kích hoạt version mới
6. Từ thời điểm này, bệnh án mới sẽ dùng version 2.0
7. Bệnh án cũ (version 1.0) vẫn giữ nguyên, không thể sửa
```

### 5.2 Scenario: Bác sĩ điền phiếu khám với validation động

```
1. Bác sĩ mở form PHIEU_KHAM cho bệnh nhân
2. Form load metadata từ version active
3. Bác sĩ nhập thông tin:
   - Họ tên: "Nguyễn Văn A"
   - Tuổi: 25
   - Huyết áp: 180/120
4. Khi nhập Huyết áp, validation trigger:
   - Range check: 180 > 140 (cao) → Warning
   - Conditional: Nếu Huyết áp > 140 → Hiển thị field "Ghi chú"
5. Bác sĩ điền thêm "Ghi chú": "Cần theo dõi"
6. Submit form:
   - Client validation: Pass
   - Server validation: Pass
   - Lưu vào database với FormVersionId = v1.0
7. Thành công
```

### 5.3 Scenario: Xem bệnh án cũ với version khác

```
1. Bác sĩ mở bệnh án từ năm 2023 (ObjectId: 12345)
2. System load FormData:
   - FormVersionId = v1.0 (version cũ)
   - DataJson = {...}
3. System load FormVersion metadata của v1.0
4. System load Fields của v1.0
5. Form được render theo đúng schema của v1.0:
   - Hiển thị field "Mã ICD-10" (có trong v1.0)
   - Không hiển thị field "Mã ICD-11" (chỉ có trong v2.0)
6. Dữ liệu hiển thị đúng với version khi tạo
```

## VI. BUSINESS CAPABILITIES

### 6.1 Core Capabilities

| Capability | Mô tả | Business Value |
|------------|-------|----------------|
| Form Design | Tạo và cấu hình form metadata | Giảm thời gian phát triển form từ tuần → giờ |
| Dynamic Rendering | Render form từ metadata | Không cần code mới cho mỗi form |
| Version Management | Quản lý version form | Đảm bảo tính toàn vẹn dữ liệu lịch sử |
| Dynamic Validation | Validation động theo rules | Linh hoạt, dễ thay đổi |
| Data Persistence | Lưu trữ dữ liệu form | Tách biệt metadata và data |
| Audit Trail | Theo dõi mọi thay đổi | Compliance, pháp lý |

### 6.2 Supporting Capabilities

| Capability | Mô tả |
|------------|-------|
| Permission Management | Phân quyền theo role, form, field |
| Export/Import | Xuất/nhập form metadata |
| Template Management | Quản lý form templates |
| Reporting | Báo cáo sử dụng form |
| Integration | Tích hợp với HIS, LIS, PACS |

## VII. BUSINESS VALUE PROPOSITION

### 7.1 Giá trị cho Tổ chức

- ✅ **Giảm chi phí phát triển**: Không cần code mới cho mỗi form
- ✅ **Tăng tốc độ thay đổi**: Cập nhật form trong vài phút thay vì vài tuần
- ✅ **Đảm bảo compliance**: Versioning đảm bảo dữ liệu lịch sử
- ✅ **Chuẩn hóa**: Tất cả form dùng chung framework

### 7.2 Giá trị cho Người dùng

- ✅ **Dễ sử dụng**: UI nhất quán, validation rõ ràng
- ✅ **Linh hoạt**: Form thích ứng với nghiệp vụ
- ✅ **Đáng tin cậy**: Dữ liệu được validate chặt chẽ
- ✅ **Truy vết**: Có thể xem lại mọi thay đổi

## VIII. STAKEHOLDER MAP

```mermaid
graph LR
    subgraph "PRIMARY STAKEHOLDERS"
        ADMIN[Quản trị viên<br/>High Power<br/>High Interest]
        DOCTOR[Bác sĩ<br/>High Power<br/>High Interest]
        NURSE[Điều dưỡng<br/>Medium Power<br/>High Interest]
    end
    
    subgraph "SECONDARY STAKEHOLDERS"
        IT[IT Department<br/>High Power<br/>Medium Interest]
        MANAGEMENT[Ban Giám đốc<br/>High Power<br/>Low Interest]
        COMPLIANCE[Phòng Pháp chế<br/>Low Power<br/>High Interest]
    end
    
    subgraph "EXTERNAL STAKEHOLDERS"
        MINISTRY[Bộ Y tế<br/>High Power<br/>Low Interest]
        PATIENT[Bệnh nhân<br/>Low Power<br/>High Interest]
    end
```
