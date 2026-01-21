using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormVersion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Public ID dùng cho API (GUID, unique, indexed)
    /// </summary>
    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public int FormId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Status: 0=Draft, 1=Published, 2=Archived
    /// </summary>
    public int Status { get; set; } = 0; // 0=Draft

    /// <summary>
    /// Deprecated: Use Status instead. Kept for backward compatibility.
    /// </summary>
    [Obsolete("Use Status property instead")]
    public bool IsActive { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Date when version was published (Status = Published)
    /// </summary>
    public DateTime? PublishedDate { get; set; }

    [MaxLength(100)]
    public string? PublishedBy { get; set; }

    /// <summary>
    /// Deprecated: Use PublishedDate instead
    /// </summary>
    [Obsolete("Use PublishedDate instead")]
    public DateTime? ApprovedDate { get; set; }

    /// <summary>
    /// Deprecated: Use PublishedBy instead
    /// </summary>
    [Obsolete("Use PublishedBy instead")]
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    [MaxLength(1000)]
    public string? ChangeLog { get; set; }

    // Navigation properties
    [ForeignKey("FormId")]
    public virtual Form Form { get; set; } = null!;

    public virtual ICollection<FormField> Fields { get; set; } = new List<FormField>();

    /// <summary>
    /// Helper property: Check if version is Published
    /// </summary>
    public bool IsPublished => Status == 1;

    /// <summary>
    /// Helper property: Check if version is Draft
    /// </summary>
    public bool IsDraft => Status == 0;
}
