# Hướng dẫn dùng AI để Generate Mermaid Diagrams

## Cách 1: Dùng ChatGPT/Claude để generate Mermaid code

### Prompt mẫu:

```
Tôi cần bạn tạo sơ đồ Mermaid cho Business Architecture của hệ thống DynamicForm.

Yêu cầu:
1. Sơ đồ tổng thể Business Architecture với:
   - Business Actors: Admin, Doctor, Nurse, Lab Tech
   - Business Processes: Form Design, Version Management, Form Filling, Form Review
   - Business Entities: Form, Field, Validation, Data
   - Access Channels: Web, Mobile
   
2. Format: Mermaid flowchart (graph TB)
3. Có màu sắc và nhóm rõ ràng
4. Có mũi tên kết nối giữa các thành phần

Hãy trả về code Mermaid đầy đủ.
```

### Sau khi có code, paste vào:
- **Mermaid Live Editor**: https://mermaid.live
- Hoặc dùng script `Export-MermaidDiagrams.ps1` để export

---

## Cách 2: Dùng Cursor AI (nếu bạn đang dùng Cursor)

1. Mở file markdown trong Cursor
2. Chọn đoạn mermaid code
3. Dùng Cursor AI (Ctrl+K) với prompt:
   ```
   Cải thiện sơ đồ mermaid này để dễ hiểu hơn, thêm màu sắc và labels rõ ràng
   ```
4. Cursor sẽ suggest code mới

---

## Cách 3: Dùng Draw.io với AI

1. Mở https://app.diagrams.net
2. Vào **Extras > Plugins** → Enable "AI Assistant"
3. Hoặc dùng **Arrange > Insert > Advanced > Mermaid**
4. Paste mermaid code → Draw.io sẽ render

---

## Cách 4: Dùng script tự động (Recommended)

### Chạy script extract diagrams:

```powershell
cd "D:\ONENET\5.Test Performance\DynamicForm\DynamicForm"
.\scripts\Export-MermaidDiagrams.ps1 -InputFile "docs\04-BUSINESS-ARCHITECTURE.md" -OutputDir "exports\diagrams"
```

### Sau đó:
1. Mở file `.html` trong browser để preview
2. Copy code từ file `.mmd` 
3. Paste vào **Mermaid Live Editor** (https://mermaid.live)
4. Export PNG/SVG/PDF

---

## Cách 5: Dùng Mermaid CLI (tự động export PNG)

### Cài đặt:
```bash
npm install -g @mermaid-js/mermaid-cli
```

### Export tất cả diagrams:
```bash
cd exports\diagrams
mmdc -i *.mmd -o *.png
```

### Hoặc export từng file:
```bash
mmdc -i flowchart-1.mmd -o flowchart-1.png -w 1920 -H 1080
```

---

## Tips để trình bày cho Lead:

1. **Export PNG với độ phân giải cao** (1920x1080 hoặc cao hơn)
2. **Chèn vào PowerPoint/Google Slides** với animation
3. **Dùng Draw.io** để chỉnh sửa thêm (thêm icons, màu sắc)
4. **Tạo slide deck** với:
   - Slide 1: Business Architecture tổng thể
   - Slide 2: Luồng Form Design
   - Slide 3: Luồng Form Filling (Web)
   - Slide 4: Luồng Form Filling (Mobile)
   - Slide 5: Domain Model
   - Slide 6: Business Rules

---

## Quick Start (Recommended Workflow):

```powershell
# Bước 1: Extract diagrams
.\scripts\Export-MermaidDiagrams.ps1 -InputFile "docs\04-BUSINESS-ARCHITECTURE.md"

# Bước 2: Mở exports\diagrams\*.html trong browser để preview

# Bước 3: Copy code từ .mmd file, paste vào https://mermaid.live

# Bước 4: Export PNG/SVG từ Mermaid Live Editor

# Bước 5: Chèn vào PowerPoint/Google Slides
```
