using Keel.Kernel.Validation;

namespace Keel.UnitTests;

public class ValidationTests
{
    [Fact]
    public void ValidationResult_ToFailureIfInvalid_Produces_Standard_Error_Code()
    {
        var vr = ValidationResult.Failure(new[]
        {
            new ValidationError("Validation.Rule", "Name is required", "Name")
        });

        var result = vr.ToFailureIfInvalid();
        Assert.False(result.IsSuccess);
        Assert.Equal("Validation.Failed", result.Error.Code);
    }
}
