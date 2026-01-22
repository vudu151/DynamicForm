# QUY TRÌNH NGHIỆP VỤ - DYNAMIC FORM

> **Mục tiêu**: Mô tả chi tiết các quy trình nghiệp vụ chính của hệ thống DynamicForm theo dạng Swimlane Diagram
>
> **Đối tượng**: Business Analyst, Product Owner, Developer, Tester

---

## 1. QUY TRÌNH TẠO VÀ THIẾT KẾ FORM

### 1.1. Mô tả

Quy trình này mô tả cách Admin tạo form mới, thiết kế các field, và kích hoạt version để người dùng có thể sử dụng.

### 1.2. Swimlane Diagram

```
┌─────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   Admin     │  Web UI      │  API         │  Database    │  Business    │
│  (Actor)    │  (Frontend)  │  (Backend)   │  (SQL)       │  Logic       │
├─────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│             │              │              │              │              │
│ [START]     │              │              │              │              │
│             │              │              │              │              │
│ Tạo form    │              │              │              │              │
│ mới         │              │              │              │              │
│ (Code,      │              │              │              │              │
│  Name,      │              │              │              │              │
│  Version)   │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ Gửi request  │              │              │              │
│             │ POST /api/   │              │              │              │
│             │ forms        │              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Kiểm tra     │              │              │
│             │              │ Form code    │              │              │
│             │              │ đã tồn tại?  │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ Query Forms  │              │
│             │              │              │ WHERE Code=  │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Form chưa    │              │              │
│             │              │ tồn tại?     │              │              │
│             │              │              │              │              │
│             │              │     │ YES    │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ INSERT Forms │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Tạo version  │              │              │
│             │              │ mới          │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ Kiểm tra     │              │
│             │              │              │ version đã   │              │
│             │              │              │ tồn tại?     │              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │ Query        │
│             │              │              │              │ FormVersions │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │ Version      │              │
│             │              │              │ trùng?       │              │
│             │              │              │              │              │
│             │              │              │     │ YES    │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │ Tự động tăng │              │
│             │              │              │ version      │              │
│             │              │              │ (1→2,        │              │
│             │              │              │  1.0.0→1.0.1)│              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │ INSERT       │
│             │              │              │              │ FormVersions │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về Form  │              │              │              │
│             │ + Version   │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Chuyển đến  │              │              │              │              │
│ màn Designer│              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Thiết kế    │              │              │              │              │
│ form        │              │              │              │              │
│ (Thêm field,│              │              │              │              │
│  sắp xếp,   │              │              │              │              │
│  validation)│              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ Gửi metadata │              │              │              │
│             │ PUT /api/    │              │              │              │
│             │ forms/       │              │              │              │
│             │ versions/    │              │              │              │
│             │ {id}/metadata│              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Validate     │              │              │
│             │              │ metadata     │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ BEGIN        │              │
│             │              │              │ TRANSACTION  │              │
│             │              │              │              │              │
│             │              │              │ DELETE       │              │
│             │              │              │ FormFields   │              │
│             │              │              │ (cũ)         │              │
│             │              │              │              │              │
│             │              │              │ INSERT       │              │
│             │              │              │ FormFields   │              │
│             │              │              │ (mới)        │              │
│             │              │              │              │              │
│             │              │              │ INSERT       │              │
│             │              │              │ FieldValidations│          │
│             │              │              │              │              │
│             │              │              │ INSERT       │              │
│             │              │              │ FieldOptions │              │
│             │              │              │              │              │
│             │              │              │ INSERT       │              │
│             │              │              │ FieldConditions│           │
│             │              │              │              │              │
│             │              │              │ UPDATE       │              │
│             │              │              │ FormVersions  │              │
│             │              │              │ (ChangeLog)  │              │
│             │              │              │              │              │
│             │              │              │ COMMIT       │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ metadata     │              │              │              │
│             │ đã lưu       │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Kích hoạt   │              │              │              │              │
│ version     │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ POST /api/   │              │              │              │
│             │ forms/       │              │              │              │
│             │ versions/    │              │              │              │
│             │ {id}/activate│              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Deactivate   │              │              │
│             │              │ các version  │              │              │
│             │              │ khác         │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ UPDATE       │              │
│             │              │              │ FormVersions │              │
│             │              │              │ SET Status=0 │              │
│             │              │              │ WHERE FormId │              │
│             │              │              │              │              │
│             │              │              │ UPDATE       │              │
│             │              │              │ FormVersions │              │
│             │              │              │ SET Status=1 │              │
│             │              │              │ WHERE Id=    │              │
│             │              │              │              │              │
│             │              │              │ UPDATE       │              │
│             │              │              │ Forms        │              │
│             │              │              │ SET          │              │
│             │              │              │ CurrentVersionId│          │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ success      │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ [END]       │              │              │              │              │
│ Form đã     │              │              │              │              │
│ sẵn sàng    │              │              │              │              │
│ sử dụng     │              │              │              │              │
│             │              │              │              │              │
└─────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

---

## 2. QUY TRÌNH ĐIỀN VÀ LƯU DỮ LIỆU FORM

### 2.1. Mô tả

Quy trình này mô tả cách người dùng (Doctor/Nurse) điền form, hệ thống validate và lưu dữ liệu vào database.

### 2.2. Swimlane Diagram

```
┌─────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   User      │  Web UI      │  API         │  Database    │  Validation │
│  (Actor)    │  (Frontend)  │  (Backend)   │  (SQL)       │  Engine     │
├─────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│             │              │              │              │              │
│ [START]     │              │              │              │              │
│             │              │              │              │              │
│ Mở form    │              │              │              │              │
│ để điền     │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ GET /api/    │              │              │              │
│             │ forms/code/  │              │              │              │
│             │ {code}/      │              │              │              │
│             │ metadata     │              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Lấy metadata │              │              │
│             │              │ của version  │              │              │
│             │              │ active       │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ Query       │              │
│             │              │              │ Forms +     │              │
│             │              │              │ FormVersions│              │
│             │              │              │ + FormFields│              │
│             │              │              │ + Validations│             │
│             │              │              │ + Options    │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ metadata     │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Render form │              │              │              │              │
│ động từ     │              │              │              │              │
│ metadata    │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Điền dữ     │              │              │              │              │
│ liệu vào    │              │              │              │              │
│ các field   │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Bấm nút     │              │              │              │              │
│ "Lưu"       │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ Validate     │              │              │              │
│             │ client-side  │              │              │              │
│             │ (optional)   │              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │              │              │              │
│             │ POST /api/   │              │              │              │
│             │ formdata     │              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Validate     │              │              │
│             │              │ dữ liệu      │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ Lấy metadata │              │
│             │              │              │ để validate  │              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │ Kiểm tra     │
│             │              │              │              │ Required     │
│             │              │              │              │ fields       │
│             │              │              │              │              │
│             │              │              │              │     │        │
│             │              │              │              │     ▼        │
│             │              │              │              │              │
│             │              │              │              │ Kiểm tra     │
│             │              │              │              │ Min/Max      │
│             │              │              │              │ values       │
│             │              │              │              │              │
│             │              │              │              │     │        │
│             │              │              │              │     ▼        │
│             │              │              │              │              │
│             │              │              │              │ Kiểm tra     │
│             │              │              │              │ Regex        │
│             │              │              │              │ patterns     │
│             │              │              │              │              │
│             │              │              │              │     │        │
│             │              │              │              │     ▼        │
│             │              │              │              │              │
│             │              │ Validation  │              │              │
│             │              │ pass?       │              │              │
│             │              │              │              │              │
│             │              │     │ NO    │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về lỗi   │              │              │              │
│             │ validation   │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Hiển thị    │              │              │              │              │
│ lỗi cho     │              │              │              │              │
│ user sửa    │              │              │              │              │
│             │              │              │              │              │
│             │              │     │ YES   │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Tạo         │              │              │
│             │              │ SubmissionId│              │              │
│             │              │ mới         │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ SELECT MAX   │              │
│             │              │              │ (SubmissionId)│             │
│             │              │              │ FROM         │              │
│             │              │              │ FormDataValues│            │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │ SubmissionId │              │
│             │              │              │ = MAX + 1    │              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │ BEGIN        │              │
│             │              │              │ TRANSACTION  │              │
│             │              │              │              │              │
│             │              │              │ INSERT      │              │
│             │              │              │ FormDataValues│            │
│             │              │              │ (cho mỗi     │              │
│             │              │              │ field)       │              │
│             │              │              │              │              │
│             │              │              │ COMMIT       │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ FormDataDto  │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Hiển thị    │              │              │              │              │
│ thông báo   │              │              │              │              │
│ thành công │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ [END]       │              │              │              │              │
│ Dữ liệu đã  │              │              │              │              │
│ được lưu    │              │              │              │              │
│             │              │              │              │              │
└─────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

