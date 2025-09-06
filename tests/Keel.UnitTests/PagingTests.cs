using Keel.Kernel.Core.Querying;

namespace Keel.UnitTests;

public class PagingTests
{
    [Fact]
    public void PageRequest_Clamps_PageSize_And_Calculates_Skip_Take()
    {
        var pr = new PageRequest { PageNumber = 0, PageSize = 1000, MaxPageSize = 50 };
        Assert.Equal(0, pr.Skip);
        Assert.Equal(50, pr.Take);
    }

    [Fact]
    public void PageResponse_Computes_TotalPages()
    {
        var resp = new PageResponse<int>
        {
            Items = new List<int> { 1, 2, 3 },
            TotalCount = 101,
            PageNumber = 2,
            PageSize = 10
        };
        Assert.Equal(11, resp.TotalPages);
    }
}
