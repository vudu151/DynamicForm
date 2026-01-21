# HƯỚNG DẪN DÙNG CURSOR AI ĐỂ GENERATE MẸRMAID DIAGRAMS

## 🎯 Mục tiêu
Sử dụng Cursor AI để:
- ✅ Generate Mermaid diagrams từ mô tả
- ✅ Cải thiện diagrams hiện có
- ✅ Thêm màu sắc và styling
- ✅ Tối ưu cho presentation

---

## 📋 BƯỚC 1: Chuẩn bị

### 1.1. Mở file markdown trong Cursor
```
1. Mở Cursor
2. Mở file: docs\04-BUSINESS-ARCHITECTURE.md
3. Tìm đến phần có mermaid diagram (ví dụ: ```mermaid ... ```)
```

### 1.2. Hoặc tạo file mới để test
```
1. Tạo file mới: exports\diagrams\new-diagram.mmd
2. Paste code mermaid hiện có (nếu có)
3. Hoặc để trống nếu muốn generate từ đầu
```

---

## 🚀 BƯỚC 2: Generate Diagram Mới với Cursor AI

### Cách 1: Generate từ mô tả (Recommended)

**Bước 1:** Tạo file mới hoặc mở file `.mmd`

**Bước 2:** Dùng Cursor AI (Ctrl+K hoặc Cmd+K trên Mac)

**Bước 3:** Nhập prompt:

```
Tạo sơ đồ Mermaid flowchart cho Business Architecture của hệ thống DynamicForm với:

1. Business Actors:
   - Admin: Quản trị viên, tạo/sửa form, quản lý version
   - Doctor: Bác sĩ, điền phiếu khám, xem bệnh án
   - Nurse: Điều dưỡng, điền phiếu chăm sóc
   - Lab Tech: Kỹ thuật viên, điền phiếu xét nghiệm

2. Access Channels:
   - Web: Razor Pages, Desktop/Tablet
   - Mobile: Android/iOS, Offline capable

3. Business Processes:
   - Form Design: Thiết kế metadata, cấu hình field
   - Version Management: Tạo version, kích hoạt
   - Form Filling: Điền form, validate
   - Form Review: Xem dữ liệu, export

4. Business Entities:
   - Form, Field, Validation, Data

5. Business Rules:
   - Versioning, Validation, Permission, Audit

Yêu cầu:
- Format: graph TB (top to bottom)
- Có subgraph để nhóm logic
- Có mũi tên kết nối rõ ràng
- Code Mermaid đầy đủ, có thể copy paste vào mermaid.live
```

**Bước 4:** Cursor sẽ generate code Mermaid → Copy code → Paste vào file

---

### Cách 2: Cải thiện Diagram Hiện Có

**Bước 1:** Mở file có diagram (ví dụ: `exports\diagrams\flowchart-1.mmd`)

**Bước 2:** Chọn toàn bộ code Mermaid

**Bước 3:** Dùng Cursor AI (Ctrl+K) với prompt:

```
Cải thiện sơ đồ Mermaid này để:
1. Dễ hiểu hơn cho business stakeholders
2. Thêm màu sắc cho các nhóm (subgraph)
3. Làm rõ mối quan hệ giữa các thành phần
4. Tối ưu layout để presentation đẹp hơn
5. Thêm labels và descriptions rõ ràng

Code hiện tại:
[Code sẽ tự động được chọn]
```

**Bước 4:** Cursor sẽ suggest code mới → Accept hoặc chỉnh sửa

---

## 🎨 BƯỚC 3: Thêm Màu Sắc và Styling

### Prompt mẫu cho Cursor:

```
Thêm styling và màu sắc vào sơ đồ Mermaid này:

1. Business Actors: Màu xanh dương (#3498db)
2. Access Channels: Màu xanh lá (#2ecc71)
3. Business Processes: Màu cam (#e67e22)
4. Business Entities: Màu tím (#9b59b6)
5. Business Rules: Màu đỏ (#e74c3c)

Sử dụng classDef và class trong Mermaid để apply màu.
Code hiện tại:
[Select code]
```

### Hoặc prompt đơn giản hơn:

```
Thêm màu sắc đẹp vào sơ đồ này, mỗi subgraph một màu khác nhau để dễ phân biệt
```

---

## 📊 BƯỚC 4: Generate Sequence Diagram

### Prompt cho Sequence Diagram:

```
Tạo sequence diagram Mermaid cho luồng điền form trên Mobile:

Actors:
- Doctor (Bác sĩ)
- MobileApp
- API
- ValidationEngine
- Database

Luồng:
1. Doctor mở app, chọn form
2. MobileApp gọi API để load metadata
3. API query database
4. MobileApp render form động
5. Doctor nhập dữ liệu
6. MobileApp validate client-side
7. MobileApp gọi API validate
8. API validate server-side
9. Nếu OK: lưu vào database
10. Trả về kết quả

Format: sequenceDiagram với các participant rõ ràng
```

---

## 🔄 BƯỚC 5: Generate Class Diagram

### Prompt cho Class Diagram:

```
Tạo class diagram Mermaid cho Domain Model của DynamicForm:

Classes:
1. Form (Id, Code, Name, Status, CurrentVersionId)
2. FormVersion (Id, FormId, Version, IsActive)
3. FormField (Id, FieldCode, FieldType, Label, IsRequired)
4. FieldValidation (Id, RuleType, RuleValue, ErrorMessage)
5. FormData (Id, FormVersionId, ObjectId, DataJson)
6. FormPermission (Id, FormId, RoleCode, CanView, CanEdit)

Relationships:
- Form 1--* FormVersion
- FormVersion 1--* FormField
- FormField 1--* FieldValidation
- FormVersion 1--* FormData
- Form 1--* FormPermission

Format: classDiagram với relationships rõ ràng
```