---

## 3. QUY TRÌNH XEM VÀ SỬA DỮ LIỆU ĐÃ LƯU

### 3.1. Mô tả

Quy trình này mô tả cách người dùng xem và sửa dữ liệu form đã được lưu trước đó.

### 3.2. Swimlane Diagram

```
┌─────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   User      │  Web UI      │  API         │  Database    │  Business    │
│  (Actor)    │  (Frontend)  │  (Backend)   │  (SQL)       │  Logic       │
├─────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│             │              │              │              │              │
│ [START]     │              │              │              │              │
│             │              │              │              │              │
│ Chọn        │              │              │              │              │
│ submission  │              │              │              │              │
│ để xem/sửa  │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ GET /api/    │              │              │              │
│             │ formdata/    │              │              │              │
│             │ {submissionId}│             │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Lấy dữ liệu  │              │              │
│             │              │ theo         │              │              │
│             │              │ SubmissionId │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ SELECT *     │              │
│             │              │              │ FROM         │              │
│             │              │              │ FormDataValues│            │
│             │              │              │ WHERE        │              │
│             │              │              │ SubmissionId=│              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │ Group by     │              │              │
│             │              │ FieldCode    │              │              │
│             │              │ và tạo       │              │              │
│             │              │ Dictionary   │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ FormDataDto  │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Load        │              │              │              │              │
│ metadata    │              │              │              │              │
│ để render   │              │              │              │              │
│ form        │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Hiển thị    │              │              │              │              │
│ form với    │              │              │              │              │
│ dữ liệu     │              │              │              │              │
│ đã điền     │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Sửa đổi     │              │              │              │              │
│ dữ liệu     │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Bấm nút     │              │              │              │              │
│ "Lưu"       │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ PUT /api/    │              │              │              │
│             │ formdata/    │              │              │              │
│             │ {submissionId}│             │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Validate     │              │              │
│             │              │ dữ liệu      │              │              │
│             │              │ (giống 2.2)  │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ BEGIN        │              │
│             │              │              │ TRANSACTION  │              │
│             │              │              │              │              │
│             │              │              │ DELETE       │              │
│             │              │              │ FormDataValues│            │
│             │              │              │ WHERE        │              │
│             │              │              │ SubmissionId=│              │
│             │              │              │              │              │
│             │              │              │ INSERT      │              │
│             │              │              │ FormDataValues│            │
│             │              │              │ (dữ liệu mới)│              │
│             │              │              │              │              │
│             │              │              │ UPDATE      │              │
│             │              │              │ ModifiedDate,│              │
│             │              │              │ ModifiedBy   │              │
│             │              │              │              │              │
│             │              │              │ COMMIT       │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ FormDataDto  │              │              │              │
│             │ đã cập nhật  │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Hiển thị    │              │              │              │              │
│ thông báo   │              │              │              │              │
│ cập nhật    │              │              │              │              │
│ thành công │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ [END]       │              │              │              │              │
│ Dữ liệu đã  │              │              │              │              │
│ được cập    │              │              │              │              │
│ nhật        │              │              │              │              │
│             │              │              │              │              │
└─────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

---

## 4. QUY TRÌNH TẠO VERSION MỚI KHI FORM ĐÃ TỒN TẠI

### 4.1. Mô tả

Quy trình này mô tả cách hệ thống tự động tạo version mới khi user tạo form với code đã tồn tại.

### 4.2. Swimlane Diagram

```
┌─────────────┬──────────────┬──────────────┬──────────────┬──────────────┐
│   Admin     │  Web UI      │  API         │  Database    │  Business    │
│  (Actor)    │  (Frontend)  │  (Backend)   │  (SQL)       │  Logic       │
├─────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│             │              │              │              │              │
│ [START]     │              │              │              │              │
│             │              │              │              │              │
│ Tạo form    │              │              │              │              │
│ với code    │              │              │              │              │
│ đã tồn tại  │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│             │ POST /api/   │              │              │              │
│             │ forms        │              │              │              │
│             │              │              │              │              │
│             │     │        │              │              │              │
│             │     ▼        │              │              │              │
│             │              │ Kiểm tra     │              │              │
│             │              │ Form code    │              │              │
│             │              │ đã tồn tại?  │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ SELECT *     │              │
│             │              │              │ FROM Forms   │              │
│             │              │              │ WHERE Code=  │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Form đã      │              │              │
│             │              │ tồn tại?     │              │              │
│             │              │              │              │              │
│             │              │     │ YES    │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Không tạo    │              │              │
│             │              │ form mới     │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Lấy FormId   │              │              │
│             │              │ của form     │              │              │
│             │              │ đã tồn tại    │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ POST /api/   │              │              │
│             │              │ forms/       │              │              │
│             │              │ {formId}/    │              │              │
│             │              │ versions     │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │              │ Kiểm tra     │              │              │
│             │              │ version đã   │              │              │
│             │              │ tồn tại?     │              │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │ SELECT *     │              │
│             │              │              │ FROM         │              │
│             │              │              │ FormVersions │              │
│             │              │              │ WHERE        │              │
│             │              │              │ FormId= AND  │              │
│             │              │              │ Version=     │              │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │              │ Tự động tăng  │
│             │              │              │              │ version      │
│             │              │              │              │              │
│             │              │              │              │ - Nếu "1"    │
│             │              │              │              │   → tìm MAX │
│             │              │              │              │   → tạo "2"  │
│             │              │              │              │              │
│             │              │              │              │ - Nếu "1.0.0"│
│             │              │              │              │   → tìm      │
│             │              │              │              │   "1.0.x"    │
│             │              │              │              │   → tạo      │
│             │              │              │              │   "1.0.1"    │
│             │              │              │              │              │
│             │              │              │     │        │              │
│             │              │              │     ▼        │              │
│             │              │              │              │              │
│             │              │              │ INSERT      │              │
│             │              │              │ FormVersions │              │
│             │              │              │ (version    │              │
│             │              │              │ đã tăng)    │              │
│             │              │              │              │              │
│             │              │     │        │              │              │
│             │              │     ▼        │              │              │
│             │              │              │              │              │
│             │ Trả về       │              │              │              │
│             │ FormVersion  │              │              │              │
│             │ đã tạo       │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ Chuyển đến  │              │              │              │              │
│ Designer    │              │              │              │              │
│ với version │              │              │              │              │
│ mới         │              │              │              │              │
│             │              │              │              │              │
│     │       │              │              │              │              │
│     ▼       │              │              │              │              │
│             │              │              │              │              │
│ [END]       │              │              │              │              │
│ Version mới │              │              │              │              │
│ đã được     │              │              │              │              │
│ tạo         │              │              │              │              │
│             │              │              │              │              │
└─────────────┴──────────────┴──────────────┴──────────────┴──────────────┘
```

---

## 5. TÓM TẮT CÁC QUY TRÌNH

### 5.1. Quy trình 1: Tạo và Thiết kế Form

**Actor**: Admin  
**Mục đích**: Tạo form mới, thiết kế metadata, và kích hoạt để sử dụng

**Các bước chính**:
1. Admin tạo form mới (Code, Name, Version)
2. Hệ thống kiểm tra form code đã tồn tại
3. Nếu chưa tồn tại → Tạo form mới + version
4. Nếu đã tồn tại → Chỉ tạo version mới (tự động tăng nếu trùng)
5. Admin thiết kế form (thêm field, validation, options)
6. Lưu metadata vào database
7. Kích hoạt version → Form sẵn sàng sử dụng

### 5.2. Quy trình 2: Điền và Lưu Dữ liệu

**Actor**: User (Doctor/Nurse)  
**Mục đích**: Điền form và lưu dữ liệu vào database

**Các bước chính**:
1. User mở form để điền
2. Web UI gọi API lấy metadata (version active)
3. Render form động từ metadata
4. User điền dữ liệu
5. Validate dữ liệu (client-side + server-side)
6. Tạo SubmissionId mới (tự động tăng)
7. Lưu từng field value vào FormDataValues
8. Trả về kết quả thành công

### 5.3. Quy trình 3: Xem và Sửa Dữ liệu

**Actor**: User  
**Mục đích**: Xem và cập nhật dữ liệu đã lưu

**Các bước chính**:
1. User chọn submission để xem/sửa
2. Web UI gọi API lấy dữ liệu theo SubmissionId
3. Group các FormDataValue theo FieldCode
4. Load metadata để render form
5. Hiển thị form với dữ liệu đã điền
6. User sửa dữ liệu
7. Validate và cập nhật (DELETE + INSERT mới)
8. Trả về kết quả

### 5.4. Quy trình 4: Tạo Version mới (Form đã tồn tại)

**Actor**: Admin  
**Mục đích**: Tạo version mới cho form đã tồn tại

**Các bước chính**:
1. Admin tạo form với code đã tồn tại
2. Hệ thống phát hiện form đã tồn tại
3. Không tạo form mới, chỉ lấy FormId
4. Kiểm tra version đã tồn tại
5. Nếu trùng → Tự động tăng version (1→2, 1.0.0→1.0.1)
6. Tạo version mới với số version đã tăng
7. Chuyển đến Designer để thiết kế

---

## 6. CÁC ĐIỂM QUAN TRỌNG

### 6.1. Version Auto-increment

- Khi version trùng, hệ thống tự động tăng
- Hỗ trợ số đơn giản (1, 2, 3...) và semantic version (1.0.0, 1.0.1...)
- Logic nằm trong `FormService.GetNextVersionAsync()`

### 6.2. SubmissionId Management

- SubmissionId là INT tự quản lý, không có FK constraint
- Tự động tăng: `MAX(SubmissionId) + 1`
- Dùng để nhóm các FormDataValue của cùng 1 submission

### 6.3. Metadata Update Strategy

- Khi cập nhật metadata: DELETE tất cả fields cũ → INSERT fields mới
- Đảm bảo tính nhất quán và không có dữ liệu rác
- Sử dụng transaction để đảm bảo atomicity

### 6.4. Validation Flow

- Client-side validation (optional, UX tốt hơn)
- Server-side validation (bắt buộc, đảm bảo an toàn)
- Validation rules được lưu trong FieldValidations table
- Priority xác định thứ tự kiểm tra

---

**Tài liệu này mô tả đầy đủ các quy trình nghiệp vụ chính của hệ thống DynamicForm. Cập nhật lần cuối: 2024-01-21**
