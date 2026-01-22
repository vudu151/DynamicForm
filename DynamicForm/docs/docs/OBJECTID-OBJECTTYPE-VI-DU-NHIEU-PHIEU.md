# VÍ DỤ: Bệnh nhân có nhiều phiếu chăm sóc điều dưỡng

## 🎯 TÌNH HUỐNG

**Bệnh nhân A** có **3 phiếu chăm sóc điều dưỡng cấp 1**:
- Phiếu 1: Ngày 01/01/2024 - Ca sáng
- Phiếu 2: Ngày 01/01/2024 - Ca chiều  
- Phiếu 3: Ngày 02/01/2024 - Ca sáng

## 📋 CÁCH MAPPING

### Cách 1: Mỗi phiếu chăm sóc là một đối tượng riêng (Khuyến nghị)

**Giả sử hệ thống có bảng `PhieuChamSoc` với các ID:**
- `PhieuChamSocId = "PCS001"` (Phiếu 1)
- `PhieuChamSocId = "PCS002"` (Phiếu 2)
- `PhieuChamSocId = "PCS003"` (Phiếu 3)

**Khi lưu dữ liệu form:**

#### Phiếu 1:
```json
{
  "formVersionId": "...",
  "objectId": "PCS001",                    // ← ID của phiếu chăm sóc 1
  "objectType": "PHIEU_CHAM_SOC",          // ← Loại đối tượng
  "data": {
    "NGAY_CHAM_SOC": "2024-01-01",
    "CA": "Sáng",
    "NHIET_DO": "37.5",
    "HUYET_AP": "120/80"
  }
}
```

#### Phiếu 2:
```json
{
  "objectId": "PCS002",                    // ← ID của phiếu chăm sóc 2
  "objectType": "PHIEU_CHAM_SOC",
  "data": {
    "NGAY_CHAM_SOC": "2024-01-01",
    "CA": "Chiều",
    "NHIET_DO": "37.2",
    "HUYET_AP": "118/78"
  }
}
```

#### Phiếu 3:
```json
{
  "objectId": "PCS003",                    // ← ID của phiếu chăm sóc 3
  "objectType": "PHIEU_CHAM_SOC",
  "data": {
    "NGAY_CHAM_SOC": "2024-01-02",
    "CA": "Sáng",
    "NHIET_DO": "37.0",
    "HUYET_AP": "115/75"
  }
}
```

**Kết quả trong database:**
```
FormDataValues:
┌──────────────┬──────────┬───────────────┬──────────────┬─────────────┐
│ SubmissionId │ ObjectId │ ObjectType    │ FormFieldId  │ FieldValue  │
├──────────────┼──────────┼───────────────┼──────────────┼─────────────┤
│ 1            │ PCS001   │ PHIEU_CHAM_SOC│ NGAY_CHAM_SOC│ "2024-01-01"│
│ 1            │ PCS001   │ PHIEU_CHAM_SOC│ CA           │ "Sáng"      │
│ 1            │ PCS001   │ PHIEU_CHAM_SOC│ NHIET_DO     │ "37.5"      │
│ 2            │ PCS002   │ PHIEU_CHAM_SOC│ NGAY_CHAM_SOC│ "2024-01-01"│
│ 2            │ PCS002   │ PHIEU_CHAM_SOC│ CA           │ "Chiều"     │
│ 2            │ PCS002   │ PHIEU_CHAM_SOC│ NHIET_DO     │ "37.2"      │
│ 3            │ PCS003   │ PHIEU_CHAM_SOC│ NGAY_CHAM_SOC│ "2024-01-02"│
│ 3            │ PCS003   │ PHIEU_CHAM_SOC│ CA           │ "Sáng"      │
│ 3            │ PCS003   │ PHIEU_CHAM_SOC│ NHIET_DO     │ "37.0"      │
└──────────────┴──────────┴───────────────┴──────────────┴─────────────┘
```

### Cách 2: ObjectId = ID bệnh nhân (Không khuyến nghị)

**Nếu dùng ObjectId = ID bệnh nhân:**
```json
{
  "objectId": "BN001",                     // ← ID của bệnh nhân
  "objectType": "PHIEU_CHAM_SOC",
  "data": { ... }
}
```

**Vấn đề:**
- ❌ Không phân biệt được 3 phiếu chăm sóc khác nhau
- ❌ Không biết phiếu nào là phiếu nào
- ❌ Khó query và quản lý

## 🔍 TRUY VẤN DỮ LIỆU

### Lấy tất cả phiếu chăm sóc của bệnh nhân A

**Nếu bảng `PhieuChamSoc` có `BenhNhanId`:**
```sql
-- Bước 1: Lấy danh sách ID phiếu chăm sóc của bệnh nhân
SELECT Id FROM PhieuChamSoc WHERE BenhNhanId = 'BN001'

-- Bước 2: Lấy dữ liệu form của các phiếu đó
SELECT * FROM FormDataValues
WHERE ObjectId IN ('PCS001', 'PCS002', 'PCS003')
  AND ObjectType = 'PHIEU_CHAM_SOC'
ORDER BY CreatedDate DESC
```

### Lấy dữ liệu của một phiếu chăm sóc cụ thể

```sql
SELECT * FROM FormDataValues
WHERE ObjectId = 'PCS001'
  AND ObjectType = 'PHIEU_CHAM_SOC'
ORDER BY DisplayOrder
```

### Lấy phiếu chăm sóc mới nhất của bệnh nhân

```sql
-- Giả sử có bảng PhieuChamSoc với BenhNhanId
SELECT TOP 1 fdv.*
FROM FormDataValues fdv
INNER JOIN PhieuChamSoc pcs ON fdv.ObjectId = pcs.Id
WHERE pcs.BenhNhanId = 'BN001'
  AND fdv.ObjectType = 'PHIEU_CHAM_SOC'
ORDER BY fdv.CreatedDate DESC
```

## 📊 CẤU TRÚC DỮ LIỆU ĐỀ XUẤT

### Bảng PhieuChamSoc (trong hệ thống HIS)

```
PhieuChamSoc:
┌────────────┬──────────────┬──────────────┬──────────────┐
│ Id         │ BenhNhanId   │ NgayChamSoc  │ Ca           │
├────────────┼──────────────┼──────────────┼──────────────┤
│ PCS001     │ BN001        │ 2024-01-01   │ Sáng         │
│ PCS002     │ BN001        │ 2024-01-01   │ Chiều        │
│ PCS003     │ BN001        │ 2024-01-02   │ Sáng         │
└────────────┴──────────────┴──────────────┴──────────────┘
```

### Mapping với FormDataValues

```
FormDataValues:
- ObjectId = PhieuChamSoc.Id (PCS001, PCS002, PCS003)
- ObjectType = "PHIEU_CHAM_SOC"
- FormVersionId = ID của form "PHIEU_CHAM_SOC_DIEU_DUONG_CAP_1"
```

## ✅ KẾT LUẬN

**Cách mapping đúng:**
- ✅ **ObjectId** = ID của từng phiếu chăm sóc (PCS001, PCS002, PCS003)
- ✅ **ObjectType** = "PHIEU_CHAM_SOC" (loại đối tượng)
- ✅ Mỗi phiếu chăm sóc là một đối tượng riêng với ID riêng

**Lợi ích:**
- ✅ Phân biệt rõ ràng từng phiếu chăm sóc
- ✅ Dễ query và quản lý
- ✅ Có thể lấy dữ liệu của từng phiếu riêng biệt
- ✅ Có thể lấy tất cả phiếu của một bệnh nhân thông qua bảng PhieuChamSoc

---

**Cập nhật: 2024-01-21**
