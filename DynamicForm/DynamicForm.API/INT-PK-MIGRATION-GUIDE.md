# HƯỚNG DẪN: CHUYỂN SANG INT PK + GUID PublicId

## Ý TƯỞNG

- **INT làm Primary Key**: Tự động tăng (IDENTITY), query/report nhanh hơn
- **GUID làm PublicId**: Unique, indexed, dùng cho public API (không đoán được)
- **Foreign Keys**: Dùng INT (thay vì GUID) → join nhanh hơn

## LỢI ÍCH

1. **Performance**: 
   - Index nhỏ hơn (4 bytes vs 16 bytes)
   - Join nhanh hơn với INT
   - Foreign key hiệu quả hơn

2. **Bảo mật**: 
   - GUID không đoán được (phù hợp public API)
   - INT chỉ dùng nội bộ

3. **Báo cáo**: 
   - Query/aggregate nhanh hơn với INT
   - Index tốt hơn

## CẤU TRÚC MỚI

### Tất cả các bảng:
- `Id` (INT, PK, IDENTITY) - Dùng nội bộ
- `PublicId` (GUID, UNIQUE, INDEXED) - Dùng cho public API
- Foreign keys: INT (thay vì GUID)

### Ví dụ: Form
```csharp
public class Form
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  // PK nội bộ

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();  // Public API

    public int? CurrentVersionId { get; set; }  // FK (INT)
    // ...
}
```

## LOGIC MAPPING

### API Layer (Controllers)
- Nhận `PublicId` (Guid) từ client
- Pass `PublicId` vào Services

### Service Layer
- Convert `PublicId` → `Id` (int) khi query database
- Return `PublicId` trong DTOs

### Database Layer
- Lưu `Id` (INT) và `PublicId` (GUID)
- Foreign keys dùng `Id` (INT)

## VÍ DỤ

### Get Form By PublicId
```csharp
// API: GET /api/forms/{publicId}
public async Task<FormDto?> GetFormByIdAsync(Guid publicId)
{
    // Convert PublicId -> Id
    var form = await _context.Forms
        .FirstOrDefaultAsync(f => f.PublicId == publicId);
    
    return new FormDto
    {
        Id = form.PublicId,  // Return PublicId cho API
        // ...
    };
}
```

## SUBMISSION ID

FormDataValue.SubmissionId cần là INT để join nhanh. Có 2 cách:

### Cách 1: Dùng SEQUENCE (Khuyến nghị)
```sql
CREATE SEQUENCE SubmissionIdSequence AS INT START WITH 1;
```

### Cách 2: Query MAX + 1
```csharp
var maxSubmissionId = await _context.FormDataValues
    .Where(v => v.ObjectId == objectId && v.ObjectType == objectType && v.FormVersionId == formVersionId)
    .Select(v => v.SubmissionId)
    .DefaultIfEmpty(0)
    .MaxAsync();
var newSubmissionId = maxSubmissionId + 1;
```

## MIGRATION STEPS

1. ✅ Cập nhật Models: Id (int), PublicId (Guid)
2. ✅ Cập nhật ApplicationDbContext: Index PublicId, config IDENTITY
3. ⏳ Cập nhật Services: Map PublicId ↔ Id
4. ⏳ Cập nhật Controllers: Nhận PublicId từ API
5. ⏳ Tạo SQL migration script
