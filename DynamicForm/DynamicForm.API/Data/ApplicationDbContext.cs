using DynamicForm.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicForm.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Form> Forms { get; set; }
    public DbSet<FormVersion> FormVersions { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<FieldValidation> FieldValidations { get; set; }
    public DbSet<FieldCondition> FieldConditions { get; set; }
    public DbSet<FieldOption> FieldOptions { get; set; }
    public DbSet<FormData> FormData { get; set; }
    public DbSet<FormDataHistory> FormDataHistory { get; set; }
    public DbSet<FormPermission> FormPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Form
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasOne(e => e.CurrentVersion)
                .WithMany()
                .HasForeignKey(e => e.CurrentVersionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FormVersion
        modelBuilder.Entity<FormVersion>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.FormId, e.IsActive });
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Versions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FormField
        modelBuilder.Entity<FormField>(entity =>
        {
            entity.HasIndex(e => new { e.FormVersionId, e.FieldCode }).IsUnique();
            entity.HasIndex(e => new { e.FormVersionId, e.DisplayOrder });
            entity.HasOne(e => e.FormVersion)
                .WithMany(v => v.Fields)
                .HasForeignKey(e => e.FormVersionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ParentField)
                .WithMany(f => f.ChildFields)
                .HasForeignKey(e => e.ParentFieldId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FieldValidation
        modelBuilder.Entity<FieldValidation>(entity =>
        {
            entity.HasIndex(e => e.FieldId);
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Validations)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FieldCondition
        modelBuilder.Entity<FieldCondition>(entity =>
        {
            entity.HasIndex(e => e.FieldId);
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Conditions)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FieldOption
        modelBuilder.Entity<FieldOption>(entity =>
        {
            entity.HasIndex(e => new { e.FieldId, e.DisplayOrder });
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Options)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormData
        modelBuilder.Entity<FormData>(entity =>
        {
            entity.HasIndex(e => new { e.ObjectId, e.ObjectType });
            entity.HasIndex(e => e.CreatedDate);
            entity.HasOne(e => e.FormVersion)
                .WithMany(v => v.FormData)
                .HasForeignKey(e => e.FormVersionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FormDataHistory
        modelBuilder.Entity<FormDataHistory>(entity =>
        {
            entity.HasIndex(e => e.FormDataId);
            entity.HasIndex(e => e.ChangedDate);
            entity.HasOne(e => e.FormData)
                .WithMany(d => d.History)
                .HasForeignKey(e => e.FormDataId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormPermission
        modelBuilder.Entity<FormPermission>(entity =>
        {
            entity.HasIndex(e => new { e.FormId, e.RoleCode }).IsUnique();
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Permissions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
