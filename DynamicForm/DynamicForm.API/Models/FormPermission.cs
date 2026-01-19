using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicForm.API.Models;

public class FormPermission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid FormId { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoleCode { get; set; } = string.Empty;

    [Required]
    public int PermissionType { get; set; } // 1=Form, 2=Field

    public bool CanView { get; set; } = true;

    public bool CanEdit { get; set; } = false;

    public bool CanDelete { get; set; } = false;

    public bool CanApprove { get; set; } = false;

    // Navigation properties
    [ForeignKey("FormId")]
    public virtual Form Form { get; set; } = null!;
}
