# Ý TƯỞNG BỔ SUNG CHO HỆ THỐNG DYNAMIC FORM

## I. CÁC Ý TƯỞNG NÂNG CAO

### 1.1 Form Template và Inheritance

**Ý tưởng**: Cho phép form kế thừa từ form template, giảm trùng lặp cấu hình.

**Lợi ích**:
- Tạo form mới nhanh hơn bằng cách kế thừa từ template
- Chuẩn hóa form theo khoa/tuyến
- Dễ cập nhật: sửa template → tất cả form kế thừa tự động cập nhật

**Implementation**:
```sql
ALTER TABLE FORM ADD TemplateFormId Guid NULL;
ALTER TABLE FORM ADD CONSTRAINT FK_FORM_TemplateFormId 
    FOREIGN KEY (TemplateFormId) REFERENCES FORM(Id);
```

**Use Case**:
- Template "PHIEU_KHAM_BASE" → Form "PHIEU_KHAM_KHOA_NOI", "PHIEU_KHAM_KHOA_NGOAI"
- Mỗi khoa có thể override một số field

### 1.2 Field Dependency và Calculation

**Ý tưởng**: Field có thể tính toán tự động dựa trên field khác.

**Ví dụ**:
- Field "BMI" = (Cân nặng) / (Chiều cao)²
- Field "Tuổi" = Năm hiện tại - Năm sinh
- Field "Tổng tiền" = Sum(các field giá trị)

**Implementation**:
```sql
CREATE TABLE FIELD_CALCULATION (
    Id Guid PK,
    FieldId Guid FK,
    Formula nvarchar(1000), -- JSON expression
    TriggerFields nvarchar(max) -- List field codes trigger calculation
);
```

**Formula Example**:
```json
{
  "type": "expression",
  "formula": "WEIGHT / (HEIGHT * HEIGHT)",
  "resultType": "number",
  "decimalPlaces": 2
}
```

### 1.3 Multi-language Support

**Ý tưởng**: Hỗ trợ đa ngôn ngữ cho label, placeholder, help text.

**Lợi ích**:
- Form có thể hiển thị tiếng Việt, tiếng Anh
- Dễ mở rộng ra thị trường quốc tế

**Implementation**:
```sql
CREATE TABLE FIELD_TRANSLATION (
    Id Guid PK,
    FieldId Guid FK,
    LanguageCode nvarchar(10), -- vi, en, fr
    Label nvarchar(200),
    Placeholder nvarchar(200),
    HelpText nvarchar(500),
    ErrorMessage nvarchar(500)
);
```

### 1.4 Form Workflow và Approval

**Ý tưởng**: Form có thể có workflow phê duyệt nhiều bước.

**Use Case**:
- Bệnh án cần duyệt: Bác sĩ → Trưởng khoa → Phó Giám đốc
- Mỗi bước có thể yêu cầu field khác nhau

**Implementation**:
```sql
CREATE TABLE FORM_WORKFLOW (
    Id Guid PK,
    FormId Guid FK,
    StepOrder int,
    StepName nvarchar(200),
    RequiredRole nvarchar(50),
    RequiredFields nvarchar(max) -- JSON array of field codes
);

CREATE TABLE FORM_DATA_APPROVAL (
    Id Guid PK,
    FormDataId Guid FK,
    WorkflowStepId Guid FK,
    Status int, -- 0=Pending, 1=Approved, 2=Rejected
    ApprovedBy nvarchar(100),
    ApprovedDate DateTime,
    Comments nvarchar(1000)
);
```

### 1.5 Form Analytics và Reporting

**Ý tưởng**: Thống kê sử dụng form, thời gian điền, tỷ lệ lỗi validation.

**Metrics**:
- Số lần form được mở
- Thời gian trung bình điền form
- Tỷ lệ validation fail
- Field nào bị bỏ trống nhiều nhất

**Implementation**:
```sql
CREATE TABLE FORM_ANALYTICS (
    Id Guid PK,
    FormId Guid FK,
    FormVersionId Guid FK,
    EventType nvarchar(50), -- OPEN, SUBMIT, VALIDATION_FAIL, ABANDON
    FieldCode nvarchar(50) NULL,
    EventDate DateTime,
    UserId nvarchar(100),
    Metadata nvarchar(max) -- JSON
);
```

