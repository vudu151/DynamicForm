# Database Scripts

## Cách sử dụng

### Option 1: Chạy trực tiếp trong SQL Server Management Studio (SSMS)

1. Mở SQL Server Management Studio
2. Kết nối đến SQL Server (LocalDB hoặc SQL Server)
3. Mở file `CreateDatabase.sql`
4. Chạy script (F5)

### Option 2: Chạy từ Command Line

```bash
sqlcmd -S (localdb)\mssqllocaldb -i CreateDatabase.sql
```

Hoặc với SQL Server:

```bash
sqlcmd -S localhost -U sa -P YourPassword -i CreateDatabase.sql
```

### Option 3: Sử dụng Entity Framework Migrations

```bash
cd DynamicForm.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Lưu ý

- Script sẽ tự động tạo database `DynamicFormDb` nếu chưa tồn tại
- Tất cả các bảng, indexes, và foreign keys sẽ được tạo tự động
- Sample data được comment, uncomment nếu muốn thêm dữ liệu mẫu

## Cấu trúc Database

- **Forms**: Thông tin form chính
- **FormVersions**: Version của form
- **FormFields**: Các field trong form
- **FieldValidations**: Validation rules
- **FieldConditions**: Conditional logic
- **FieldOptions**: Options cho select/radio fields
- **FormData**: Dữ liệu đã điền
- **FormDataHistory**: Lịch sử thay đổi
- **FormPermissions**: Phân quyền
