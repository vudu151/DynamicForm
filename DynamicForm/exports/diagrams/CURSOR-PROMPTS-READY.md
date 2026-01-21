# PROMPTS SẴN SÀNG ĐỂ PASTE VÀO CURSOR AI

> **Cách dùng**: Copy prompt → Mở Cursor → Ctrl+K → Paste → Tab để accept

---

## PROMPT 1: Business Architecture Tổng Thể

```
Tạo sơ đồ Mermaid flowchart cho Business Architecture của hệ thống DynamicForm với:

1. Business Actors (màu xanh dương):
   - Admin: Quản trị viên, tạo/sửa form, quản lý version
   - Doctor: Bác sĩ, điền phiếu khám, xem bệnh án
   - Nurse: Điều dưỡng, điền phiếu chăm sóc
   - Lab Tech: Kỹ thuật viên, điền phiếu xét nghiệm

2. Access Channels (màu xanh lá):
   - Web: Razor Pages, Desktop/Tablet
   - Mobile: Android/iOS, Offline capable

3. Business Processes (màu cam):
   - Form Design: Thiết kế metadata, cấu hình field
   - Version Management: Tạo version, kích hoạt
   - Form Filling: Điền form, validate
   - Form Review: Xem dữ liệu, export

4. Business Entities (màu tím):
   - Form, Field, Validation, Data

5. Business Rules (màu đỏ):
   - Versioning, Validation, Permission, Audit

Yêu cầu:
- Format: graph TB (top to bottom)
- Có subgraph để nhóm logic
- Có mũi tên kết nối rõ ràng
- Sử dụng classDef để thêm màu sắc cho các subgraph
- Code Mermaid đầy đủ, có thể copy paste vào mermaid.live
```

---

## PROMPT 2: Luồng Tạo Form (Admin)

```
Tạo sequence diagram Mermaid cho luồng tạo và quản lý form:

Participants:
- Admin (Quản trị viên)
- FormDesigner (Form Designer)
- ValidationEngine (Validation Engine)
- VersionManager (Version Manager)

Luồng:
1. Admin tạo form mới (PHIEU_KHAM)
2. FormDesigner định nghĩa form metadata
3. FormDesigner thêm các field (Text, Number, Date...)
4. FormDesigner thiết lập validation rules
5. ValidationEngine xác nhận rules hợp lệ
6. FormDesigner tạo version 1.0
7. VersionManager lưu form + version (trạng thái Draft)
8. Admin duyệt và kích hoạt version
9. VersionManager set IsActive = true
10. Form đã active, sẵn sàng sử dụng

Format: sequenceDiagram
Có notes giải thích các bước quan trọng
```

---

## PROMPT 3: Luồng Điền Form - Web

```
Tạo sequence diagram Mermaid cho luồng điền form qua Web Application:

Participants:
- User (Bác sĩ/Điều dưỡng)
- WebApp (Web Application)
- API (Backend API)
- Validator (Validation Engine)

Luồng:
1. User mở form (code: PHIEU_KHAM)
2. WebApp gọi API lấy form metadata (version active)
3. API trả về metadata (fields, validation)
4. WebApp render form động từ metadata
5. WebApp hiển thị form cho User
6. User nhập dữ liệu vào form
7. User bấm Submit
8. WebApp validate client-side
9. Nếu validation fail: WebApp hiển thị lỗi ngay trên form
10. Nếu validation pass: WebApp gửi dữ liệu để validate
11. API gọi Validator validate server-side
12. Nếu validation fail: API trả về lỗi chi tiết, WebApp hiển thị lỗi theo từng field
13. Nếu validation pass: API lưu dữ liệu form
14. API trả về thành công (201 Created)
15. WebApp hiển thị thông báo "Lưu thành công"

Format: sequenceDiagram với alt blocks cho error handling
Có notes giải thích
```

---

## PROMPT 4: Luồng Điền Form - Mobile

```
Tạo sequence diagram Mermaid cho luồng điền form qua Mobile App:

Participants:
- User (Bác sĩ/Điều dưỡng)
- MobileApp (Mobile App)
- API (Backend API)
- Validator (Validation Engine)

Luồng:
1. User mở app, chọn form
2. MobileApp hiển thị danh sách form
3. User chọn PHIEU_KHAM
4. MobileApp gọi API lấy form metadata
5. API trả về metadata
6. MobileApp render form động (native controls: Entry, DatePicker, Picker...)
7. MobileApp hiển thị form native cho User
8. User nhập dữ liệu (có thể tại giường bệnh)
9. User bấm Submit
10. MobileApp gọi API validate dữ liệu trước
11. API gọi Validator validate
12. Nếu có lỗi: API trả về lỗi theo field, MobileApp hiển thị lỗi dưới từng field
13. Nếu validation OK: MobileApp gửi dữ liệu để lưu
14. API lưu dữ liệu form
15. API trả về thành công
16. MobileApp hiển thị thông báo "Lưu thành công"

Format: sequenceDiagram với alt blocks
Có notes giải thích
```

---

## PROMPT 5: Luồng Tạo Version Mới

```
Tạo sequence diagram Mermaid cho luồng tạo version mới của form:

Participants:
- Admin (Quản trị viên)
- VersionManager (Version Manager)
- Designer (Form Designer)

Luồng:
1. Admin yêu cầu tạo version mới
2. VersionManager load version hiện tại (v1.0)
3. VersionManager copy metadata từ version cũ
4. VersionManager tạo version mới (v2.0)
5. VersionManager trả về version mới (trạng thái Draft)
6. Admin chỉnh sửa fields/validation (thêm field mới, sửa validation, xóa field cũ)
7. Designer lưu thay đổi
8. VersionManager cập nhật version 2.0
9. Admin kích hoạt version mới
10. VersionManager set v2.0 IsActive = true, set v1.0 IsActive = false
11. VersionManager trả về "Version 2.0 đã active"

Notes:
- Version cũ (v1.0) vẫn giữ nguyên, không sửa được
- Data cũ gắn với v1.0
- Từ thời điểm này, form mới sẽ dùng version 2.0

Format: sequenceDiagram với notes giải thích
```

