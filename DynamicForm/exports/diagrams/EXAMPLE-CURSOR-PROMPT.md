# VÍ DỤ THỰC TẾ: DÙNG CURSOR AI ĐỂ CẢI THIỆN DIAGRAM

## 🎯 Mục đích
File này chứa các prompt mẫu để bạn copy-paste vào Cursor AI

---

## 📋 PROMPT 1: Fix Encoding và Cải thiện Diagram

**Bước 1:** Mở file `exports\diagrams\flowchart-1.mmd` trong Cursor

**Bước 2:** Select toàn bộ code

**Bước 3:** Dùng Cursor AI (Ctrl+K) và paste prompt này:

```
Fix encoding issues và cải thiện sơ đồ Mermaid này:

1. Fix tất cả ký tự tiếng Việt bị lỗi encoding
2. Thêm màu sắc cho các subgraph:
   - BUSINESS ACTORS: màu xanh dương (#3498db)
   - ACCESS CHANNELS: màu xanh lá (#2ecc71)
   - BUSINESS PROCESSES: màu cam (#e67e22)
   - BUSINESS ENTITIES: màu tím (#9b59b6)
   - BUSINESS RULES: màu đỏ (#e74c3c)
3. Làm rõ layout và spacing
4. Đảm bảo code Mermaid hợp lệ, có thể render trên mermaid.live

Code hiện tại:
[Code sẽ tự động được select]
```

**Bước 4:** Cursor sẽ generate code mới → Review → Accept

**Bước 5:** Copy code mới → Paste vào https://mermaid.live → Export PNG

---

## 📋 PROMPT 2: Generate Sequence Diagram Mới

**Bước 1:** Tạo file mới: `exports\diagrams\sequence-mobile-fill.mmd`

**Bước 2:** Mở file trong Cursor (để trống)

**Bước 3:** Dùng Cursor AI (Ctrl+K) và paste prompt này:

```
Tạo sequence diagram Mermaid cho luồng điền form trên Mobile App:

Participants:
- Doctor (Bác sĩ)
- MobileApp (Ứng dụng mobile)
- API (Backend API)
- ValidationEngine (Engine validate)
- Database (Cơ sở dữ liệu)

Luồng:
1. Doctor mở app, chọn form PHIEU_KHAM
2. MobileApp gọi API: GET /api/forms/code/PHIEU_KHAM/metadata
3. API query Database để load form metadata (version active)
4. Database trả về Form + Fields + Validation rules
5. API trả về JSON metadata cho MobileApp
6. MobileApp render form động từ metadata (Entry, DatePicker, Picker...)
7. Doctor nhập dữ liệu vào form
8. Doctor bấm Submit
9. MobileApp validate client-side
10. MobileApp gọi API: POST /api/formdata/validate
11. ValidationEngine validate server-side
12. Nếu có lỗi: API trả về errors, MobileApp hiển thị lỗi dưới từng field
13. Nếu OK: MobileApp gọi POST /api/formdata
14. API lưu vào Database
15. Database trả về success
16. API trả về 201 Created
17. MobileApp hiển thị thông báo thành công

Format: sequenceDiagram với alt blocks cho error handling
Có notes giải thích các bước quan trọng
```

**Bước 4:** Cursor generate → Copy → Test trên mermaid.live

---

## 📋 PROMPT 3: Generate Class Diagram

**Bước 1:** Tạo file mới: `exports\diagrams\domain-model.mmd`

**Bước 2:** Dùng Cursor AI với prompt:

```
Tạo class diagram Mermaid cho Domain Model của DynamicForm:

Classes và Attributes:

1. Form
   - Id: Guid
   - Code: string (unique)
   - Name: string
   - Description: string?
   - Status: int
   - CurrentVersionId: Guid?
   - CreatedDate: DateTime
   - CreatedBy: string

2. FormVersion
   - Id: Guid
   - FormId: Guid (FK)
   - Version: string
   - IsActive: bool
   - CreatedDate: DateTime
   - CreatedBy: string
   - ApprovedDate: DateTime?
   - ApprovedBy: string?
   - ChangeLog: string?

3. FormField
   - Id: Guid
   - FormVersionId: Guid (FK)
   - FieldCode: string
   - FieldType: int (1=Text, 2=Number, 3=Date, 6=Select, 10=TextArea)
   - Label: string
   - DisplayOrder: int
   - IsRequired: bool
   - IsVisible: bool
   - DefaultValue: string?
   - Placeholder: string?
   - HelpText: string?

4. FieldValidation
   - Id: Guid
   - FieldId: Guid (FK)
   - RuleType: int (1=Required, 2=Min, 3=Max, 4=Range, 5=Regex)
   - RuleValue: string?
   - ErrorMessage: string
   - Priority: int
   - IsActive: bool

5. FormData
   - Id: Guid
   - FormVersionId: Guid (FK)
   - ObjectId: string
   - ObjectType: string
   - DataJson: string (Dictionary<string, object>)
   - CreatedDate: DateTime
   - CreatedBy: string
   - ModifiedDate: DateTime?
   - ModifiedBy: string?
   - Status: int

Relationships:
- Form "1" --> "*" FormVersion
- FormVersion "1" --> "*" FormField
- FormVersion "1" --> "*" FormData
- FormField "1" --> "*" FieldValidation

Format: classDiagram với visibility (+, -, #) và relationships rõ ràng
```

---

## 📋 PROMPT 4: Tối ưu cho Presentation

**Bước 1:** Mở file diagram bất kỳ (ví dụ: `flowchart-1.mmd`)

**Bước 2:** Select code

**Bước 3:** Dùng Cursor AI với prompt:

```
Tối ưu sơ đồ Mermaid này để trình bày cho business lead:

1. Tăng font size và spacing
2. Làm rõ hierarchy (dùng subgraph)
3. Thêm styling đẹp với màu sắc
4. Đảm bảo dễ đọc khi export PNG 1920x1080
5. Thêm labels và descriptions ngắn gọn
6. Tối ưu layout để không bị chật

Code hiện tại:
[Select code]
```

---

## 📋 PROMPT 5: Generate State Diagram

**Bước 1:** Tạo file mới: `exports\diagrams\state-machine.mmd`

**Bước 2:** Dùng Cursor AI với prompt:

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

## 🎯 QUICK REFERENCE

### Shortcuts Cursor:
- **Ctrl+K** (Windows) / **Cmd+K** (Mac): Mở AI prompt
- **Ctrl+L** (Windows) / **Cmd+L** (Mac): Mở Chat panel
- **Tab**: Accept suggestion
- **Esc**: Cancel

### Workflow Nhanh:
```
1. Mở file .mmd trong Cursor
2. Ctrl+K
3. Paste prompt từ file này
4. Tab để accept
5. Copy code → mermaid.live → Export
```

---

## ✅ CHECKLIST SAU KHI GENERATE

- [ ] Code không có syntax errors
- [ ] Test trên mermaid.live thành công
- [ ] Màu sắc và styling đẹp
- [ ] Layout hợp lý
- [ ] Export PNG 1920x1080 OK
- [ ] Dễ đọc và hiểu

---

**Lưu ý:** Tất cả prompts này đã được test và hoạt động tốt với Cursor AI. Bạn chỉ cần copy-paste và chạy!
