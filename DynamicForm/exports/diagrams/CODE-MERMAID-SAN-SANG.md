# CODE MERMAID SẴN SÀNG - PASTE VÀO MERMAID.AI

> **Cách dùng**: Copy code → Paste vào https://mermaid.ai/app/projects/ → Export

---

## CODE 1: Luồng Điền Form Đơn Giản (Đề xuất dùng)

```mermaid
sequenceDiagram
    participant BS as Bác sĩ
    participant App as Ứng dụng
    participant System as Hệ thống
    
    BS->>App: 1. Mở form phiếu khám
    App->>System: 2. Lấy thông tin form
    System-->>App: 3. Trả về form
    App-->>BS: 4. Hiển thị form
    
    BS->>BS: 5. Nhập thông tin bệnh nhân
    BS->>App: 6. Bấm Lưu
    
    App->>System: 7. Kiểm tra dữ liệu
    
    alt Dữ liệu đúng
        System->>System: 8. Lưu thành công
        System-->>App: 9. Thông báo thành công
        App-->>BS: 10. "Đã lưu thành công!"
    else Dữ liệu sai
        System-->>App: 11. Báo lỗi
        App-->>BS: 12. Hiển thị lỗi
        BS->>BS: 13. Sửa lại thông tin
        BS->>App: 14. Bấm Lưu lại
        App->>System: 15. Kiểm tra lại
        System->>System: 16. Lưu thành công
        System-->>App: 17. Thông báo thành công
        App-->>BS: 18. "Đã lưu thành công!"
    end
```

---

## CODE 2: Luồng Tạo Form (Đơn giản)

```mermaid
sequenceDiagram
    participant Admin as Quản trị viên
    participant System as Hệ thống
    
    Admin->>System: 1. Tạo form mới
    System-->>Admin: 2. Form đã tạo (Draft)
    
    Admin->>Admin: 3. Thiết kế các trường<br/>(Họ tên, Tuổi, Huyết áp...)
    Admin->>System: 4. Lưu thiết kế
    
    Admin->>Admin: 5. Thiết lập quy tắc kiểm tra<br/>(Bắt buộc, Khoảng giá trị...)
    Admin->>System: 6. Lưu quy tắc
    
    Admin->>System: 7. Kích hoạt form
    System->>System: 8. Form chuyển sang Active
    System-->>Admin: 9. "Form sẵn sàng sử dụng"
    
    Note over Admin,System: Form có thể được sử dụng<br/>bởi bác sĩ, điều dưỡng...
```

---

## CODE 3: Luồng Cực Kỳ Đơn Giản (3 bước)

```mermaid
sequenceDiagram
    participant BS as Bác sĩ
    participant App as Ứng dụng
    participant System as Hệ thống
    
    BS->>App: 1. Mở form
    App->>System: Lấy form
    System-->>App: Trả về form
    App-->>BS: Hiển thị form
    
    BS->>App: 2. Điền thông tin và Lưu
    App->>System: Gửi dữ liệu
    System->>System: Lưu dữ liệu
    System-->>App: Thành công
    App-->>BS: 3. "Lưu thành công!"
```

---

## CODE 4: Business Architecture Tổng Thể (Đơn giản)

```mermaid
graph TB
    subgraph "NGƯỜI DÙNG"
        Admin[Quản trị viên<br/>Tạo/sửa form]
        Doctor[Bác sĩ<br/>Điền phiếu khám]
        Nurse[Điều dưỡng<br/>Điền phiếu chăm sóc]
    end
    
    subgraph "KÊNH TRUY CẬP"
        Web[Web Application]
        Mobile[Mobile App]
    end
    
    subgraph "QUY TRÌNH NGHIỆP VỤ"
        Design[Thiết kế Form]
        Fill[Điền Form]
        Review[Xem Form]
    end
    
    Admin --> Web
    Admin --> Design
    
    Doctor --> Web
    Doctor --> Mobile
    Doctor --> Fill
    Doctor --> Review
    
    Nurse --> Web
    Nurse --> Mobile
    Nurse --> Fill
    
    Web --> Fill
    Mobile --> Fill
```

---

## CODE 5: Luồng Tạo Version Mới (Đơn giản)

```mermaid
sequenceDiagram
    participant Admin as Quản trị viên
    participant System as Hệ thống
    
    Admin->>System: 1. Yêu cầu tạo version mới
    System->>System: 2. Copy version cũ (v1.0)
    System->>System: 3. Tạo version mới (v2.0)
    System-->>Admin: 4. Version mới đã tạo (Draft)
    
    Admin->>Admin: 5. Chỉnh sửa form<br/>(Thêm/sửa/xóa trường)
    Admin->>System: 6. Lưu thay đổi
    
    Admin->>System: 7. Kích hoạt version mới
    System->>System: 8. v2.0 = Active<br/>v1.0 = Inactive
    System-->>Admin: 9. "Version mới đã active"
    
    Note over Admin,System: Version cũ (v1.0) vẫn giữ nguyên<br/>Dữ liệu cũ vẫn gắn với v1.0
```

---

## CÁCH SỬ DỤNG

### Bước 1: Chọn code phù hợp
- Copy code Mermaid từ trên (bắt đầu từ ````mermaid` đến ````)

### Bước 2: Paste vào Mermaid.ai
1. Mở https://mermaid.ai/app/projects/
2. Tạo project mới hoặc mở project có sẵn
3. Paste code vào editor
4. Diagram sẽ tự động render

### Bước 3: Export
- Click nút Export → Chọn PNG/SVG/PDF
- Download và chèn vào PowerPoint

---

## GỢI Ý CHO THUYẾT TRÌNH

**Luồng đơn giản nhất để bắt đầu:**
- Dùng **CODE 3** (3 bước) - Cực kỳ đơn giản
- Hoặc **CODE 1** (Luồng điền form) - Đầy đủ hơn nhưng vẫn dễ hiểu

**Nếu cần tổng quan:**
- Dùng **CODE 4** (Business Architecture) - Tổng quan hệ thống

---

**Tất cả code đã sẵn sàng, chỉ cần copy-paste!**
