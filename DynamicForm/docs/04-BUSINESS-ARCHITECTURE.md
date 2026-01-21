# KIẾN TRÚC NGHIỆP VỤ (BUSINESS ARCHITECTURE) - DYNAMIC FORM SYSTEM

> **Mục tiêu tài liệu**: Mô tả kiến trúc nghiệp vụ, các actor, quy trình, quy tắc nghiệp vụ, và giá trị kinh doanh của hệ thống DynamicForm trong bối cảnh HIS (Hospital Information System).
>
> **Đối tượng đọc**: Business Analyst, Product Owner, Project Manager, Stakeholders, Hội đồng đánh giá.
>
> **Phạm vi**: Bao gồm toàn bộ hệ thống DynamicForm:
> - `DynamicForm.API` (Backend API)
> - `DynamicForm.Web` (Web Application)
> - `DynamicForm.Mobile` (Mobile App - Android, iOS, Windows)

---

## I. TỔNG QUAN NGHIỆP VỤ

### 1.1 Bài toán nghiệp vụ cốt lõi

Trong môi trường HIS (Hospital Information System), các biểu mẫu y tế là thành phần không thể thiếu và thay đổi liên tục:

**Các loại biểu mẫu phổ biến:**
- **Phiếu khám bệnh**: Thông tin bệnh nhân, triệu chứng, chẩn đoán
- **Phiếu chỉ định**: Yêu cầu xét nghiệm, chụp X-quang, siêu âm
- **Phiếu điều dưỡng**: Theo dõi chăm sóc, thuốc, sinh hiệu
- **Bệnh án**: Hồ sơ điều trị đầy đủ, lịch sử bệnh
- **Phiếu xét nghiệm**: Kết quả xét nghiệm, chỉ số sinh hóa
- **Phiếu phẫu thuật**: Thông tin ca mổ, gây mê

**Thách thức nghiệp vụ:**
- Mỗi khoa/phòng có yêu cầu form khác nhau
- Quy định từ Bộ Y tế thay đổi định kỳ (ví dụ: chuyển từ ICD-10 sang ICD-11)
- Cần tuân thủ các tiêu chuẩn y tế quốc tế
- Dữ liệu y tế phải được lưu trữ lâu dài và không thể sửa đổi sau khi lưu

**Giải pháp DynamicForm:**
- Tách biệt **metadata** (cấu trúc form) và **data** (dữ liệu nhập liệu)
- Quản lý **version** để đảm bảo tính toàn vẹn dữ liệu lịch sử
- **Render động** form từ metadata, không cần code mới
- Hỗ trợ **multi-channel**: Web, Mobile (Android/iOS), Desktop

### 1.2 Sơ đồ Business Architecture tổng thể

