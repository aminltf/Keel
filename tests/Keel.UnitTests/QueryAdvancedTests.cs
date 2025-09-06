using System.Linq.Expressions;
using Keel.Kernel.Core.Querying;
using Keel.UnitTests.Support;

namespace Keel.UnitTests;

public class QueryAdvancedTests
{
    private sealed class EmpSpec : Specification<Employee> { /* empty; we use public builders */ }

    [Fact]
    public void RangeFilter_ToPredicate_Works_For_Dates()
    {
        var range = new RangeFilter<DateTimeOffset>
        {
            From = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            FromInclusive = true,
            To = new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero),
            ToInclusive = false
        };

        var pred = range.ToPredicate<Employee, DateTimeOffset>(e => e.CreatedOnUtc).Compile();

        Assert.True(pred(new Employee { CreatedOnUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero) }));
        Assert.False(pred(new Employee { CreatedOnUtc = new DateTimeOffset(2025, 1, 31, 0, 0, 0, TimeSpan.Zero) }));
    }

    [Fact]
    public void ApplyRange_Composes_With_Criteria()
    {
        var spec = new EmpSpec();
        spec.Where(e => e.LastName.StartsWith("A"));
        spec.ApplyRange(new RangeFilter<DateTimeOffset>
        {
            From = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            To = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero),
            ToInclusive = false
        }, e => e.CreatedOnUtc);

        var f = spec.Criteria!.Compile();
        Assert.True(f(new Employee { LastName = "Allen", CreatedOnUtc = new DateTimeOffset(2025, 1, 15, 0, 0, 0, TimeSpan.Zero) }));
        Assert.False(f(new Employee { LastName = "Baker", CreatedOnUtc = new DateTimeOffset(2025, 1, 15, 0, 0, 0, TimeSpan.Zero) }));
    }

    [Fact]
    public void MultiSortOptions_Parses_And_ApplySorts_Uses_Whitelist()
    {
        var sort = MultiSortOptions.Parse("lastName,-createdOn");
        var spec = new EmpSpec();

        var map = new Dictionary<string, Expression<Func<Employee, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["firstName"] = e => e.FirstName,
            ["lastName"] = e => e.LastName,
            ["createdOn"] = e => e.CreatedOnUtc
        };

        spec.ApplySorts(sort, map, defaultOrder: new[] { ((Expression<Func<Employee, object>>)(e => e.LastName), false) });

        Assert.Equal(2, spec.OrderBy.Count);
        Assert.Equal("LastName", ((MemberExpression)StripConvert(spec.OrderBy[0].Key.Body)).Member.Name);
        Assert.False(spec.OrderBy[0].Descending);
        Assert.True(spec.OrderBy[1].Descending);
    }

    private static Expression StripConvert(Expression e) =>
        e is UnaryExpression u && u.NodeType == ExpressionType.Convert ? u.Operand : e;
}
