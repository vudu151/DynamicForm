# LUỒNG NGHIỆP VỤ (BUSINESS FLOWS) - DYNAMIC FORM

> **Mục đích**: Tập trung vào các luồng nghiệp vụ chính, không bao gồm chi tiết kỹ thuật/database

---

## 1. LUỒNG TẠO VÀ QUẢN LÝ FORM (Admin)

```mermaid
sequenceDiagram
    participant Admin as Quản trị viên
    participant Designer as Form Designer
    participant Validator as Validation Engine
    participant VersionMgr as Version Manager
    
    Admin->>Designer: Tạo form mới (PHIEU_KHAM)
    Designer->>Designer: Định nghĩa form metadata
    Designer->>Designer: Thêm các field (Text, Number, Date...)
    Designer->>Validator: Thiết lập validation rules
    Validator-->>Designer: Xác nhận rules hợp lệ
    Designer->>VersionMgr: Tạo version 1.0
    VersionMgr-->>Admin: Form đã tạo (trạng thái Draft)
    
    Note over Admin,VersionMgr: Form ở trạng thái Draft
    
    Admin->>VersionMgr: Duyệt và kích hoạt version
    VersionMgr-->>Admin: Form đã active, sẵn sàng sử dụng
```

---

## 2. LUỒNG ĐIỀN FORM - WEB (Bác sĩ/Điều dưỡng)

```mermaid
sequenceDiagram
    participant User as Bác sĩ/Điều dưỡng
    participant WebApp as Web Application
    participant API as Backend API
    participant Validator as Validation Engine
    
    User->>WebApp: Mở form (code: PHIEU_KHAM)
    WebApp->>API: Lấy form metadata (version active)
    API-->>WebApp: Trả về metadata (fields, validation)
    WebApp->>WebApp: Render form động từ metadata
    WebApp-->>User: Hiển thị form
    
    User->>User: Nhập dữ liệu vào form
    User->>WebApp: Bấm Submit
    
    WebApp->>WebApp: Validate client-side
    alt Validation fail (client)
        WebApp-->>User: Hiển thị lỗi ngay trên form
    else Validation pass
        WebApp->>API: Gửi dữ liệu để validate
        API->>Validator: Validate server-side
        Validator-->>API: Kết quả validation
        
        alt Validation fail (server)
            API-->>WebApp: Trả về lỗi chi tiết
            WebApp-->>User: Hiển thị lỗi theo từng field
        else Validation pass
            API->>API: Lưu dữ liệu form
            API-->>WebApp: Thành công (201 Created)
            WebApp-->>User: Thông báo "Lưu thành công"
        end
    end
```

---

## 3. LUỒNG ĐIỀN FORM - MOBILE (Bác sĩ/Điều dưỡng)

```mermaid
sequenceDiagram
    participant User as Bác sĩ/Điều dưỡng
    participant MobileApp as Mobile App
    participant API as Backend API
    participant Validator as Validation Engine
    
    User->>MobileApp: Mở app, chọn form
    MobileApp->>MobileApp: Hiển thị danh sách form
    User->>MobileApp: Chọn PHIEU_KHAM
    
    MobileApp->>API: Lấy form metadata
    API-->>MobileApp: Trả về metadata
    MobileApp->>MobileApp: Render form động (native controls)
    MobileApp-->>User: Hiển thị form native
    
    User->>User: Nhập dữ liệu (có thể tại giường bệnh)
    User->>MobileApp: Bấm Submit
    
    MobileApp->>API: Validate dữ liệu trước
    API->>Validator: Validate
    Validator-->>API: Kết quả
    
    alt Có lỗi validation
        API-->>MobileApp: Trả về lỗi theo field
        MobileApp-->>User: Hiển thị lỗi dưới từng field
    else Validation OK
        MobileApp->>API: Gửi dữ liệu để lưu
        API->>API: Lưu dữ liệu form
        API-->>MobileApp: Thành công
        MobileApp-->>User: Thông báo "Lưu thành công"
    end
```

---

## 4. LUỒNG TẠO VERSION MỚI (Admin)