### 1.6 Offline Support và Sync

**Ý tưởng**: Cho phép điền form offline, sync sau khi có internet.

**Lợi ích**:
- Điều dưỡng có thể điền form ở khu vực không có wifi
- Mobile app hoạt động tốt hơn

**Implementation**:
- Lưu form data vào local storage (IndexedDB)
- Queue sync khi có internet
- Conflict resolution khi có nhiều device

### 1.7 Form Version Comparison

**Ý tưởng**: So sánh 2 version để xem thay đổi gì.

**Features**:
- Highlight field mới, field bị xóa, field bị sửa
- Show diff của validation rules
- Export report thay đổi

**Use Case**:
- Admin muốn xem version 2.0 khác gì version 1.0
- Báo cáo thay đổi cho ban giám đốc

### 1.8 Conditional Section và Page Break

**Ý tưởng**: Form có thể chia thành nhiều section/page, hiển thị theo điều kiện.

**Features**:
- Section chỉ hiển thị khi condition đúng
- Form dài có thể chia thành nhiều tab/page
- Progress indicator

**Implementation**:
```sql
ALTER TABLE FORM_FIELD ADD SectionId Guid NULL;
ALTER TABLE FORM_FIELD ADD CONSTRAINT FK_FORM_FIELD_SectionId 
    FOREIGN KEY (SectionId) REFERENCES FORM_SECTION(Id);

CREATE TABLE FORM_SECTION (
    Id Guid PK,
    FormVersionId Guid FK,
    SectionName nvarchar(200),
    DisplayOrder int,
    ConditionExpression nvarchar(1000) NULL
);
```

### 1.9 Form Data Export/Import

**Ý tưởng**: Xuất/nhập form data ra Excel, PDF, JSON.

**Formats**:
- Excel: Dễ phân tích, báo cáo
- PDF: In ấn, lưu trữ
- JSON: Tích hợp với hệ thống khác
- CSV: Import vào hệ thống khác

**Features**:
- Template export tùy chỉnh
- Batch export nhiều form data
- Scheduled export

### 1.10 Form Validation với External Data

**Ý tưởng**: Validation có thể check với dữ liệu từ hệ thống khác.

**Ví dụ**:
- Validate "Mã bệnh nhân" phải tồn tại trong HIS
- Validate "Mã xét nghiệm" phải có trong LIS
- Validate "Mã ICD" phải đúng format từ Bộ Y tế

**Implementation**:
```sql
ALTER TABLE FIELD_VALIDATION ADD ValidationSource nvarchar(50) NULL;
-- ValidationSource: 'INTERNAL', 'HIS', 'LIS', 'EXTERNAL_API'
ALTER TABLE FIELD_VALIDATION ADD ExternalEndpoint nvarchar(500) NULL;
```

## II. CẢI TIẾN PERFORMANCE

### 2.1 Caching Strategy

**Metadata Caching**:
- Cache form metadata trong Redis
- TTL: 1 giờ
- Invalidate khi có thay đổi

**Data Caching**:
- Cache form data đã load gần đây
- Cache theo ObjectId
- TTL: 5 phút

### 2.2 Lazy Loading Fields

**Ý tưởng**: Chỉ load field khi cần thiết (scroll vào view).

**Lợi ích**:
- Form dài load nhanh hơn
- Giảm memory usage

### 2.3 Database Optimization

**Partitioning**:
- Partition FORM_DATA theo tháng
- Partition AUDIT_LOG theo tháng

**Archiving**:
- Archive form data cũ (> 2 năm) sang storage rẻ hơn
- Giữ metadata trong DB chính

## III. SECURITY ENHANCEMENTS

### 3.1 Field-level Encryption

**Ý tưởng**: Mã hóa field nhạy cảm (Số CMND, Số điện thoại).

