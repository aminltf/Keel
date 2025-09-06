using Keel.Kernel.Abstractions.Time;

namespace Keel.UnitTests.Support;

internal sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow => new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
}