---

## PROMPT 6: Luồng Xem Dữ liệu Form Cũ

```
Tạo sequence diagram Mermaid cho luồng xem dữ liệu form cũ:

Participants:
- Doctor (Bác sĩ)
- App (Web/Mobile App)
- API (Backend API)

Luồng:
1. Doctor xem bệnh án cũ (ObjectId: 12345)
2. App gọi API lấy dữ liệu form
3. API load FormData (có FormVersionId = v1.0 - version cũ)
4. API load metadata của version v1.0
5. API load Fields của version v1.0
6. API trả về Data + Metadata (version cũ)
7. App render form theo đúng version cũ
8. App hiển thị form với dữ liệu cũ

Notes:
- Form được render theo đúng schema của version khi dữ liệu được tạo
- Ví dụ: v1.0 có field "Mã ICD-10", v2.0 có "Mã ICD-11"
- Khi xem dữ liệu cũ, hiển thị đúng field của version đó

Format: sequenceDiagram với notes giải thích
```

---

## PROMPT 7: Domain Model (Class Diagram)

```
Tạo class diagram Mermaid cho Domain Model của DynamicForm:

Classes:

1. Form
   + Id: Guid
   + Code: string (unique)
   + Name: string
   + Status: int
   + CurrentVersionId: Guid?

2. FormVersion
   + Id: Guid
   + FormId: Guid (FK)
   + Version: string
   + IsActive: bool
   + CreatedDate: DateTime

3. FormField
   + Id: Guid
   + FormVersionId: Guid (FK)
   + FieldCode: string
   + FieldType: int (1=Text, 2=Number, 3=Date, 6=Select, 10=TextArea)
   + Label: string
   + IsRequired: bool
   + IsVisible: bool

4. FieldValidation
   + Id: Guid
   + FieldId: Guid (FK)
   + RuleType: int (1=Required, 2=Min, 3=Max, 4=Range, 5=Regex)
   + RuleValue: string
   + ErrorMessage: string

5. FormData
   + Id: Guid
   + FormVersionId: Guid (FK)
   + ObjectId: string
   + ObjectType: string
   + DataJson: string (Dictionary<string, object>)

Relationships:
- Form "1" --> "*" FormVersion
- FormVersion "1" --> "*" FormField
- FormVersion "1" --> "*" FormData
- FormField "1" --> "*" FieldValidation

Format: classDiagram với visibility (+, -, #) và relationships rõ ràng
```

---

## PROMPT 8: State Machine (Form/Version)

```
Tạo state diagram Mermaid cho Form/Version State Machine:

States:
- Draft: Form version mới tạo, chưa active
- Active: Version đang được sử dụng
- Inactive: Version đã bị deactivate

Transitions:
- Draft --> Active: Khi admin activate version
- Active --> Inactive: Khi admin deactivate form hoặc activate version khác
- Inactive --> Active: Khi admin activate lại version này

Format: stateDiagram-v2
Có notes giải thích mỗi transition
```

---

## PROMPT 9: Cải Thiện Diagram Có Sẵn

```
Cải thiện sơ đồ Mermaid này để:

1. Dễ hiểu hơn cho business stakeholders
2. Thêm màu sắc cho các nhóm (subgraph) hoặc participants
3. Làm rõ mối quan hệ giữa các thành phần
4. Tối ưu layout để presentation đẹp hơn
5. Thêm labels và descriptions rõ ràng
6. Đảm bảo code hợp lệ, có thể render trên mermaid.live

Code hiện tại:
[Paste code Mermaid hiện có vào đây]
```

---

## PROMPT 10: Tối Ưu Cho Presentation

```
Tối ưu sơ đồ Mermaid này để trình bày cho business lead:

1. Tăng font size và spacing giữa các elements
2. Làm rõ hierarchy (dùng subgraph nếu cần)
3. Thêm styling đẹp với màu sắc
4. Đảm bảo dễ đọc khi export PNG 1920x1080
5. Thêm labels và descriptions ngắn gọn
6. Tối ưu layout để không bị chật
7. Sử dụng classDef để thêm màu sắc

Code hiện tại:
[Paste code Mermaid hiện có vào đây]
```

---

## CÁCH SỬ DỤNG

### Bước 1: Chọn prompt phù hợp
- Copy prompt từ file này

### Bước 2: Mở Cursor
- Mở file `.mmd` mới hoặc file có sẵn
- Nếu file mới: để trống
- Nếu file có sẵn: select code cần cải thiện

### Bước 3: Dùng Cursor AI
- Nhấn **Ctrl+K** (Windows) hoặc **Cmd+K** (Mac)
- Paste prompt đã copy
- Nhấn **Tab** để accept suggestion

### Bước 4: Test và Export
- Copy code mới từ Cursor
- Paste vào https://mermaid.live
- Export PNG/SVG/PDF

---

## TIPS

- **Prompt 9 và 10**: Dùng để cải thiện diagram có sẵn
- **Prompt 1-8**: Dùng để generate diagram mới
- Có thể combine: Generate → Cải thiện → Tối ưu
- Test trên mermaid.live trước khi export

---

**Tất cả prompts đã được test và hoạt động tốt với Cursor AI!**
