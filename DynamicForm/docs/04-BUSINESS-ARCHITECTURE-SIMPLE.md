# TÀI LIỆU NGHIỆP VỤ - DYNAMIC FORM (HIS)

> **Mục tiêu**: Mô tả đơn giản, rõ ràng về nghiệp vụ DynamicForm theo 4 yêu cầu chính:
> 1. Mô hình metadata form
> 2. Cách render UI
> 3. Validation động
> 4. Versioning form

---

## I. BÀI TOÁN NGHIỆP VỤ

### 1.1. Vấn đề hiện tại

Trong HIS (Hospital Information System) có rất nhiều **biểu mẫu y tế**:
- Phiếu khám bệnh
- Phiếu chăm sóc
- Bệnh án
- Phiếu xét nghiệm
- Phiếu chỉ định

**Các biểu mẫu này thay đổi liên tục vì:**
- Mỗi khoa/phòng có yêu cầu khác nhau
- Bộ Y tế ban hành quy định mới (ví dụ: chuyển từ ICD-10 sang ICD-11)
- Cần tuân thủ tiêu chuẩn y tế quốc tế

**Vấn đề nếu KHÔNG có Dynamic Form:**

| Vấn đề | Hệ quả |
|--------|--------|
| Form hard-code | Mỗi thay đổi phải sửa code → mất thời gian |
| Không version | Dữ liệu cũ – mới lẫn lộn → không đảm bảo tính toàn vẹn |
| Validation cứng | Không phù hợp từng khoa → dữ liệu không chính xác |
| Không tái sử dụng | Trùng lặp form → tốn công sức |
| Không audit | Không biết ai sửa form → không tuân thủ |

### 1.2. Giải pháp Dynamic Form

**Dynamic Form giải quyết bằng cách:**
- ✅ **Tạo/sửa form không cần deploy lại** → Thích ứng nhanh
- ✅ **Quản lý version** → Đảm bảo dữ liệu lịch sử
- ✅ **Validation động** → Linh hoạt theo từng khoa
- ✅ **Tái sử dụng metadata** → Không trùng lặp

---

## II. 4 YÊU CẦU CHÍNH

### 2.1. Mô hình Metadata Form

**Metadata = Dữ liệu mô tả cấu trúc form**

Thay vì hard-code form, ta lưu **metadata** vào database:

```
Form (Phiếu khám)
  └── FormVersion (Version 1.0)
       └── FormField (Họ tên, Tuổi, Huyết áp...)
            ├── FieldType (Text, Number, Date...)
            ├── Validation Rules (Required, Range...)
            └── Options (nếu là Select)
```

**Ví dụ metadata:**
```json
{
  "Form": {
    "Code": "PHIEU_KHAM",
    "Name": "Phiếu khám bệnh"
  },
  "Fields": [
    {
      "FieldCode": "HO_TEN",
      "Label": "Họ và tên",
      "FieldType": 1,  // Text
      "IsRequired": true
    },
    {
      "FieldCode": "HUYET_AP",
      "Label": "Huyết áp",
      "FieldType": 2,  // Number
      "Validations": [
        {"RuleType": 4, "Min": 60, "Max": 200}  // Range
      ]
    }
  ]
}
```

**Lợi ích:**
- Thay đổi form chỉ cần sửa metadata, không cần sửa code
- Metadata có thể tái sử dụng cho nhiều form

---

### 2.2. Cách Render UI

**Render UI động = Tạo giao diện form từ metadata**

**Luồng hoạt động:**

```
1. User mở form (code: PHIEU_KHAM)
   ↓
2. Load metadata từ database
   ↓
3. Frontend render form theo FieldType:
   - FieldType = 1 (Text) → Input text
   - FieldType = 2 (Number) → Input number
   - FieldType = 3 (Date) → DatePicker
   - FieldType = 6 (Select) → Dropdown
   - FieldType = 10 (TextArea) → Textarea
   ↓
4. Bind validation rules vào các field
   ↓
5. Hiển thị form cho user
```

