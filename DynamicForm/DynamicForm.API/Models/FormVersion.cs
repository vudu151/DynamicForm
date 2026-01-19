using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormVersion
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FormId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    public bool IsActive { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? ApprovedDate { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    [MaxLength(1000)]
    public string? ChangeLog { get; set; }

    // Navigation properties
    [ForeignKey("FormId")]
    public virtual Form Form { get; set; } = null!;

    public virtual ICollection<FormField> Fields { get; set; } = new List<FormField>();
    public virtual ICollection<FormData> FormData { get; set; } = new List<FormData>();
}
