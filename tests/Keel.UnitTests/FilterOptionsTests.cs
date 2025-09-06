using Keel.Kernel.Core.Querying;

namespace Keel.UnitTests;

public sealed class FilterOptionsTests
{
    private sealed class Employee2
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Guid DepartmentId { get; set; }
        public DateTimeOffset CreatedOnUtc { get; set; }
    }

    private sealed class EmpSpec : Specification<Employee2> { }

    private static readonly Guid DeptA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DeptB = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Employee2[] Seed =
    {
        new() { FirstName="Ali",   LastName="Ahmadi", DepartmentId=DeptA, CreatedOnUtc=new DateTimeOffset(2025,01,05,0,0,0,TimeSpan.Zero) },
        new() { FirstName="Sara",  LastName="Babaei", DepartmentId=DeptA, CreatedOnUtc=new DateTimeOffset(2025,01,20,0,0,0,TimeSpan.Zero) },
        new() { FirstName="Reza",  LastName="Rahimi", DepartmentId=DeptB, CreatedOnUtc=new DateTimeOffset(2025,02,10,0,0,0,TimeSpan.Zero) },
        new() { FirstName=null,    LastName=null,     DepartmentId=DeptB, CreatedOnUtc=new DateTimeOffset(2025,01,15,0,0,0,TimeSpan.Zero) },
    };

    [Fact]
    public void Contains_CaseInsensitive_And_Trim_Works()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter>
            {
                new() { Field="firstName", Operator=FilterOperator.Contains, Value="  li " } // "Ali"
            }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForContains("firstName", e => e.FirstName) // case-insensitive by default
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).Select(e => e.FirstName ?? "").ToArray();

        Assert.Single(hits);
        Assert.Equal("Ali", hits[0]);
    }

    [Fact]
    public void Equals_Guid_Parses_And_Filters_Safely()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter>
            {
                new() { Field="departmentId", Operator=FilterOperator.Equals, Value=DeptA.ToString() }
            }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForEquals("departmentId", e => e.DepartmentId, FilterParsers.TryGuid)
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).ToArray();

        Assert.Equal(2, hits.Length);
        Assert.All(hits, e => Assert.Equal(DeptA, e.DepartmentId));
    }

    [Fact]
    public void Between_Date_Range_Inclusive_Exclusive_Works()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter>
            {
                new()
                {
                    Field="createdOn", Operator=FilterOperator.Between,
                    From="2025-01-01", To="2025-02-01", ToInclusive=false
                }
            }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForBetween("createdOn", e => e.CreatedOnUtc, FilterParsers.TryDateTimeOffset)
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).ToArray();

        Assert.Equal(3, hits.Length);
        Assert.All(hits, e => Assert.True(e.CreatedOnUtc < new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void Unknown_Field_Is_Ignored_Silently()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter> { new() { Field = "unknown", Operator = FilterOperator.Equals, Value = "x" } }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, _ => { });

        Assert.Null(spec.Criteria);
    }

    [Fact]
    public void Bad_Parse_Is_Ignored_Safely()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter>
            {
                new() { Field="departmentId", Operator=FilterOperator.Equals, Value="NOT-A-GUID" }
            }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForEquals("departmentId", e => e.DepartmentId, FilterParsers.TryGuid)
        );

        Assert.Null(spec.Criteria);
    }

    [Fact]
    public void Multiple_Filters_Compose_With_AND()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter>
            {
                new() { Field="firstName", Operator=FilterOperator.Contains, Value="a" },     // Ali, Sara
                new() { Field="createdOn", Operator=FilterOperator.Between, From="2025-01-10", To="2025-01-31" }, // Sara + null-name
                new() { Field="departmentId", Operator=FilterOperator.Equals, Value=DeptA.ToString() } // Sara
            }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForContains("firstName", e => e.FirstName)
            .ForBetween("createdOn", e => e.CreatedOnUtc, FilterParsers.TryDateTimeOffset)
            .ForEquals("departmentId", e => e.DepartmentId, FilterParsers.TryGuid)
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).ToArray();

        Assert.Single(hits);
        Assert.Equal("Sara", hits[0].FirstName);
    }

    [Fact]
    public void Null_Strings_Do_Not_Throw_And_Are_Excluded()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter> { new() { Field = "lastName", Operator = FilterOperator.Contains, Value = "a" } }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForContains("lastName", e => e.LastName)
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).ToArray();

        Assert.Equal(3, hits.Length);
        Assert.DoesNotContain(hits, e => e.LastName is null);
    }

    [Fact]
    public void Field_Name_Is_Case_Insensitive()
    {
        var filters = new FilterOptions
        {
            Filters = new List<FieldFilter> { new() { Field = "FiRsTnAmE", Operator = FilterOperator.Contains, Value = "ALI" } }
        };

        var spec = new EmpSpec();
        spec.ApplyFilters(filters, map => map
            .ForContains("firstName", e => e.FirstName)
        );

        var pred = spec.Criteria!.Compile();
        var hits = Seed.Where(pred).ToArray();

        Assert.Single(hits);
        Assert.Equal("Ali", hits[0].FirstName);
    }
}
