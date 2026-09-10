namespace Serenity.Data;

public class SafeCriteriaValidatorTests
{
    [Fact]
    public void Validate_SimpleFieldCriteria_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(new Criteria("Name"));
    }

    [Fact]
    public void Validate_EmptyCriteria_DoesNotThrow()
    {
        // Empty expression passes validation without a field name check.
        new SafeCriteriaValidator().Validate(Criteria.Empty);
    }

    [Fact]
    public void Validate_InvalidFieldExpression_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("x > 5")));

        Assert.Equal("InvalidCriteriaField", exception.ErrorCode);
        Assert.Equal("x > 5 is not a valid field name!", exception.Message);
    }

    [Fact]
    public void Validate_ParamCriteria_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new ParamCriteria("@p1")));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
        Assert.Equal("Param type criterias is not supported!", exception.Message);
    }

    [Fact]
    public void Validate_BinaryCriteriaWithValidField_DoesNotThrow()
    {
        // Visitor traverses the binary criteria and its value operand.
        new SafeCriteriaValidator().Validate(new Criteria("Name") == "x");
    }

    [Fact]
    public void Validate_BinaryCriteriaWithInvalidLeftField_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("bad expr") == "x"));

        Assert.Equal("InvalidCriteriaField", exception.ErrorCode);
        Assert.Equal("bad expr is not a valid field name!", exception.Message);
    }

    [Fact]
    public void Validate_BinaryCriteriaWithInvalidRightField_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("Name") == new Criteria("bad expr")));

        Assert.Equal("InvalidCriteriaField", exception.ErrorCode);
        Assert.Equal("bad expr is not a valid field name!", exception.Message);
    }

    [Fact]
    public void Validate_ParamCriteriaInsideBinary_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("Name") == new Parameter("@p1")));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_IsNullCriteria_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(new Criteria("Name").IsNull());
    }

    [Fact]
    public void Validate_NestedLogicalCriteria_DoesNotThrow()
    {
        var a = new Criteria("Field1").IsNotNull();
        var b = new Criteria("Field2").IsNotNull();
        var c = new Criteria("Field3").IsNotNull();

        new SafeCriteriaValidator().Validate((a & b) | c);
    }
}