```mermaid
sequenceDiagram
    participant Admin as Quản trị viên
    participant VersionMgr as Version Manager
    participant Designer as Form Designer
    
    Admin->>VersionMgr: Yêu cầu tạo version mới
    VersionMgr->>VersionMgr: Load version hiện tại (v1.0)
    VersionMgr->>VersionMgr: Copy metadata từ version cũ
    VersionMgr->>VersionMgr: Tạo version mới (v2.0)
    VersionMgr-->>Admin: Version mới đã tạo (Draft)
    
    Admin->>Designer: Chỉnh sửa fields/validation
    Note over Admin,Designer: Thêm field mới, sửa validation, xóa field cũ
    
    Designer->>VersionMgr: Lưu thay đổi
    VersionMgr-->>Admin: Version 2.0 đã cập nhật
    
    Note over VersionMgr: Version cũ (v1.0) vẫn giữ nguyên<br/>Data cũ gắn với v1.0
    
    Admin->>VersionMgr: Kích hoạt version mới
    VersionMgr->>VersionMgr: Set v2.0 IsActive = true<br/>Set v1.0 IsActive = false
    VersionMgr-->>Admin: Version 2.0 đã active
    
    Note over Admin,VersionMgr: Từ thời điểm này, form mới<br/>sẽ dùng version 2.0
```

---

## 5. LUỒNG XEM DỮ LIỆU FORM CŨ (Bác sĩ)

```mermaid
sequenceDiagram
    participant Doctor as Bác sĩ
    participant App as Web/Mobile App
    participant API as Backend API
    
    Doctor->>App: Xem bệnh án cũ (ObjectId: 12345)
    App->>API: Lấy dữ liệu form
    API->>API: Load FormData (có FormVersionId = v1.0)
    API->>API: Load metadata của version v1.0
    API->>API: Load Fields của version v1.0
    API-->>App: Trả về Data + Metadata (version cũ)
    
    App->>App: Render form theo đúng version cũ
    App-->>Doctor: Hiển thị form với dữ liệu cũ
    
    Note over Doctor,API: Form được render theo đúng<br/>schema của version khi dữ liệu được tạo<br/>(ví dụ: v1.0 có field "Mã ICD-10",<br/>v2.0 có "Mã ICD-11")
```

---

## 6. LUỒNG DEACTIVATE FORM (Admin)

```mermaid
sequenceDiagram
    participant Admin as Quản trị viên
    participant VersionMgr as Version Manager
    
    Admin->>VersionMgr: Yêu cầu deactivate form
    VersionMgr->>VersionMgr: Set tất cả versions IsActive = false
    VersionMgr->>VersionMgr: Set Form.Status = Inactive
    VersionMgr-->>Admin: Form đã deactivate
    
    Note over Admin,VersionMgr: Form không còn active<br/>Không thể tạo form data mới<br/>Nhưng vẫn xem được dữ liệu cũ
```

---

## TÓM TẮT CÁC LUỒNG NGHIỆP VỤ

| Luồng | Actor | Mục đích | Kết quả |
|-------|-------|----------|---------|
| **Tạo Form** | Admin | Tạo form mới với metadata | Form ở trạng thái Draft |
| **Kích hoạt Form** | Admin | Duyệt và active form | Form sẵn sàng sử dụng |
| **Điền Form (Web)** | Doctor/Nurse | Nhập dữ liệu qua web | Dữ liệu được lưu |
| **Điền Form (Mobile)** | Doctor/Nurse | Nhập dữ liệu qua mobile | Dữ liệu được lưu |
| **Tạo Version Mới** | Admin | Cập nhật form theo quy định mới | Version mới, version cũ giữ nguyên |
| **Xem Dữ liệu Cũ** | Doctor | Xem bệnh án/form cũ | Hiển thị đúng theo version cũ |
| **Deactivate Form** | Admin | Ngừng sử dụng form | Form không còn active |

---

## LƯU Ý

- ✅ Tập trung vào **business logic**, không có chi tiết database
- ✅ Dễ hiểu cho **business stakeholders**
- ✅ Có thể dùng để **presentation cho lead**
- ✅ Có thể **export PNG** từ mermaid.live để chèn vào PowerPoint

---

**File này chỉ chứa business flows, không có technical details.**
