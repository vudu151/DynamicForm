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
    public DbSet<FormDataValue> FormDataValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Form
        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.CurrentVersion)
                .WithMany()
                .HasForeignKey(e => e.CurrentVersionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // FormVersion
        modelBuilder.Entity<FormVersion>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => new { e.FormId, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.FormId, e.Status });
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Versions)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FormField
        modelBuilder.Entity<FormField>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => new { e.FormVersionId, e.FieldCode }).IsUnique();
            entity.HasIndex(e => new { e.FormVersionId, e.DisplayOrder });
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.FormVersion)
                .WithMany(v => v.Fields)
                .HasForeignKey(e => e.FormVersionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ParentField)
                .WithMany(f => f.ChildFields)
                .HasForeignKey(e => e.ParentFieldId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction to avoid cascade path conflict
        });

        // FieldValidation
        modelBuilder.Entity<FieldValidation>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => e.FieldId);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Validations)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FieldCondition
        modelBuilder.Entity<FieldCondition>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => e.FieldId);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Conditions)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FieldOption
        modelBuilder.Entity<FieldOption>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            entity.HasIndex(e => new { e.FieldId, e.DisplayOrder });
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            entity.HasOne(e => e.Field)
                .WithMany(f => f.Options)
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FormDataValue
        modelBuilder.Entity<FormDataValue>(entity =>
        {
            entity.HasIndex(e => e.PublicId).IsUnique(); // Index cho public API
            // Indexes cho query performance
            entity.HasIndex(e => e.SubmissionId); // Để group các values của cùng submission
            entity.HasIndex(e => new { e.ObjectId, e.ObjectType, e.FormVersionId }); // Để query theo object
            entity.HasIndex(e => e.FormVersionId);
            entity.HasIndex(e => e.FormFieldId);
            entity.HasIndex(e => new { e.SubmissionId, e.FormFieldId, e.DisplayOrder }); // Để query và order
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // IDENTITY
            
            // Foreign keys
            // Note: SubmissionId là INT tự quản lý, không có FK constraint
            entity.HasOne(e => e.FormVersion)
                .WithMany()
                .HasForeignKey(e => e.FormVersionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.FormField)
                .WithMany()
                .HasForeignKey(e => e.FormFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
