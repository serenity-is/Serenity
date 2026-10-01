namespace Serenity.Data;

public class SafeCriteriaValidatorTests
{
    [Fact]
    public void Validate_SimpleFieldCriteria_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(new Criteria("Name"));
    }

    [Fact]
    public void Validate_EmptyCriteriaAtRoot_DoesNotThrow()
    {
        // Empty root criteria means "no filter" and is skipped without visiting.
        new SafeCriteriaValidator().Validate(Criteria.Empty);
        new SafeCriteriaValidator().Validate(null);
    }

    [Fact]
    public void Validate_EmptyCriteriaAsNestedOperand_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("Name") == Criteria.Empty));

        Assert.Equal("InvalidCriteriaField", exception.ErrorCode);
    }

    [Fact]
    public void Validate_CriteriaValueContainingNestedCriteria_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new Criteria("Name") == new ValueCriteria(new Criteria("X"))));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_ValueArrayContainingNestedCriteria_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(
                new Criteria("Name") == new ValueCriteria(new object[] { new Criteria("X") })));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_NestedValueArrayContainingNestedCriteria_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(
                new Criteria("Name") == new ValueCriteria(new object[] { new object[] { new Criteria("X") } })));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_ValueArrayWithoutCriteria_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(
            new Criteria("Name") == new ValueCriteria(new object[] { 1, "x", null }));
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
    public void Validate_UpperFunctionWithFieldArgument_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(new UpperFunctionCriteria(new Criteria("Name")));
    }

    [Fact]
    public void Validate_UpperFunctionWithStringArgument_DoesNotThrow()
    {
        new SafeCriteriaValidator().Validate(new UpperFunctionCriteria(new ValueCriteria("name")));
    }

    [Fact]
    public void Validate_UpperFunctionWithInvalidField_ThrowsValidationError()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new UpperFunctionCriteria(new Criteria("bad expr"))));

        Assert.Equal("InvalidCriteriaField", exception.ErrorCode);
        Assert.Equal("bad expr is not a valid field name!", exception.Message);
    }

    [Fact]
    public void Validate_UpperFunctionWithNonStringValue_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new UpperFunctionCriteria(new ValueCriteria(5))));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_UpperFunctionWithCriteriaExpression_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new UpperFunctionCriteria(new Criteria("Name") == "x")));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_UpperFunctionWithWrongNumberOfArguments_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new TestFunction("UPPER", new Criteria("Name"), new Criteria("Other"))));

        Assert.Equal("UnsupportedCriteriaType", exception.ErrorCode);
    }

    [Fact]
    public void Validate_NonUpperFunction_ThrowsUnsupportedCriteriaType()
    {
        var exception = Assert.Throws<ValidationError>(
            () => new SafeCriteriaValidator().Validate(new TestFunction("LOWER", new Criteria("Name"))));

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

    private sealed class TestFunction(string name, params BaseCriteria[] arguments)
        : FunctionCallCriteria(arguments)
    {
        public override string GetFunctionName(ISqlDialect dialect) => name;
    }
}
