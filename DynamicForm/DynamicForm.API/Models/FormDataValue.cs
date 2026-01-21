using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

/// <summary>
/// Lưu giá trị của từng field trong form.
/// Mỗi field value là 1 bản ghi riêng.
/// Các FormDataValue có cùng SubmissionId thuộc về cùng 1 lần submit form.
/// </summary>
public class FormDataValue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Public ID dùng cho API (GUID, unique, indexed)
    /// </summary>
    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// SubmissionId (INT) để nhóm các FormDataValue của cùng 1 lần submit form.
    /// Tự quản lý, không có FK constraint.
    /// </summary>
    [Required]
    public int SubmissionId { get; set; }

    [Required]
    public int FormVersionId { get; set; }

    [Required]
    public int FormFieldId { get; set; }

    /// <summary>
    /// ID của object liên quan (ví dụ: KhamBenhId, DieuTriId)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    /// Loại object (ví dụ: "KhamBenh", "DieuTri")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ObjectType { get; set; } = string.Empty;

    /// <summary>
    /// Giá trị của field (lưu dạng string, có thể parse theo FieldType)
    /// </summary>
    [MaxLength(4000)]
    public string? FieldValue { get; set; }

    /// <summary>
    /// Thứ tự hiển thị (dùng cho repeat section - MaxOccurs > 1)
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Mã section (dùng cho repeat section)
    /// </summary>
    [MaxLength(50)]
    public string? SectionCode { get; set; }

    /// <summary>
    /// Trạng thái: 0=Draft, 1=Submitted, 2=Approved
    /// </summary>
    public int Status { get; set; } = 0; // 0=Draft

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? ModifiedDate { get; set; }

    [MaxLength(100)]
    public string? ModifiedBy { get; set; }

    // Navigation properties
    // Note: SubmissionId không có FK, chỉ dùng để nhóm các values
    [ForeignKey("FormVersionId")]
    public virtual FormVersion FormVersion { get; set; } = null!;

    [ForeignKey("FormFieldId")]
    public virtual FormField FormField { get; set; } = null!;
}
