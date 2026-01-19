using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FormVersionId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ObjectId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ObjectType { get; set; } = string.Empty;

    [Required]
    public string DataJson { get; set; } = "{}";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime? ModifiedDate { get; set; }

    [MaxLength(100)]
    public string? ModifiedBy { get; set; }

    public int Status { get; set; } = 0; // 0=Draft, 1=Submitted, 2=Approved

    // Navigation properties
    [ForeignKey("FormVersionId")]
    public virtual FormVersion FormVersion { get; set; } = null!;

    public virtual ICollection<FormDataHistory> History { get; set; } = new List<FormDataHistory>();
}
