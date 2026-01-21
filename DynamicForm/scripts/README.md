# Scripts để Export Mermaid Diagrams

## Quick Start

### Cách 1: Chạy script tự động (Recommended)

```powershell
cd "D:\ONENET\5.Test Performance\DynamicForm\DynamicForm"
.\scripts\Quick-ExportDiagrams.ps1
```

Script sẽ:
- ✅ Extract tất cả Mermaid diagrams từ `docs\04-BUSINESS-ARCHITECTURE.md`
- ✅ Tạo file `.mmd` (Mermaid code) và `.html` (preview)
- ✅ Mở thư mục `exports\diagrams` tự động

### Cách 2: Export thủ công

```powershell
.\scripts\Export-MermaidDiagrams.ps1 -InputFile "docs\04-BUSINESS-ARCHITECTURE.md" -OutputDir "exports\diagrams"
```

---

## Export PNG/SVG để trình bày

### Option 1: Dùng Mermaid Live Editor (Dễ nhất, có AI)

1. Mở https://mermaid.live
2. Copy code từ file `.mmd` trong `exports\diagrams`
3. Paste vào editor
4. Click **Actions > PNG/SVG/PDF** để export
5. Download và chèn vào PowerPoint

### Option 2: Dùng Mermaid CLI (Tự động)

```bash
# Cài đặt (chỉ cần 1 lần)
npm install -g @mermaid-js/mermaid-cli

# Export tất cả diagrams
cd exports\diagrams
mmdc -i *.mmd -o *.png -w 1920 -H 1080
```

### Option 3: Dùng Draw.io với AI

1. Mở https://app.diagrams.net
2. **File > Import from > Device** → Chọn file `.mmd`
3. Hoặc **Arrange > Insert > Advanced > Mermaid** → Paste code
4. Draw.io sẽ render và bạn có thể chỉnh sửa
5. Export PNG/SVG/PDF

---

## Dùng AI để Generate/Cải thiện Diagrams

### Với ChatGPT/Claude:

**Prompt mẫu:**
```
Tôi có sơ đồ Mermaid này, hãy cải thiện để:
1. Dễ hiểu hơn cho business stakeholders
2. Thêm màu sắc và styling
3. Nhóm các thành phần logic hơn

[Paste code Mermaid từ file .mmd]
```

### Với Cursor AI:

1. Mở file `.mmd` trong Cursor
2. Chọn code
3. Dùng Cursor AI (Ctrl+K):
   ```
   Cải thiện sơ đồ này để presentation cho lead dễ hiểu
   ```

---

## File Structure

```
exports\diagrams\
├── flowchart-1.mmd      # Business Architecture tổng thể
├── flowchart-1.html     # Preview trong browser
├── sequence-2.mmd       # Luồng tạo form
├── sequence-2.html
├── sequence-3.mmd       # Luồng điền form (Web)
├── sequence-3.html
├── sequence-4.mmd       # Luồng điền form (Mobile)
├── sequence-4.html
├── sequence-5.mmd       # Luồng versioning
├── sequence-5.html
├── sequence-6.mmd       # Luồng xem dữ liệu cũ
├── sequence-6.html
├── class-7.mmd          # Domain Model
├── class-7.html
├── flowchart-8.mmd      # Business Constraints
├── flowchart-8.html
├── flowchart-9.mmd      # Stakeholder Map
└── flowchart-9.html
```

---

## Tips để trình bày cho Lead

1. **Export PNG với độ phân giải cao** (1920x1080)
2. **Tạo slide deck**:
   - Slide 1: Business Architecture tổng thể (flowchart-1)
   - Slide 2: Luồng Form Design (sequence-2)
   - Slide 3: Luồng Form Filling Web (sequence-3)
   - Slide 4: Luồng Form Filling Mobile (sequence-4)
   - Slide 5: Domain Model (class-7)
   - Slide 6: Stakeholder Map (flowchart-9)

3. **Dùng Draw.io** để:
   - Thêm icons
   - Chỉnh màu sắc theo brand
   - Thêm annotations
   - Tạo animation (nếu cần)

4. **Export format**:
   - PNG: Cho PowerPoint/Google Slides
   - SVG: Cho web/documentation (có thể scale)
   - PDF: Cho in ấn

---

## Troubleshooting

**Lỗi: "Execution policy"**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Lỗi: "Mermaid CLI not found"**
```bash
npm install -g @mermaid-js/mermaid-cli
```

**Diagrams không render trong HTML:**
- Cần internet để load Mermaid từ CDN
- Hoặc download Mermaid locally và update HTML