---

## 💡 BƯỚC 6: Tối ưu cho Presentation

### Prompt để tối ưu:

```
Tối ưu sơ đồ Mermaid này để trình bày cho lead:

1. Tăng kích thước font
2. Thêm spacing giữa các node
3. Làm rõ hierarchy và flow
4. Thêm annotations nếu cần
5. Đảm bảo dễ đọc khi export PNG 1920x1080

Code hiện tại:
[Select code]
```

---

## 🎯 WORKFLOW HOÀN CHỈNH

### Workflow 1: Generate Diagram Mới

```
1. Tạo file mới: exports\diagrams\my-diagram.mmd
2. Mở file trong Cursor
3. Ctrl+K → Nhập prompt mô tả diagram
4. Cursor generate → Review code
5. Copy code → Paste vào https://mermaid.live
6. Export PNG/SVG
7. Chèn vào PowerPoint
```

### Workflow 2: Cải thiện Diagram Có Sẵn

```
1. Mở file: exports\diagrams\flowchart-1.mmd
2. Select toàn bộ code
3. Ctrl+K → Prompt: "Cải thiện sơ đồ này..."
4. Review changes → Accept
5. Test trên mermaid.live
6. Export và update
```

### Workflow 3: Batch Generate Nhiều Diagrams

```
1. Tạo file: exports\diagrams\all-diagrams.md
2. List tất cả diagrams cần generate
3. Dùng Cursor AI để generate từng cái
4. Copy vào file riêng
5. Export tất cả
```

---

## 📝 PROMPT TEMPLATES

### Template 1: Business Architecture

```
Tạo sơ đồ Mermaid flowchart cho [Tên] với:
- Actors: [List actors]
- Processes: [List processes]
- Entities: [List entities]
- Rules: [List rules]
- Format: graph TB với subgraph
- Có màu sắc và styling đẹp
```

### Template 2: Sequence Diagram

```
Tạo sequence diagram cho [Luồng] với:
- Participants: [List]
- Steps: [List steps]
- Format: sequenceDiagram
- Có alt/opt blocks nếu cần
```

### Template 3: Class Diagram

```
Tạo class diagram cho [Domain] với:
- Classes: [List với attributes]
- Relationships: [List relationships]
- Format: classDiagram
- Có visibility và methods nếu cần
```

---

## ⚡ TIPS & TRICKS

### 1. Dùng Multi-line Selection
- Select nhiều dòng code
- Ctrl+K → Prompt sẽ áp dụng cho tất cả

### 2. Dùng Cursor Chat (Ctrl+L)
- Mở chat panel
- Hỏi: "Làm sao để tạo sequence diagram trong Mermaid?"
- Cursor sẽ hướng dẫn và generate

### 3. Iterative Improvement
- Generate lần 1: Basic structure
- Generate lần 2: Add colors
- Generate lần 3: Optimize layout
- Generate lần 4: Add annotations

### 4. Combine với Mermaid Live Editor
- Generate trong Cursor
- Test trên mermaid.live
- Fine-tune trong Cursor
- Export từ mermaid.live

---

## 🐛 TROUBLESHOOTING

### Cursor không generate Mermaid code?
```
Prompt: "Generate Mermaid diagram code for [description], return only the code block starting with ```mermaid"
```

### Code không render đúng?
```
1. Copy code từ Cursor
2. Paste vào mermaid.live để test
3. Nếu lỗi, prompt Cursor: "Fix syntax errors in this Mermaid code: [paste code]"
```

### Diagram quá phức tạp?
```
Prompt: "Simplify this Mermaid diagram, keep only essential elements for business presentation"
```

---

## 📚 VÍ DỤ THỰC TẾ

### Ví dụ 1: Generate Business Architecture

**File:** `exports\diagrams\business-arch.mmd`

**Prompt trong Cursor:**
```
Tạo sơ đồ Business Architecture cho DynamicForm với:
- Actors: Admin, Doctor, Nurse, Lab Tech
- Channels: Web, Mobile
- Processes: Design, Versioning, Filling, Review
- Entities: Form, Field, Data
- Rules: Version, Validation, Permission, Audit

Format: graph TB, có subgraph, có màu sắc
```

**Kết quả:** Cursor generate code → Copy → Test trên mermaid.live → Export

### Ví dụ 2: Cải thiện Sequence Diagram

**File:** `exports\diagrams\sequence-3.mmd` (đã có sẵn)

**Prompt trong Cursor:**
```
Cải thiện sequence diagram này:
1. Thêm error handling (alt blocks)
2. Thêm notes giải thích
3. Làm rõ data flow
4. Tối ưu cho presentation

[Select existing code]
```

**Kết quả:** Cursor suggest improvements → Accept → Export

---

## ✅ CHECKLIST

Trước khi export:
- [ ] Code Mermaid đã test trên mermaid.live
- [ ] Không có syntax errors
- [ ] Màu sắc và styling đẹp
- [ ] Layout hợp lý cho presentation
- [ ] Labels và descriptions rõ ràng
- [ ] Export resolution: 1920x1080 hoặc cao hơn

---

## 🎓 NEXT STEPS

1. **Practice:** Generate 2-3 diagrams với Cursor
2. **Test:** Export và xem trên mermaid.live
3. **Refine:** Dùng Cursor để cải thiện
4. **Present:** Chèn vào PowerPoint/Google Slides

**Happy diagramming! 🚀**