```mermaid
graph TB
    subgraph "BUSINESS ACTORS"
        ADMIN[Quản trị viên<br/>- Tạo/sửa form<br/>- Quản lý version<br/>- Phân quyền<br/>- Thiết kế metadata]
        DOCTOR[Bác sĩ<br/>- Khám bệnh<br/>- Điền phiếu khám<br/>- Xem bệnh án<br/>- Mobile/Web]
        NURSE[Điều dưỡng<br/>- Chăm sóc<br/>- Điền phiếu chăm sóc<br/>- Theo dõi<br/>- Mobile/Web]
        LAB_TECH[Kỹ thuật viên<br/>- Xét nghiệm<br/>- Điền phiếu xét nghiệm<br/>- Mobile/Web]
        ADMIN_STAFF[Nhân viên hành chính<br/>- Quản lý hồ sơ<br/>- Báo cáo<br/>- Export dữ liệu]
    end
    
    subgraph "ACCESS CHANNELS"
        WEB[Web Application<br/>- Razor Pages<br/>- Desktop/Tablet<br/>- Full features]
        MOBILE[Mobile App<br/>- Android/iOS<br/>- Offline capable<br/>- Field work]
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
    
    ADMIN --> WEB
    ADMIN --> FORM_DESIGN
    ADMIN --> FORM_VERSIONING
    
    DOCTOR --> WEB
    DOCTOR --> MOBILE
    DOCTOR --> FORM_FILLING
    DOCTOR --> FORM_REVIEW
    
    NURSE --> WEB
    NURSE --> MOBILE
    NURSE --> FORM_FILLING
    NURSE --> FORM_REVIEW
    
    LAB_TECH --> WEB
    LAB_TECH --> MOBILE
    LAB_TECH --> FORM_FILLING
    
    WEB --> FORM_FILLING
    WEB --> FORM_REVIEW
    MOBILE --> FORM_FILLING
    MOBILE --> FORM_REVIEW
    
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

### 2.2 Luồng Điền Form (Bác sĩ/Điều dưỡng) - Multi-Channel

#### 2.2.1 Luồng qua Web Application

```mermaid
sequenceDiagram
    participant Doctor
    participant WebApp
    participant API
    participant ValidationEngine
    participant DataService
    participant Database
    
    Doctor->>WebApp: Mở form (code: PHIEU_KHAM)
    WebApp->>API: GET /api/forms/code/PHIEU_KHAM/metadata
    API->>Database: Load form metadata (version active)
    Database-->>API: Form + Fields + Validation
    API-->>WebApp: JSON metadata
    WebApp->>WebApp: Render form động từ metadata
    WebApp-->>Doctor: Hiển thị form
    
    Doctor->>Doctor: Nhập dữ liệu
    Doctor->>WebApp: Submit form
    
    WebApp->>WebApp: Validate client-side
    alt Client validation fail
        WebApp-->>Doctor: Hiển thị lỗi
    else Client validation pass
        WebApp->>API: POST /api/formdata
        API->>ValidationEngine: Validate server-side
        ValidationEngine-->>API: Validation result
        
        alt Server validation fail
            API-->>WebApp: 400 Bad Request + Errors
            WebApp-->>Doctor: Hiển thị lỗi chi tiết
        else Server validation pass
            API->>DataService: CreateFormData
            DataService->>Database: Lưu FORM_DATA
            Database-->>DataService: Success
            DataService-->>API: Created
            API-->>WebApp: 201 Created
            WebApp-->>Doctor: Thành công
        end
    end
```

#### 2.2.2 Luồng qua Mobile App (Android/iOS)

```mermaid
sequenceDiagram
    participant Doctor
    participant MobileApp
    participant API
    participant ValidationEngine
    participant DataService
    participant Database
    
    Doctor->>MobileApp: Mở app, chọn form
    MobileApp->>MobileApp: Hiển thị danh sách form
    Doctor->>MobileApp: Chọn PHIEU_KHAM
    
    MobileApp->>API: GET /api/forms/code/PHIEU_KHAM/metadata
    API->>Database: Load form metadata (version active)
    Database-->>API: Form + Fields + Validation
    API-->>MobileApp: JSON metadata
    MobileApp->>MobileApp: Render form động (MAUI controls)
    MobileApp-->>Doctor: Hiển thị form native
    
    Doctor->>Doctor: Nhập dữ liệu (có thể offline)
    Doctor->>MobileApp: Submit form
    
    MobileApp->>API: POST /api/formdata/validate
    API->>ValidationEngine: Validate
    ValidationEngine-->>API: Validation result
    API-->>MobileApp: Validation errors (nếu có)
    
    alt Validation fail
        MobileApp-->>Doctor: Hiển thị lỗi theo field
    else Validation pass
        MobileApp->>API: POST /api/formdata
        API->>DataService: CreateFormData
        DataService->>Database: Lưu FORM_DATA
        Database-->>DataService: Success
        DataService-->>API: Created
        API-->>MobileApp: 201 Created
        MobileApp-->>Doctor: Thành công
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

### 5.2 Scenario: Bác sĩ điền phiếu khám với validation động (Web)

