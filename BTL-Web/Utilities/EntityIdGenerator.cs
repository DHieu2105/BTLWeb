using Microsoft.EntityFrameworkCore;

namespace BTL_Web.Utilities;

public static class EntityIdGenerator
{
    public static async Task<string> GenerateNextIdAsync(
        IQueryable<string?> sourceIds,
        Func<string, Task<bool>> existsAsync,
        string prefix,
        int digits = 6)
    {
        var ids = await sourceIds
            .Where(id => id != null)
            .ToListAsync();

        var maxNumeric = 0;
        foreach (var item in ids)
        {
            if (string.IsNullOrWhiteSpace(item) || !item.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || item.Length <= prefix.Length)
            {
                continue;
            }

            var numericPart = item.Substring(prefix.Length);
            if (int.TryParse(numericPart, out var num) && num > maxNumeric)
            {
                maxNumeric = num;
            }
        }

        var next = maxNumeric + 1;
        var candidate = $"{prefix}{next.ToString($"D{digits}")}";

        while (await existsAsync(candidate))
        {
            next++;
            candidate = $"{prefix}{next.ToString($"D{digits}")}";
        }

        return candidate;
    }
}
