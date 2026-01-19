using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormDataHistory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FormDataId { get; set; }

    [Required]
    public string DataJson { get; set; } = "{}";

    public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string ChangedBy { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ChangeReason { get; set; }

    // Navigation properties
    [ForeignKey("FormDataId")]
    public virtual FormData FormData { get; set; } = null!;
}
