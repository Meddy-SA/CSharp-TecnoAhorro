using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TecnoCredito.Helpers;

public class IntListToStringConverter : ValueConverter<List<int>?, string>
{
    public IntListToStringConverter()
        : base(
            // Conversión de List<int>? a string
            v => v != null ? string.Join(",", v) : string.Empty,
            // Conversión de string a List<int>
            v =>
                !string.IsNullOrEmpty(v)
                    ? v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                    : new List<int>()
        ) { }

    public static ValueComparer<List<int>?> GetComparer()
    {
        return new ValueComparer<List<int>?>(
            (l1, l2) => l1 != null && l2 != null && l1.SequenceEqual(l2),
            c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
            c => c != null ? new List<int>(c) : null
        );
    }
}
