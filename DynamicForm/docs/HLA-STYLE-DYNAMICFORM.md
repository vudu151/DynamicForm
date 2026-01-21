# DYNAMIC FORM MODULE – HLA STYLE DOCUMENT

> **Mục tiêu**: Tài liệu mô-đun Dynamic Form dùng cho thuyết trình/đánh giá (phong cách HLA-Connector), tập trung nghiệp vụ, luồng chính, và cách tích hợp.
>
> **Đối tượng đọc**: PO/BA, kiến trúc sư, tech lead, đội tích hợp.

---

## 1. Bối cảnh & Phạm vi
- HIS có nhiều phiếu (khám, chăm sóc, xét nghiệm…) thay đổi liên tục.
- Dynamic Form cung cấp **metadata-driven form**: khai báo, render động, validate động, versioning.
- Phạm vi: mô-đun DynamicForm (Web + API + Mobile), không đi sâu DB.

## 2. Actor & Vai trò
- **Admin**: Thiết kế form, tạo/kích hoạt version.
- **Doctor/Nurse**: Điền form (phiếu khám, chăm sóc).
- **Lab Tech**: Điền phiếu xét nghiệm.
- **System**: Lưu trữ/validate theo metadata, quản lý version.

## 3. Luồng nghiệp vụ chính

### 3.1. Thiết kế & kích hoạt form (Admin)
```mermaid
sequenceDiagram
    participant Admin
    participant Designer as Form Designer (UI)
    participant System

    Admin->>Designer: Tạo form mới (PHIEU_KHAM)
    Designer->>Designer: Khai báo field + validation (metadata)
    Designer->>System: Lưu metadata (Draft)
    Admin->>System: Duyệt & kích hoạt version
    System-->>Admin: Form active, sẵn sàng dùng
```

### 3.2. Điền form (Doctor/Nurse)
```mermaid
sequenceDiagram
    participant User as Bác sĩ/Điều dưỡng
    participant App as Web/Mobile
    participant System

    User->>App: Mở form PHIEU_KHAM
    App->>System: GET metadata (version active)
    System-->>App: Metadata (fields + rules)
    App-->>User: Render form động

    User->>App: Nhập dữ liệu, bấm Lưu
    App->>System: Validate + Submit
    alt Sai dữ liệu
        System-->>App: Trả lỗi từng field
        App-->>User: Hiển thị lỗi
    else Đúng dữ liệu
        System-->>App: Lưu thành công
        App-->>User: Thông báo OK
    end
```

### 3.3. Tạo version mới (Admin)
```mermaid
sequenceDiagram
    participant Admin
    participant System

    Admin->>System: Tạo version mới từ v1.0
    System->>System: Copy metadata v1.0 -> v2.0 (Draft)
    Admin->>System: Sửa field/validation
    Admin->>System: Kích hoạt v2.0
    System-->>Admin: v2.0 Active, v1.0 giữ nguyên (immutable)
```

### 3.4. Xem dữ liệu cũ theo version
```mermaid
sequenceDiagram
    participant Doctor
    participant App
    participant System

    Doctor->>App: Xem bệnh án (ObjectId: 12345)
    App->>System: GET FormData + FormVersionId
    System->>System: Load metadata của version tương ứng
    System-->>App: Data + Metadata đúng version
    App-->>Doctor: Hiển thị form theo version gốc
```

## 4. Nguyên tắc nghiệp vụ cốt lõi
- **Metadata-first**: Form/Field/Validation khai báo, không hard-code.
- **Validation hai tầng**: Client (UX) + Server (an toàn).
- **Versioning immutable**: Version cũ không sửa; Data gắn version cụ thể.
- **Reuse**: Metadata tái sử dụng giữa Web/Mobile.

## 5. Khả năng tích hợp
- **API mở**: GET metadata, POST formdata, POST validate.
- **Kênh truy cập**: Web (Razor) và Mobile (MAUI) cùng dùng một metadata.
- **Bảo mật**: dự kiến RBAC (doctor/nurse/admin), TLS, audit (ai/bao giờ/sửa gì).

## 6. KPI kỳ vọng (business)
- Thời gian tạo form mới: 2–4 giờ (thay vì 2–4 tuần).
- Cập nhật form: 30–120 phút (không deploy).
- Lỗi dữ liệu nhập: < 1% (nhờ validation động).
- Duy trì dữ liệu lịch sử: 100% (nhờ versioning).

## 7. Phụ lục: Block Mermaid sẵn dùng (copy-paste)

**Flow điền form đơn giản (3 bước):**
```mermaid
sequenceDiagram
    participant BS as Bác sĩ
    participant App as Ứng dụng
    participant System as Hệ thống
    BS->>App: Mở form
    App->>System: Lấy metadata
    System-->>App: Trả metadata
    App-->>BS: Render form
    BS->>App: Điền thông tin & Lưu
    App->>System: Validate & Lưu
    System-->>App: Thành công
    App-->>BS: \"Lưu thành công\"
```

**State machine Form/Version:**
```mermaid
stateDiagram-v2
    [*] --> Draft
    Draft --> Active : ActivateVersion
    Active --> Inactive : DeactivateForm\n(or ActivateOtherVersion)
    Inactive --> Active : ActivateVersion
```

---

**Tài liệu này giữ phong cách HLA-Connector: ngắn gọn, tập trung nghiệp vụ, kèm sơ đồ dễ nhìn.**

---

## 8. Vai trò & Trách nhiệm (RACI rút gọn)
| Hoạt động | Admin | Doctor/Nurse | Lab Tech | System |
|-----------|-------|--------------|----------|--------|
| Thiết kế form | R/A | C | C | I |
| Duyệt/Kích hoạt version | R/A | I | I | C |
| Điền form | I | R/A | R/A | C |
| Validate & Lưu | I | I | I | R/A |
| Xem dữ liệu cũ | I | R/A | R | C |

R: Responsible, A: Accountable, C: Consulted, I: Informed

## 9. Từ vựng nhanh (Glossary)
- **Metadata**: Cấu trúc form (fields, validation, option, condition).
- **FormVersion**: Phiên bản form, immutable sau khi tạo.
- **FormData**: Dữ liệu người dùng nhập, gắn với FormVersionId.
- **Validation hai tầng**: Client (UX) + Server (an toàn dữ liệu).
- **Active Version**: Version đang được dùng để tạo form mới.
- **Immutable**: Version cũ không được sửa; chỉ copy để tạo version mới.

## 10. Điều kiện tối thiểu (MVP)
- Tạo form + version, kích hoạt version.
|- Render động từ metadata (Web/Mobile).
|- Validation động tối thiểu: Required, Range, Regex.
|- Lưu FormData gắn với FormVersion.
|- Xem lại dữ liệu theo đúng version đã lưu.

## 11. Điểm nhấn thuyết trình
- “Không deploy lại khi đổi form” → metadata-first.
- “Dữ liệu lịch sử không vỡ” → versioning immutable.
- “Ít lỗi nhập liệu” → validation hai tầng.
- “Một metadata, nhiều kênh” → Web + Mobile cùng dùng.