**Implementation**:
```sql
ALTER TABLE FORM_FIELD ADD IsEncrypted bit DEFAULT 0;
ALTER TABLE FORM_DATA ADD EncryptedFields nvarchar(max) NULL;
```

### 3.2 Data Masking

**Ý tưởng**: Mask dữ liệu khi xem (chỉ hiển thị 4 số cuối).

**Use Case**:
- Nhân viên hành chính xem form nhưng không thấy đầy đủ thông tin nhạy cảm

### 3.3 Audit Trail nâng cao

**Features**:
- Track mọi thay đổi field-level
- So sánh before/after
- Export audit report
- Alert khi có thay đổi bất thường

## IV. UX/UI IMPROVEMENTS

### 4.1 Auto-save

**Ý tưởng**: Tự động lưu draft khi người dùng đang điền.

**Lợi ích**:
- Không mất dữ liệu khi browser crash
- Có thể tiếp tục điền sau

**Implementation**:
- Auto-save mỗi 30 giây
- Lưu vào local storage + server
- Show indicator "Đã lưu"

### 4.2 Smart Defaults

**Ý tưởng**: Tự động điền giá trị mặc định thông minh.

**Ví dụ**:
- Ngày khám = Hôm nay
- Bác sĩ = User hiện tại
- Khoa = Khoa của user

### 4.3 Form Validation Real-time

**Ý tưởng**: Validate ngay khi user nhập (không đợi submit).

**Features**:
- Validate on blur
- Show error ngay lập tức
- Highlight field lỗi

### 4.4 Mobile-optimized UI

**Ý tưởng**: UI tối ưu cho mobile.

**Features**:
- Touch-friendly controls
- Swipe để chuyển field
- Camera integration cho file upload
- GPS integration cho location field

## V. INTEGRATION IDEAS

### 5.1 HIS Integration

**Features**:
- Pull patient info từ HIS
- Push form data vào HIS
- Real-time sync

**API Integration**:
```csharp
public interface IHisIntegrationService
{
    Task<PatientInfo> GetPatientInfo(string patientId);
    Task<bool> SubmitFormData(string formCode, object data);
}
```

### 5.2 LIS/PACS Integration

**Features**:
- Tự động điền kết quả xét nghiệm vào form
- Hiển thị hình ảnh từ PACS trong form

### 5.3 HL7/FHIR Support

**Ý tưởng**: Hỗ trợ chuẩn HL7/FHIR để tích hợp với hệ thống y tế quốc tế.

**Benefits**:
- Dễ tích hợp với hệ thống nước ngoài
- Chuẩn hóa dữ liệu y tế

## VI. TESTING VÀ QUALITY

### 6.1 Form Testing Framework

**Ý tưởng**: Framework test form tự động.

**Features**:
- Test validation rules
- Test conditional logic
- Test form rendering
- Performance testing

### 6.2 Form Validation Testing

**Tools**:
- Unit test cho validation rules
- Integration test cho form workflow
- E2E test cho user journey

## VII. MONITORING VÀ ALERTING

### 7.1 Health Monitoring

**Metrics**:
- API response time
- Database query performance
- Cache hit rate
- Error rate

### 7.2 Alerting

**Alerts**:
- Form validation fail rate cao
- API error rate cao
- Database slow query
- Cache miss rate cao

## VIII. DOCUMENTATION VÀ TRAINING

### 8.1 Form Builder Documentation

**Content**:
- Hướng dẫn tạo form
- Best practices
- Common patterns
- Troubleshooting

### 8.2 User Training Materials

**Content**:
- Video hướng dẫn
- User manual
- FAQ
- Training sessions

## IX. ROADMAP IMPLEMENTATION

### Phase 1 (MVP)
- ✅ Form metadata model
- ✅ Dynamic rendering
- ✅ Basic validation
- ✅ Versioning

### Phase 2 (Enhancement)
- Form template
- Field calculation
- Multi-language
- Analytics

### Phase 3 (Advanced)
- Workflow approval
- Offline support
- External validation
- Advanced security

### Phase 4 (Enterprise)
- HL7/FHIR support
- Advanced analytics
- AI/ML integration
- Multi-tenant
