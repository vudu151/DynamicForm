using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class Form
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int Status { get; set; } = 0; // 0=Draft, 1=Active, 2=Inactive

    public Guid? CurrentVersionId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? ModifiedDate { get; set; }

    [MaxLength(100)]
    public string? ModifiedBy { get; set; }

    // Navigation properties
    [ForeignKey("CurrentVersionId")]
    public virtual FormVersion? CurrentVersion { get; set; }

    public virtual ICollection<FormVersion> Versions { get; set; } = new List<FormVersion>();
    public virtual ICollection<FormPermission> Permissions { get; set; } = new List<FormPermission>();
}
