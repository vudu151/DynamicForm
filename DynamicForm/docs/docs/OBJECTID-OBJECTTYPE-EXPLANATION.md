# GIẢI THÍCH: ObjectId và ObjectType trong FormDataValues

## 🎯 MỤC ĐÍCH CHÍNH

`ObjectId` và `ObjectType` được dùng để **liên kết dữ liệu form với các đối tượng nghiệp vụ** trong hệ thống HIS (Hospital Information System).

## 📋 VÍ DỤ CỤ THỂ

### Ví dụ 1: Phiếu đăng ký khám bệnh

**Tình huống:**
- Bệnh nhân "Nguyễn Văn A" đăng ký khám bệnh
- Hệ thống tạo bản ghi `DangKyKham` với ID = "DK001"
- Bệnh nhân điền form "PHIEU_DANG_KY_KHAM_BENH"

**Khi lưu dữ liệu form:**
```json
{
  "formVersionId": "...",
  "objectId": "DK001",                    // ← ID của bản ghi DangKyKham
  "objectType": "PHIEU_DANG_KY_KHAM_BENH", // ← Có thể là Form.Code hoặc loại đối tượng
  "data": {
    "HO_TEN": "Nguyễn Văn A",
    "TUOI": 30,
    "SO_DIEN_THOAI": "0912345678"
  }
}
```

**Lưu ý:** Trong ví dụ API, `objectType` có giá trị `"PHIEU_DANG_KY_KHAM_BENH"` - giống với `Form.Code`. 
Có 2 cách hiểu:
1. **ObjectType = Form.Code** (đơn giản, dùng luôn Form.Code)
2. **ObjectType ≠ Form.Code** (ObjectType là loại đối tượng nghiệp vụ riêng như "DANG_KY_KHAM", "PHIEU_KHAM")

**Kết quả trong database:**
```
FormDataValues:
- SubmissionId: 1
- ObjectId: "DK001"
- ObjectType: "DANG_KY_KHAM"
- FormFieldId: 4 (HO_TEN)
- FieldValue: "Nguyễn Văn A"
- ...

FormDataValues:
- SubmissionId: 1
- ObjectId: "DK001"
- ObjectType: "DANG_KY_KHAM"
- FormFieldId: 5 (TUOI)
- FieldValue: "30"
- ...
```

### Ví dụ 2: Phiếu khám bệnh

**Tình huống:**
- Bác sĩ khám cho bệnh nhân với `KhamBenhId = "KB123"`
- Điền form "PHIEU_KHAM_BENH"

**Khi lưu dữ liệu:**
```json
{
  "objectId": "KB123",                    // ← ID của bản ghi KhamBenh
  "objectType": "PHIEU_KHAM_BENH",        // ← Có thể là Form.Code
  "data": {
    "CHAN_DOAN": "Cảm cúm",
    "TRIEU_CHUNG": "Sốt, ho",
    "DON_THUOC": "Paracetamol 500mg"
  }
}
```

### Ví dụ 3: Bệnh án

**Tình huống:**
- Bệnh nhân có `BenhAnId = "BA456"`
- Điền form "BENH_AN"

**Khi lưu dữ liệu:**
```json
{
  "objectId": "BA456",           // ← ID của bản ghi BenhAn
  "objectType": "BENH_AN",       // ← Có thể là Form.Code
  "data": {
    "TIEN_SU_BENH": "...",
    "KHAM_LAM_SANG": "..."
  }
}
```

## 🔗 TẠI SAO CẦN ObjectId và ObjectType?

### 1. **Tích hợp với hệ thống HIS**

DynamicForm là một module độc lập, nhưng cần tích hợp với các module khác:
- Module Quản lý Đăng ký khám
- Module Quản lý Khám bệnh
- Module Quản lý Bệnh án
- Module Quản lý Điều trị
- ...

### 2. **Truy vấn dữ liệu theo đối tượng**

**Câu hỏi:** "Lấy tất cả dữ liệu form của đăng ký khám DK001?"

**SQL Query:**
```sql
SELECT * FROM FormDataValues
WHERE ObjectId = 'DK001' 
  AND ObjectType = 'DANG_KY_KHAM'
```

**Câu hỏi:** "Lấy dữ liệu form mới nhất của bệnh nhân này?"

**SQL Query:**
```sql
SELECT * FROM FormDataValues
WHERE ObjectId = 'BN789' 
  AND ObjectType = 'BENH_NHAN'
ORDER BY CreatedDate DESC
```

### 3. **Một đối tượng có thể có nhiều form**

Ví dụ: Một `KhamBenhId = "KB123"` có thể có:
- Form "PHIEU_KHAM_BENH" (phiếu khám chính)
- Form "PHIEU_XET_NGHIEM" (phiếu xét nghiệm)
- Form "PHIEU_SIEU_AM" (phiếu siêu âm)

Tất cả đều có cùng `ObjectId = "KB123"` nhưng khác `ObjectType` hoặc `FormVersionId`.

## 📊 CẤU TRÚC DỮ LIỆU