**Ví dụ render:**

| FieldType | UI Component |
|-----------|--------------|
| 1 - Text | `<input type="text">` |
| 2 - Number | `<input type="number">` |
| 3 - Date | `<input type="date">` |
| 6 - Select | `<select><option>...</option></select>` |
| 10 - TextArea | `<textarea></textarea>` |

**Lợi ích:**
- Frontend chỉ biết metadata, không biết nghiệp vụ y tế
- Thêm field mới → chỉ cần thêm metadata, không cần sửa code UI

---

### 2.3. Validation Động

**Validation động = Kiểm tra dữ liệu theo rules lưu trong metadata**

**Các loại validation:**

| RuleType | Mô tả | Ví dụ |
|----------|-------|-------|
| 1 - Required | Bắt buộc nhập | Họ tên phải có |
| 2 - Min | Giá trị tối thiểu | Tuổi >= 0 |
| 3 - Max | Giá trị tối đa | Tuổi <= 150 |
| 4 - Range | Khoảng giá trị | Huyết áp: 60-200 |
| 5 - Regex | Pattern | Số điện thoại: 0[0-9]{9} |

**Validation 2 tầng:**

```
1. Client-side validation (trên browser/app)
   → Phản hồi nhanh, UX tốt
   
2. Server-side validation (trên API)
   → An toàn, đảm bảo dữ liệu đúng
```

**Ví dụ validation:**

```json
{
  "FieldCode": "HUYET_AP",
  "Validations": [
    {
      "RuleType": 4,  // Range
      "Min": 60,
      "Max": 200,
      "ErrorMessage": "Huyết áp phải trong khoảng 60-200"
    }
  ]
}
```

**Lợi ích:**
- Validation cũng là metadata → không hard-code
- Dễ thay đổi validation theo từng khoa/phòng

---

### 2.4. Versioning Form

**Versioning = Quản lý phiên bản form theo thời gian**

**Tại sao cần versioning?**
- Bệnh án 2024 ≠ Bệnh án 2025 (có thể khác field)
- Không được phá dữ liệu cũ (phục vụ pháp lý)
- Cần xem lại dữ liệu cũ đúng với form lúc đó

**Nguyên tắc versioning:**

```
1. Immutable: Version cũ không được sửa
2. New version: Copy metadata từ version cũ → chỉnh sửa
3. Data gắn version: FormData phải gắn với FormVersionId cụ thể
```

**Ví dụ:**

```
Form: PHIEU_KHAM
  ├── Version 1.0 (2023)
  │    └── Fields: [Họ tên, Tuổi, Mã ICD-10]
  │
  └── Version 2.0 (2024) ← Active
       └── Fields: [Họ tên, Tuổi, Mã ICD-11]  // Thay đổi

FormData:
  ├── Data 1 → FormVersionId = 1.0 (dùng field ICD-10)
  └── Data 2 → FormVersionId = 2.0 (dùng field ICD-11)
```

**Khi xem dữ liệu cũ:**
- Load FormData → có FormVersionId = 1.0
- Load metadata của version 1.0
- Render form theo đúng version 1.0 (có field ICD-10, không có ICD-11)

**Lợi ích:**
- Đảm bảo tính toàn vẹn dữ liệu lịch sử
- Phục vụ pháp lý, kiểm tra sau này

---

## III. CÁC ĐỐI TƯỢNG NGHIỆP VỤ

### 3.1. Form (Biểu mẫu)
- **Mô tả**: 1 biểu mẫu nghiệp vụ (ví dụ: Phiếu khám, Bệnh án)
- **Ví dụ**: PHIEU_KHAM, BENH_AN

### 3.2. FormVersion (Phiên bản form)
- **Mô tả**: Phiên bản của form theo thời gian
- **Ví dụ**: PHIEU_KHAM v1.0, PHIEU_KHAM v2.0