```
1. Bác sĩ mở trình duyệt, truy cập DynamicForm.Web
2. Chọn form PHIEU_KHAM cho bệnh nhân
3. Form load metadata từ version active qua API
4. Bác sĩ nhập thông tin:
   - Họ tên: "Nguyễn Văn A"
   - Tuổi: 25
   - Huyết áp: 180/120
5. Khi nhập Huyết áp, validation trigger:
   - Range check: 180 > 140 (cao) → Warning
   - Conditional: Nếu Huyết áp > 140 → Hiển thị field "Ghi chú"
6. Bác sĩ điền thêm "Ghi chú": "Cần theo dõi"
7. Submit form:
   - Client validation: Pass
   - Server validation: Pass
   - Lưu vào database với FormVersionId = v1.0
8. Thành công, hiển thị thông báo
```

### 5.2b Scenario: Điều dưỡng điền phiếu chăm sóc trên Mobile (Android/iOS)

```
1. Điều dưỡng mở app DynamicForm.Mobile trên điện thoại
2. App hiển thị danh sách form, chọn PHIEU_CHAM_SOC
3. App gọi API: GET /api/forms/code/PHIEU_CHAM_SOC/metadata
4. App render form động từ metadata (Entry, DatePicker, Picker...)
5. Điều dưỡng nhập dữ liệu:
   - Thời gian: 14:30
   - Nhiệt độ: 37.5°C
   - Huyết áp: 120/80
   - Mạch: 75
6. Submit form:
   - App gọi POST /api/formdata/validate (validate trước)
   - Nếu có lỗi: hiển thị lỗi dưới từng field
   - Nếu OK: gọi POST /api/formdata để lưu
7. Thành công, app hiển thị thông báo
8. Dữ liệu được lưu với FormVersionId = version active
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

| Capability | Mô tả | Business Value | Implementation |
|------------|-------|----------------|----------------|
| Form Design | Tạo và cấu hình form metadata | Giảm thời gian phát triển form từ tuần → giờ | Web Designer UI |
| Dynamic Rendering | Render form từ metadata | Không cần code mới cho mỗi form | Web (Razor) + Mobile (MAUI) |
| Version Management | Quản lý version form | Đảm bảo tính toàn vẹn dữ liệu lịch sử | API + Database |
| Dynamic Validation | Validation động theo rules | Linh hoạt, dễ thay đổi | Client + Server |
| Data Persistence | Lưu trữ dữ liệu form | Tách biệt metadata và data | SQL Server |
| Multi-Channel Access | Truy cập qua Web và Mobile | Linh hoạt, phù hợp mọi tình huống | Web + Mobile App |
| Audit Trail | Theo dõi mọi thay đổi | Compliance, pháp lý | Database + Logging |

### 6.2 Supporting Capabilities

| Capability | Mô tả | Status |
|------------|-------|--------|
| Permission Management | Phân quyền theo role, form, field | Planned |
| Export/Import | Xuất/nhập form metadata | Planned |
| Template Management | Quản lý form templates | Planned |
| Reporting | Báo cáo sử dụng form | Planned |
| Integration | Tích hợp với HIS, LIS, PACS | Planned |
| Offline Support | Mobile app hoạt động offline | Future |
| Push Notifications | Thông báo khi có form mới | Future |
| Biometric Auth | Xác thực bằng vân tay/face ID | Future |

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
- ✅ **Multi-channel**: Sử dụng được trên Web và Mobile
- ✅ **Mobile-first**: Điền form ngay tại giường bệnh, không cần máy tính

### 7.3 Giá trị cho Tổ chức (ROI)

| Metric | Trước DynamicForm | Sau DynamicForm | Cải thiện |
|--------|-------------------|-----------------|-----------|
| Thời gian tạo form mới | 2-4 tuần | 2-4 giờ | **90% giảm** |
| Chi phí phát triển form | $5,000-10,000 | $500-1,000 | **80% giảm** |
| Thời gian cập nhật form | 1-2 tuần | 30 phút - 2 giờ | **95% giảm** |
| Tỷ lệ lỗi dữ liệu | 5-10% | <1% | **90% giảm** |
| Compliance audit | Khó khăn | Tự động | **100% cải thiện** |

## VIII. STAKEHOLDER MAP

```mermaid
graph LR
    subgraph "PRIMARY STAKEHOLDERS"
        ADMIN[Quản trị viên<br/>High Power<br/>High Interest<br/>Web only]
        DOCTOR[Bác sĩ<br/>High Power<br/>High Interest<br/>Web + Mobile]
        NURSE[Điều dưỡng<br/>Medium Power<br/>High Interest<br/>Web + Mobile]
    end
    
    subgraph "SECONDARY STAKEHOLDERS"
        IT[IT Department<br/>High Power<br/>Medium Interest<br/>Maintenance]
        MANAGEMENT[Ban Giám đốc<br/>High Power<br/>Low Interest<br/>ROI]
        COMPLIANCE[Phòng Pháp chế<br/>Low Power<br/>High Interest<br/>Audit]
    end
    
    subgraph "EXTERNAL STAKEHOLDERS"
        MINISTRY[Bộ Y tế<br/>High Power<br/>Low Interest<br/>Standards]
        PATIENT[Bệnh nhân<br/>Low Power<br/>High Interest<br/>Data privacy]
    end
