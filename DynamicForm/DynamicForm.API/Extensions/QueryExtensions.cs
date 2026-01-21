using DynamicForm.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicForm.API.Extensions;

/// <summary>
/// Extension methods để query bằng PublicId (Guid) thay vì Id (int)
/// </summary>
public static class QueryExtensions
{
    public static async Task<Form?> FindByPublicIdAsync(this DbSet<Form> dbSet, Guid publicId)
    {
        return await dbSet.FirstOrDefaultAsync(f => f.PublicId == publicId);
    }

    public static async Task<FormVersion?> FindByPublicIdAsync(this DbSet<FormVersion> dbSet, Guid publicId)
    {
        return await dbSet.FirstOrDefaultAsync(v => v.PublicId == publicId);
    }

    public static async Task<FormField?> FindByPublicIdAsync(this DbSet<FormField> dbSet, Guid publicId)
    {
        return await dbSet.FirstOrDefaultAsync(f => f.PublicId == publicId);
    }

    public static async Task<FormDataValue?> FindByPublicIdAsync(this DbSet<FormDataValue> dbSet, Guid publicId)
    {
        return await dbSet.FirstOrDefaultAsync(v => v.PublicId == publicId);
    }

    public static async Task<int?> GetIdByPublicIdAsync(this DbSet<Form> dbSet, Guid publicId)
    {
        return await dbSet
            .Where(f => f.PublicId == publicId)
            .Select(f => (int?)f.Id)
            .FirstOrDefaultAsync();
    }

    public static async Task<int?> GetIdByPublicIdAsync(this DbSet<FormVersion> dbSet, Guid publicId)
    {
        return await dbSet
            .Where(v => v.PublicId == publicId)
            .Select(v => (int?)v.Id)
            .FirstOrDefaultAsync();
    }
}
