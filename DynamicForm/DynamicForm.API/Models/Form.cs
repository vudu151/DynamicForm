using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class Form
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
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int Status { get; set; } = 0; // 0=Draft, 1=Active, 2=Inactive

    public int? CurrentVersionId { get; set; }

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
}
