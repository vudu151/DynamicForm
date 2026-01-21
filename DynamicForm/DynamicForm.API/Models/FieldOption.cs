using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FieldOption
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
    [MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [Required]
    public int DisplayOrder { get; set; }

    public bool IsDefault { get; set; } = false;

    // Navigation properties
    [ForeignKey("FieldId")]
    public virtual FormField Field { get; set; } = null!;
}
