using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FieldCondition
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FieldId { get; set; }

    [Required]
    public int ConditionType { get; set; } // 1=Show, 2=Hide, 3=Enable, 4=Disable

    [Required]
    [MaxLength(1000)]
    public string Expression { get; set; } = string.Empty; // JSON expression

    public string? ActionsJson { get; set; } // JSON for actions

    public int Priority { get; set; } = 0;

    // Navigation properties
    [ForeignKey("FieldId")]
    public virtual FormField Field { get; set; } = null!;
}
