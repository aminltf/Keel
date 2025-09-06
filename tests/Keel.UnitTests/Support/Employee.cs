namespace Keel.UnitTests.Support;

internal sealed class Employee
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
}
