using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormField
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
    public int FormVersionId { get; set; }

    [Required]
    [MaxLength(50)]
    public string FieldCode { get; set; } = string.Empty;

    [Required]
    public int FieldType { get; set; } // 1=Text, 2=Number, 3=Date, 4=Select, etc.

    [Required]
    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [Required]
    public int DisplayOrder { get; set; }

    public bool IsRequired { get; set; } = false;

    public bool IsVisible { get; set; } = true;

    [MaxLength(500)]
    public string? DefaultValue { get; set; }

    [MaxLength(200)]
    public string? Placeholder { get; set; }

    [MaxLength(500)]
    public string? HelpText { get; set; }

    [MaxLength(200)]
    public string? CssClass { get; set; }

    public string? PropertiesJson { get; set; } // JSON for dynamic properties

    public int? ParentFieldId { get; set; } // For nested/repeater fields

    public int? MinOccurs { get; set; }

    public int? MaxOccurs { get; set; }

    /// <summary>
    /// Mã section dùng cho repeat section / nhóm field.
    /// </summary>
    [MaxLength(50)]
    public string? SectionCode { get; set; }

    // Navigation properties
    [ForeignKey("FormVersionId")]
    public virtual FormVersion FormVersion { get; set; } = null!;

    [ForeignKey("ParentFieldId")]
    public virtual FormField? ParentField { get; set; }

    public virtual ICollection<FormField> ChildFields { get; set; } = new List<FormField>();
    public virtual ICollection<FieldValidation> Validations { get; set; } = new List<FieldValidation>();
    public virtual ICollection<FieldCondition> Conditions { get; set; } = new List<FieldCondition>();
    public virtual ICollection<FieldOption> Options { get; set; } = new List<FieldOption>();
}
