using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FieldValidation
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
    public int FieldId { get; set; }

    [Required]
    public int RuleType { get; set; } // 1=Required, 2=Min, 3=Max, 4=Range, 5=Regex, etc.

    [MaxLength(500)]
    public string? RuleValue { get; set; }

    [Required]
    [MaxLength(500)]
    public string ErrorMessage { get; set; } = string.Empty;

    public int Priority { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("FieldId")]
    public virtual FormField Field { get; set; } = null!;
}