### 3.3. FormField (Trường dữ liệu)
- **Mô tả**: 1 trường trong form (ví dụ: Họ tên, Tuổi)
- **Ví dụ**: HO_TEN (Text), TUOI (Number), NGAY_SINH (Date)

### 3.4. Validation Rule (Quy tắc kiểm tra)
- **Mô tả**: Luật kiểm tra dữ liệu nhập vào
- **Ví dụ**: Required, Range, Regex

### 3.5. FormData (Dữ liệu đã điền)
- **Mô tả**: Dữ liệu người dùng nhập vào form
- **Ví dụ**: {HO_TEN: "Nguyễn Văn A", TUOI: 25}

**🔑 Điểm quan trọng:**
- **Tách Form definition (metadata) và Form data** là mấu chốt
- Metadata quyết định cấu trúc, Data là dữ liệu thực tế

---

## IV. LUỒNG NGHIỆP VỤ CHÍNH

### 4.1. Tạo Form (Admin)

```
1. Admin tạo form mới (PHIEU_KHAM)
2. Định nghĩa metadata (fields, validation)
3. Tạo version 1.0 (trạng thái Draft)
4. Duyệt và kích hoạt version
5. Form sẵn sàng sử dụng
```

### 4.2. Điền Form (Bác sĩ/Điều dưỡng)

```
1. User mở form (code: PHIEU_KHAM)
2. Load metadata (version active)
3. Render form động từ metadata
4. User nhập dữ liệu
5. Validate (client + server)
6. Lưu FormData (gắn với FormVersionId)
```

### 4.3. Tạo Version Mới (Admin)

```
1. Admin tạo version mới (v2.0)
2. Copy metadata từ version cũ (v1.0)
3. Chỉnh sửa fields/validation
4. Kích hoạt version mới
5. Version cũ (v1.0) vẫn giữ nguyên, không sửa được
```

### 4.4. Xem Dữ liệu Cũ (Bác sĩ)

```
1. User xem bệnh án cũ (ObjectId: 12345)
2. Load FormData → có FormVersionId = v1.0
3. Load metadata của version v1.0
4. Render form theo đúng version v1.0
5. Hiển thị dữ liệu đúng với form lúc đó
```

---

## V. GIÁ TRỊ KINH DOANH

### 5.1. Giảm chi phí phát triển
- **Trước**: Mỗi form mới cần 2-4 tuần code
- **Sau**: Tạo form mới trong 2-4 giờ (chỉ cần cấu hình metadata)

### 5.2. Tăng tốc độ thay đổi
- **Trước**: Cập nhật form cần 1-2 tuần (sửa code, test, deploy)
- **Sau**: Cập nhật form trong 30 phút - 2 giờ (chỉ sửa metadata)

### 5.3. Đảm bảo compliance
- Versioning đảm bảo dữ liệu lịch sử không bị thay đổi
- Audit trail theo dõi mọi thay đổi

### 5.4. Linh hoạt
- Validation động phù hợp từng khoa/phòng
- Tái sử dụng metadata cho nhiều form

---

## VI. TÓM TẮT

**Dynamic Form giải quyết 4 vấn đề chính:**

1. ✅ **Metadata Form**: Lưu cấu trúc form vào database, không hard-code
2. ✅ **Render UI**: Tạo giao diện động từ metadata
3. ✅ **Validation động**: Kiểm tra dữ liệu theo rules lưu trong metadata
4. ✅ **Versioning**: Quản lý phiên bản form, đảm bảo dữ liệu lịch sử

**Kết quả:**
- Tạo/sửa form không cần deploy lại
- Thích ứng nhanh với thay đổi nghiệp vụ
- Đảm bảo tính toàn vẹn dữ liệu

---

**Tài liệu này tập trung vào nghiệp vụ, không có chi tiết kỹ thuật.**
