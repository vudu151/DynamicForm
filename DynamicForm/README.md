# Hệ thống Dynamic Form cho HIS

Hệ thống quản lý form động cho Hospital Information System (HIS) với Backend API và Frontend Razor Pages.

## Cấu trúc Project

```
DynamicForm/
├── DynamicForm.API/          # Backend API (ASP.NET Core Web API)
├── DynamicForm.Web/          # Frontend (ASP.NET Core Razor Pages)
└── docs/                     # Tài liệu thiết kế
```

## Tài liệu kỹ thuật (khuyến nghị đọc theo thứ tự)

- `docs/07-TECHNICAL-DESIGN-DYNAMICFORM.md` – **Tài liệu thiết kế kỹ thuật (TDD)** + sơ đồ (C4/ERD/Sequence/Deployment/State)
- `docs/02-SYSTEM-ARCHITECTURE.md` – Sơ đồ kiến trúc hệ thống (tham khảo mở rộng)
- `docs/docs/03-DATABASE-ERD.md` – ERD chi tiết database
- `docs/06-ARCHITECTURE-RECOMMENDATION-HIS.md` – Khuyến nghị kiến trúc cho HIS

## Yêu cầu

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code

## Cài đặt và Chạy

### 1. Tạo Database

```bash
# Tạo migration
cd DynamicForm.API
dotnet ef migrations add InitialCreate

# Cập nhật database
dotnet ef database update
```

Hoặc sử dụng Package Manager Console trong Visual Studio:
```
Add-Migration InitialCreate
Update-Database
```

### 2. Chạy Backend API

```bash
cd DynamicForm.API
dotnet run
```

API sẽ chạy tại: `https://localhost:7000` (hoặc port được cấu hình)

Swagger UI: `https://localhost:7000/swagger`

### 3. Chạy Frontend Web

```bash
cd DynamicForm.Web
dotnet run
```

Web sẽ chạy tại: `https://localhost:5000` (hoặc port được cấu hình)

### 4. Cấu hình Connection String

Cập nhật connection string trong `DynamicForm.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DynamicFormDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Cập nhật API URL trong `DynamicForm.Web/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7000"
  }
}
```

## API Endpoints

### Forms
- `GET /api/forms` - Lấy danh sách tất cả forms
- `GET /api/forms/{id}` - Lấy form theo ID
- `GET /api/forms/code/{code}` - Lấy form theo code
- `GET /api/forms/code/{code}/metadata` - Lấy metadata của form (bao gồm fields, validations)
- `POST /api/forms` - Tạo form mới
- `POST /api/forms/{formId}/versions` - Tạo version mới cho form
- `POST /api/forms/versions/{versionId}/activate` - Kích hoạt version

### Form Data
- `GET /api/formdata/{id}` - Lấy form data theo ID
- `GET /api/formdata/object/{objectId}/{objectType}` - Lấy form data theo object
- `POST /api/formdata` - Tạo form data mới
- `PUT /api/formdata/{id}` - Cập nhật form data
- `POST /api/formdata/validate` - Validate form data

## Tính năng

### ✅ Đã hoàn thành
- [x] Models và Entities (Form, FormVersion, FormField, FieldValidation, etc.)
- [x] Database Context với Entity Framework Core
- [x] API Controllers (Forms, FormData)
- [x] Services (FormService, FormDataService)
- [x] Validation động
- [x] Razor Pages (Danh sách form, Điền form)
- [x] Dynamic form rendering từ metadata

### 🚧 Cần phát triển thêm
- [ ] Form Builder UI (Tạo/sửa form từ giao diện)
- [ ] Version management UI
- [ ] Authentication & Authorization
- [ ] Field conditions (Show/Hide fields based on conditions)
- [ ] Form permissions
- [ ] Audit logging
- [ ] Export/Import form data

## Database Schema

Xem chi tiết trong `docs/docs/03-DATABASE-ERD.md`

Các bảng chính:
- `Form` - Thông tin form
- `FormVersion` - Version của form
- `FormField` - Các field trong form
- `FieldValidation` - Validation rules
- `FormData` - Dữ liệu đã điền
- `FormDataHistory` - Lịch sử thay đổi

## Tài liệu

- [Kiến trúc hệ thống](docs/02-SYSTEM-ARCHITECTURE.md)
- [Business Architecture](docs/04-BUSINESS-ARCHITECTURE.md)
- [Database ERD](docs/docs/03-DATABASE-ERD.md)
- [Ý tưởng bổ sung](docs/05-ADDITIONAL-IDEAS.md)
- [Khuyến nghị kiến trúc](docs/06-ARCHITECTURE-RECOMMENDATION-HIS.md)

## Cách sử dụng

1. **Tạo Form từ API** (hoặc dùng Swagger UI):
   ```json
   POST /api/forms
   {
     "code": "PHIEU_KHAM",
     "name": "Phiếu Khám Bệnh",
     "description": "Phiếu khám bệnh chuẩn",
     "status": 0,
     "createdBy": "admin"
   }
   ```

2. **Tạo Version và Fields** (sẽ có UI sau)

3. **Truy cập Web App**: Mở `https://localhost:5000/Forms` để xem danh sách form và điền form

## License

MIT