### Bảng FormDataValues

```
SubmissionId | ObjectId | ObjectType      | FormVersionId | FormFieldId | FieldValue
-------------|----------|-----------------|---------------|-------------|------------
1            | DK001    | DANG_KY_KHAM    | v1            | HO_TEN      | "Nguyễn Văn A"
1            | DK001    | DANG_KY_KHAM    | v1            | TUOI        | "30"
1            | DK001    | DANG_KY_KHAM    | v1            | SDT         | "0912345678"
2            | KB123    | PHIEU_KHAM      | v2            | CHAN_DOAN   | "Cảm cúm"
2            | KB123    | PHIEU_KHAM      | v2            | TRIEU_CHUNG | "Sốt, ho"
```

### Index để tối ưu query

```sql
CREATE INDEX IX_FormDataValues_ObjectId_ObjectType_FormVersionId 
ON FormDataValues(ObjectId, ObjectType, FormVersionId);
```

Index này giúp query nhanh khi tìm dữ liệu theo đối tượng.

## 🎯 CÁC TRƯỜNG HỢP SỬ DỤNG

### 1. **Xem dữ liệu form từ đối tượng**

Khi xem chi tiết một `DangKyKham`, hệ thống có thể:
```csharp
// Lấy dữ liệu form liên quan
var formData = await _formDataService.GetByObjectAsync(
    objectId: "DK001",
    objectType: "DANG_KY_KHAM",
    formVersionId: versionId
);
```

### 2. **Hiển thị form trong context của đối tượng**

Khi bác sĩ khám bệnh:
- Mở màn hình khám bệnh với `KhamBenhId = "KB123"`
- Hệ thống tự động load form "PHIEU_KHAM_BENH"
- Khi lưu, tự động gán `ObjectId = "KB123"`, `ObjectType = "PHIEU_KHAM"`

### 3. **Báo cáo và thống kê**

```sql
-- Đếm số lượng form đã điền cho mỗi loại đối tượng
SELECT ObjectType, COUNT(DISTINCT SubmissionId) as TotalSubmissions
FROM FormDataValues
GROUP BY ObjectType;

-- Kết quả:
-- DANG_KY_KHAM: 150
-- PHIEU_KHAM: 200
-- BENH_AN: 50
```

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. **ObjectId không có Foreign Key**

- `ObjectId` là `NVARCHAR(100)`, không phải INT
- Không có Foreign Key constraint đến bảng khác
- Lý do: Các đối tượng có thể nằm ở các bảng/module khác nhau
- Hệ thống tự quản lý tính toàn vẹn dữ liệu

### 2. **ObjectType có thể là Form.Code hoặc loại đối tượng**

**Cách 1: ObjectType = Form.Code** (Đơn giản, khuyến nghị)
- `ObjectType = "PHIEU_DANG_KY_KHAM_BENH"` (giống Form.Code)
- Ưu điểm: Dễ hiểu, không cần mapping
- Nhược điểm: Phụ thuộc vào Form.Code

**Cách 2: ObjectType = Loại đối tượng nghiệp vụ** (Linh hoạt hơn)
- `ObjectType = "DANG_KY_KHAM"` (loại đối tượng, không phụ thuộc Form.Code)
- Ưu điểm: Một đối tượng có thể có nhiều form khác nhau
- Nhược điểm: Cần quản lý mapping

**Trong ví dụ API hiện tại:**
- `objectType: "PHIEU_DANG_KY_KHAM_BENH"` → Có vẻ như đang dùng **Form.Code**

**Các giá trị có thể:**
- Nếu dùng Form.Code: `PHIEU_DANG_KY_KHAM_BENH`, `PHIEU_KHAM_BENH`, `BENH_AN`, ...
- Nếu dùng loại đối tượng: `DANG_KY_KHAM`, `PHIEU_KHAM`, `BENH_AN`, `DIEU_TRI`, `XET_NGHIEM`, ...

### 3. **Kết hợp với SubmissionId**

- `SubmissionId`: Nhóm các field values của cùng 1 lần submit
- `ObjectId + ObjectType`: Liên kết với đối tượng nghiệp vụ
- Cả 2 đều cần thiết cho các mục đích khác nhau

## 📝 TÓM TẮT

| Trường | Mục đích | Ví dụ |
|--------|----------|-------|
| **ObjectId** | ID của đối tượng nghiệp vụ liên quan | "DK001", "KB123", "BA456" |
| **ObjectType** | Loại đối tượng hoặc Form.Code | "PHIEU_DANG_KY_KHAM_BENH" (Form.Code) hoặc "DANG_KY_KHAM" (loại đối tượng) |

**Mục đích chính:**
- ✅ Liên kết dữ liệu form với các đối tượng nghiệp vụ
- ✅ Tích hợp DynamicForm với các module khác trong hệ thống
- ✅ Truy vấn dữ liệu form theo đối tượng
- ✅ Hỗ trợ báo cáo và thống kê

---

**Cập nhật: 2024-01-21**
