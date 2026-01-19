# ✅ Database Setup Hoàn Tất

## Kết quả

Database `DynamicFormDb` đã được tạo thành công với:

- ✅ **1 Form**: PHIEU_KHAM (Phiếu Khám Bệnh)
- ✅ **1 Version**: 1.0.0 (Active)
- ✅ **6 Fields**: 
  - HO_TEN (Họ và Tên) - Text, Required
  - TUOI (Tuổi) - Number, Required, Range 0-150
  - GIOI_TINH (Giới tính) - Select, Required
  - NGAY_KHAM (Ngày khám) - Date, Required
  - HUYET_AP (Huyết áp) - Number, Range 50-250
  - CHAN_DOAN (Chẩn đoán) - TextArea
- ✅ **3 Options** cho field GIOI_TINH
- ✅ **5 Validations** rules
- ✅ **1 Sample Form Data**

## Cách chạy lại script

### Option 1: Chạy script PowerShell (Tự động)
```powershell
cd DynamicForm.API\Scripts
powershell -ExecutionPolicy Bypass -File SetupDatabase.ps1
```

### Option 2: Chạy từng script SQL
```bash
# Tạo schema
sqlcmd -S "(localdb)\mssqllocaldb" -f 65001 -b -i CreateDatabase.sql

# Thêm sample data
sqlcmd -S "(localdb)\mssqllocaldb" -d "DynamicFormDb" -f 65001 -b -i InsertSampleData.sql
```

### Option 3: Sử dụng SSMS
1. Mở SQL Server Management Studio
2. Kết nối đến `(localdb)\mssqllocaldb`
3. Mở và chạy `CreateDatabase.sql`
4. Mở và chạy `InsertSampleData.sql`

## Kiểm tra Database

```sql
-- Kiểm tra Forms
SELECT * FROM Forms;

-- Kiểm tra FormVersions
SELECT * FROM FormVersions;

-- Kiểm tra FormFields
SELECT * FROM FormFields ORDER BY DisplayOrder;

-- Kiểm tra FormData
SELECT * FROM FormData;
```

## Fix lỗi tiếng Việt bị "KhÃ¡m Bá»‡nh" (nếu đã lỡ insert sai)

Nếu dữ liệu đã bị lưu sai (do chạy `sqlcmd` không ép UTF-8), bạn có 2 cách:

- **Cách 1 (khuyến nghị)**: Drop + tạo lại DB rồi chạy lại `SetupDatabase.ps1` (đã ép `-f 65001`).
- **Cách 2 (nhanh cho sample PHIEU_KHAM)**: chạy script fix:

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -d "DynamicFormDb" -f 65001 -b -i FixSampleVietnamese.sql
```

## Test API

Sau khi chạy API, bạn có thể test:

1. **Lấy danh sách forms:**
   ```
   GET https://localhost:7000/api/forms
   ```

2. **Lấy metadata của form:**
   ```
   GET https://localhost:7000/api/forms/code/PHIEU_KHAM/metadata
   ```

3. **Xem Swagger UI:**
   ```
   https://localhost:7000/swagger
   ```

## Test Web App

1. Chạy Web app:
   ```bash
   cd DynamicForm.Web
   dotnet run
   ```

2. Truy cập:
   ```
   https://localhost:5000/Forms
   ```

3. Click "Điền Form" để test form PHIEU_KHAM

## Lưu ý

- Database đã có sample data sẵn
- Có thể xóa và tạo lại bằng cách chạy lại script
- Connection string trong `appsettings.json` đã được cấu hình đúng
- Nếu thấy **tiếng Việt bị lỗi kiểu** `KhÃ¡m Bá»‡nh` / `Phiáº¿u...` thì nguyên nhân thường là chạy `sqlcmd` không ép UTF-8. Hãy chạy lại bằng `SetupDatabase.ps1` (đã ép `-f 65001`) hoặc thêm `-f 65001` như ở Option 2, rồi **tạo lại DB / insert lại dữ liệu**.