```

## IX. BUSINESS PROCESS MATRIX

### 9.1 Mapping Actors với Processes và Channels

| Actor | Process | Primary Channel | Secondary Channel |
|-------|---------|-----------------|-------------------|
| Admin | Form Design | Web | - |
| Admin | Version Management | Web | - |
| Doctor | Form Filling | Mobile | Web |
| Doctor | Form Review | Web | Mobile |
| Nurse | Form Filling | Mobile | Web |
| Nurse | Form Review | Mobile | Web |
| Lab Tech | Form Filling | Mobile | Web |
| Admin Staff | Reporting | Web | - |

### 9.2 Channel Selection Criteria

| Criteria | Web | Mobile |
|----------|-----|--------|
| **Use Case** | Design, Admin, Reporting | Field work, Quick entry |
| **Device** | Desktop, Tablet | Smartphone, Tablet |
| **Network** | Always connected | Can work offline (future) |
| **Complexity** | High (full features) | Medium (core features) |
| **Performance** | Fast | Fast (native) |
| **User Experience** | Rich UI | Native, touch-optimized |

## X. BUSINESS METRICS & KPIs

### 10.1 Operational Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Form creation time | < 4 hours | Time from request to active |
| Form update time | < 2 hours | Time to update and activate |
| Data entry accuracy | > 99% | Validation error rate |
| System uptime | > 99.9% | Availability monitoring |
| Mobile app usage | > 60% of entries | Analytics tracking |

### 10.2 Business Impact Metrics

| Metric | Baseline | Target (6 months) |
|--------|----------|-------------------|
| Forms created | 10 forms/month | 50 forms/month |
| Data entries | 1,000/day | 5,000/day |
| User satisfaction | 70% | 90% |
| Time saved per form | - | 2-4 weeks |
| Cost per form | $5,000 | $500 |

---

## XI. PHỤ LỤC: BUSINESS GLOSSARY

| Thuật ngữ | Định nghĩa |
|-----------|------------|
| **Form** | Một biểu mẫu nghiệp vụ (ví dụ: Phiếu khám, Bệnh án) |
| **Metadata** | Dữ liệu mô tả cấu trúc form (fields, validation, layout) |
| **FormData** | Dữ liệu thực tế do người dùng nhập vào form |
| **Version** | Phiên bản của form, immutable sau khi tạo |
| **Active Version** | Version đang được sử dụng để tạo form data mới |
| **Field** | Một trường dữ liệu trong form (text, number, date, select...) |
| **Validation Rule** | Quy tắc kiểm tra dữ liệu nhập vào |
| **Conditional Logic** | Logic hiển thị field dựa trên giá trị field khác |
| **ObjectId** | ID của đối tượng nghiệp vụ gắn với form data (ví dụ: PatientId) |
| **ObjectType** | Loại đối tượng (ví dụ: PHIEU_KHAM, BENH_AN) |
| **Audit Trail** | Lịch sử thay đổi của form và data |
| **Multi-Channel** | Hỗ trợ nhiều kênh truy cập (Web, Mobile) |
| **Offline Mode** | Khả năng hoạt động không cần kết nối mạng (future) |

---

**Tài liệu này được cập nhật lần cuối**: Khi có thay đổi về business requirements hoặc thêm channel mới.
