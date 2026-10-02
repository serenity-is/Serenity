using System.Collections.Concurrent;

namespace Serenity.Data;

public class BaseCriteriaTests
{
    private enum TestEnum
    {
        A = 1
    }

    // IsNull / IsNotNull

    [Fact]
    public void IsNull_RendersIsNull()
    {
        var name = new Criteria("Name");

        Assert.Equal("Name IS NULL", name.IsNull().ToString());
    }

    [Fact]
    public void IsNotNull_RendersIsNotNull()
    {
        var name = new Criteria("Name");

        Assert.Equal("Name IS NOT NULL", name.IsNotNull().ToString());
    }

    // Like / NotLike

    [Fact]
    public void Like_RendersLikeWithParam()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1)", name.Like("x%").ToString(query));
        Assert.Equal("x%", query.Params["@p1"]);
    }

    [Fact]
    public void Like_WithUpper_WrapsBothSidesInUpperFunction()
    {
        var name = new Criteria("Name");
        var criteria = name.Like("x%", upper: true);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.Like, binary.Operator);

        var left = Assert.IsType<UpperFunctionCriteria>(binary.LeftOperand);
        Assert.Same(name, Assert.IsType<Criteria>(left.Arguments[0]));

        var right = Assert.IsType<UpperFunctionCriteria>(binary.RightOperand);
        Assert.Equal("x%", Assert.IsType<ValueCriteria>(right.Arguments[0]).Value);

        // Only the mask becomes a parameter; with a fresh query it is @p1.
        var query = new SqlQuery();
        Assert.Equal("(UPPER(Name) LIKE UPPER(@p1))", criteria.ToString(query));
        Assert.Equal("x%", query.Params["@p1"]);
    }

    [Fact]
    public void NotLike_RendersNotLikeWithParam()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name NOT LIKE @p1)", name.NotLike("x%").ToString(query));
        Assert.Equal("x%", query.Params["@p1"]);
    }

    [Fact]
    public void NotLike_WithUpper_WrapsBothSidesInUpperFunction()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(UPPER(Name) NOT LIKE UPPER(@p1))", name.NotLike("x%", upper: true).ToString(query));
        Assert.Equal("x%", query.Params["@p1"]);
    }

    // StartsWith / EndsWith / Contains / NotContains

    [Fact]
    public void StartsWith_AppendsPercentToMask()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1)", name.StartsWith("x").ToString(query));
        Assert.Equal("x%", query.Params["@p1"]);
    }

    [Fact]
    public void EndsWith_PrependsPercentToMask()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1)", name.EndsWith("x").ToString(query));
        Assert.Equal("%x", query.Params["@p1"]);
    }

    [Fact]
    public void Contains_WrapsMaskWithPercents()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1)", name.Contains("x").ToString(query));
        Assert.Equal("%x%", query.Params["@p1"]);
    }

    [Fact]
    public void NotContains_UsesNotLikeWithWrappedMask()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name NOT LIKE @p1)", name.NotContains("x").ToString(query));
        Assert.Equal("%x%", query.Params["@p1"]);
    }

    [Fact]
    public void Contains_EscapesLikeSpecialsInMask()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name LIKE @p1 ESCAPE '!')", name.Contains("a%b").ToString(query));
        Assert.Equal("%a!%b%", query.Params["@p1"]);
    }

    [Fact]
    public void StartsWith_NullMask_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.StartsWith(null));
        Assert.Equal("text", exception.ParamName);
    }

    [Fact]
    public void EndsWith_NullMask_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.EndsWith(null));
        Assert.Equal("text", exception.ParamName);
    }

    [Fact]
    public void Contains_NullMask_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.Contains(null));
        Assert.Equal("text", exception.ParamName);
    }

    [Fact]
    public void NotContains_NullMask_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.NotContains(null));
        Assert.Equal("text", exception.ParamName);
    }

    // In

    [Fact]
    public void In_WithValues_RendersInList()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name IN (@p1,@p2,@p3))", name.In(1, 2, 3).ToString(query));
        Assert.Equal(1, query.Params["@p1"]);
        Assert.Equal(2, query.Params["@p2"]);
        Assert.Equal(3, query.Params["@p3"]);
    }

    [Fact]
    public void In_NullValuesArray_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.In((object[])null));
        Assert.Equal("values", exception.ParamName);
    }

    [Fact]
    public void In_EmptyValuesArray_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        Assert.Throws<ArgumentNullException>(() => name.In<int>());
    }

    [Fact]
    public void In_WithCriteriaStatement_RendersSubQuery()
    {
        var name = new Criteria("Name");
        var statement = new Criteria("SELECT Id FROM SomeTable");

        var criteria = name.In(statement);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.In, binary.Operator);
        Assert.Same(statement, binary.RightOperand);
        // The BaseCriteria overload uses the statement as is, without parens.
        // Use In(ISqlQuery) for subqueries, which wraps the query in parens.
        Assert.Equal("(Name IN SELECT Id FROM SomeTable)", criteria.ToString());
    }

    [Fact]
    public void In_WithSingleCriteriaAsObject_DelegatesToCriteriaOverload()
    {
        var name = new Criteria("Name");
        var statement = new Criteria("SELECT Id FROM SomeTable");

        var criteria = name.In<object>(statement);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Same(statement, binary.RightOperand);
        Assert.Equal("(Name IN SELECT Id FROM SomeTable)", criteria.ToString());
    }

    [Fact]
    public void In_NullStatement_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.In((BaseCriteria)null));
        Assert.Equal("statement", exception.ParamName);
    }

    [Fact]
    public void In_EmptyCriteria_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        Assert.Throws<ArgumentNullException>(() => name.In(Criteria.Empty));
    }

    [Fact]
    public void In_WithSingleEnumerableAsObject_WrapsCollectionAsValue()
    {
        var name = new Criteria("Name");
        var values = new List<int> { 1, 2, 3 };

        var criteria = name.In<object>(values);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(values, Assert.IsType<ValueCriteria>(binary.RightOperand).Value);

        var query = new SqlQuery();
        Assert.Equal("(Name IN (@p1,@p2,@p3))", criteria.ToString(query));
        Assert.Equal(1, query.Params["@p1"]);
        Assert.Equal(2, query.Params["@p2"]);
        Assert.Equal(3, query.Params["@p3"]);
    }

    [Fact]
    public void In_WithQuery_RendersSubQuery()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery().From("MyTable").Select("Id");

        var criteria = name.In(query);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        // In(ISqlQuery) wraps the query text in parentheses as a plain query
        // renders without them.
        Assert.Equal("(" + query.ToString() + ")", Assert.IsType<Criteria>(binary.RightOperand).Expression);
        Assert.Equal("(Name IN (" + query.ToString() + "))", criteria.ToString());
    }

    [Fact]
    public void In_WithSubQuery_DoesNotDoubleParentheses()
    {
        var name = new Criteria("Name");
        var outer = new SqlQuery().From("Outer").Select("X");
        var subQuery = outer.SubQuery().From("Inner").Select("Id");

        var criteria = name.In(subQuery);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        // SubQuery() renders enclosed in parenthesis already, so it is used as is.
        Assert.Equal(subQuery.ToString(), Assert.IsType<Criteria>(binary.RightOperand).Expression);
        Assert.Equal("(Name IN " + subQuery.ToString() + ")", criteria.ToString());
    }

    [Fact]
    public void In_WithUnionQuery_RendersSingleParenthesis()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery().From("First").Select("Id");
        query.Union(SqlUnionType.UnionAll)
            .From("Second").Select("Id");

        var criteria = name.In(query);

        // union queries render without parenthesis (OmitParens), so they are wrapped
        Assert.Equal("(Name IN (" + query.ToString() + "))", criteria.ToString());
    }

    [Fact]
    public void In_WithSingleQueryAsObject_DelegatesToQueryOverload()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery().From("MyTable").Select("Id");

        var criteria = name.In<object>(query);

        Assert.Equal("(Name IN (" + query.ToString() + "))", criteria.ToString());
    }

    [Fact]
    public void In_NullQuery_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.In((ISqlQuery)null));
        Assert.Equal("statement", exception.ParamName);
    }

    // NotIn

    [Fact]
    public void NotIn_WithValues_RendersNotInList()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name NOT IN (@p1,@p2))", name.NotIn(1, 2).ToString(query));
        Assert.Equal(1, query.Params["@p1"]);
        Assert.Equal(2, query.Params["@p2"]);
    }

    [Fact]
    public void NotIn_WithCriteriaStatement_RendersSubQuery()
    {
        var name = new Criteria("Name");
        var statement = new Criteria("SELECT Id FROM SomeTable");

        var criteria = name.NotIn(statement);

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.NotIn, binary.Operator);
        Assert.Same(statement, binary.RightOperand);
        // The BaseCriteria overload uses the statement as is, without parens.
        // Use NotIn(ISqlQuery) for subqueries, which wraps the query in parens.
        Assert.Equal("(Name NOT IN SELECT Id FROM SomeTable)", criteria.ToString());
    }

    [Fact]
    public void NotIn_WithQuery_RendersSubQuery()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery().From("MyTable").Select("Id");

        var criteria = name.NotIn(query);

        Assert.Equal("(Name NOT IN (" + query.ToString() + "))", criteria.ToString());
    }

    [Fact]
    public void NotIn_WithSubQuery_DoesNotDoubleParentheses()
    {
        var name = new Criteria("Name");
        var outer = new SqlQuery().From("Outer").Select("X");
        var subQuery = outer.SubQuery().From("Inner").Select("Id");

        var criteria = name.NotIn(subQuery);

        // SubQuery() renders enclosed in parenthesis already, so it is used as is.
        Assert.Equal("(Name NOT IN " + subQuery.ToString() + ")", criteria.ToString());
    }

    [Fact]
    public void NotIn_NullValuesArray_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.NotIn((object[])null));
        Assert.Equal("values", exception.ParamName);
    }

    [Fact]
    public void NotIn_EmptyValuesArray_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        Assert.Throws<ArgumentNullException>(() => name.NotIn<int>());
    }

    [Fact]
    public void NotIn_NullStatement_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => name.NotIn((BaseCriteria)null));
        Assert.Equal("statement", exception.ParamName);
    }

    [Fact]
    public void NotIn_EmptyCriteria_ThrowsArgumentNullException()
    {
        var name = new Criteria("Name");

        Assert.Throws<ArgumentNullException>(() => name.NotIn(Criteria.Empty));
    }

    // Comparison operators with values

    [Fact]
    public void Operator_Equal_WithStringValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name = @p1)", (name == "x").ToString(query));
        Assert.Equal("x", query.Params["@p1"]);
    }

    [Fact]
    public void Operator_NotEqual_WithStringValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name != @p1)", (name != "x").ToString(query));
        Assert.Equal("x", query.Params["@p1"]);
    }

    [Fact]
    public void Operator_GreaterThan_WithIntValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name > @p1)", (name > 5).ToString(query));
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void Operator_GreaterThanOrEqual_WithIntValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name >= @p1)", (name >= 5).ToString(query));
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void Operator_LessThan_WithIntValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name < @p1)", (name < 5).ToString(query));
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void Operator_LessThanOrEqual_WithIntValue()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name <= @p1)", (name <= 5).ToString(query));
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void Operator_Equal_WithNumericValueTypes()
    {
        var name = new Criteria("Name");

        var intQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == 5).ToString(intQuery));
        Assert.Equal(5, intQuery.Params["@p1"]);

        var longQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == 5L).ToString(longQuery));
        Assert.Equal(5L, longQuery.Params["@p1"]);

        var doubleQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == 5.5).ToString(doubleQuery));
        Assert.Equal(5.5, doubleQuery.Params["@p1"]);

        var decimalQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == 5.5m).ToString(decimalQuery));
        Assert.Equal(5.5m, decimalQuery.Params["@p1"]);
    }

    [Fact]
    public void Operator_Equal_WithDateTimeGuidAndEnumValues()
    {
        var name = new Criteria("Name");

        var date = new DateTime(2023, 1, 15, 10, 30, 0);
        var dateQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == date).ToString(dateQuery));
        Assert.Equal(date, dateQuery.Params["@p1"]);

        var guid = new Guid("12345678-1234-1234-1234-123456789012");
        var guidQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == guid).ToString(guidQuery));
        Assert.Equal(guid, guidQuery.Params["@p1"]);

        var enumQuery = new SqlQuery();
        Assert.Equal("(Name = @p1)", (name == TestEnum.A).ToString(enumQuery));
        Assert.Equal(TestEnum.A, enumQuery.Params["@p1"]);
    }

    // Comparison operators with criteria / field / parameter

    [Fact]
    public void Operator_Equal_WithCriteria()
    {
        var name = new Criteria("Name");
        var other = new Criteria("Other");

        var criteria = name == other;

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.EQ, binary.Operator);
        Assert.Same(name, binary.LeftOperand);
        Assert.Same(other, binary.RightOperand);
        Assert.Equal("(Name = Other)", criteria.ToString());
    }

    [Fact]
    public void Operator_Equal_WithFieldCriteria()
    {
        var name = new Criteria("Name");
        var field = new Criteria(AllFieldsRow.Fields.AString);

        Assert.Equal("(Name = T0.[AString])", (name == field).ToString());
    }

    [Fact]
    public void Operator_Equal_WithParameter()
    {
        var name = new Criteria("Name");

        var criteria = name == new Parameter("@p1");

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal("@p1", Assert.IsType<ParamCriteria>(binary.RightOperand).Name);
        Assert.Equal("(Name = @p1)", criteria.ToString());
    }

    [Fact]
    public void Operator_NotEqual_WithParameter()
    {
        var name = new Criteria("Name");

        var criteria = name != new Parameter("@p1");

        var binary = Assert.IsType<BinaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.NE, binary.Operator);
        Assert.Equal("@p1", Assert.IsType<ParamCriteria>(binary.RightOperand).Name);
        Assert.Equal("(Name != @p1)", criteria.ToString());
    }

    // Unary operators

    [Fact]
    public void Operator_Not_WrapsInNotUnary()
    {
        var name = new Criteria("Name");

        var criteria = !name;

        var unary = Assert.IsType<UnaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.Not, unary.Operator);
        Assert.Same(name, unary.Operand);
        Assert.Equal("NOT (Name)", criteria.ToString());
    }

    [Fact]
    public void Operator_Paren_WrapsInParenUnary()
    {
        var name = new Criteria("Name");

        var criteria = ~name;

        var unary = Assert.IsType<UnaryCriteria>(criteria);
        Assert.Equal(CriteriaOperator.Paren, unary.Operator);
        Assert.Same(name, unary.Operand);
        Assert.Equal("(Name)", criteria.ToString());
    }

    [Fact]
    public void Operator_Paren_OnEmptyCriteria_ReturnsSameInstance()
    {
        // ~ returns the criteria itself when it is empty, without wrapping.
        Assert.Same(Criteria.Empty, ~Criteria.Empty);
        Assert.Equal("", (~Criteria.Empty).ToString());
    }

    // Logical operators

    [Fact]
    public void Operator_And_Or_Xor_CombineCriteria()
    {
        var name = new Criteria("Name");
        var other = new Criteria("Other");

        Assert.Equal("(Name AND Other)", (name & other).ToString());
        Assert.Equal("(Name OR Other)", (name | other).ToString());
        Assert.Equal("(Name XOR Other)", (name ^ other).ToString());
    }

    [Fact]
    public void Operator_And_WithEmptyLeftSide_ReturnsRightSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, Criteria.Empty & name);
    }

    [Fact]
    public void Operator_And_WithEmptyRightSide_ReturnsLeftSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, name & Criteria.Empty);
    }

    [Fact]
    public void Operator_And_WithBothSidesEmpty_ReturnsEmpty()
    {
        Assert.Same(Criteria.Empty, Criteria.Empty & Criteria.Empty);
    }

    [Fact]
    public void Operator_And_WithNullLeftSide_ReturnsRightSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, null & name);
    }

    [Fact]
    public void Operator_And_WithNullRightSide_ReturnsLeftSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, name & null);
    }

    [Fact]
    public void Operator_Or_WithEmptySide_ReturnsOtherSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, Criteria.Empty | name);
        Assert.Same(name, name | Criteria.Empty);
    }

    [Fact]
    public void Operator_Xor_WithEmptySide_ReturnsOtherSide()
    {
        var name = new Criteria("Name");

        Assert.Same(name, Criteria.Empty ^ name);
        Assert.Same(name, name ^ Criteria.Empty);
    }

    // Short circuit && and || (operator true/false both return false,
    // so both sides are always combined with & / |)

    [Fact]
    public void ShortCircuitAnd_CombinesBothSidesWithAnd()
    {
        var name = new Criteria("Name");
        var other = new Criteria("Other");
        var query = new SqlQuery();

        // Each side is a BinaryCriteria which wraps itself in parens.
        Assert.Equal("((Name = @p1) AND (Other = @p2))", (name == "x" && other == "y").ToString(query));
    }

    [Fact]
    public void ShortCircuitOr_CombinesBothSidesWithOr()
    {
        var name = new Criteria("Name");
        var other = new Criteria("Other");
        var query = new SqlQuery();

        // Each side is a BinaryCriteria which wraps itself in parens.
        Assert.Equal("((Name = @p1) OR (Other = @p2))", (name == "x" || other == "y").ToString(query));
    }

    // ToString family

    [Fact]
    public void ToString_WithValueCriteria_ThrowsInvalidOperationException()
    {
        var name = new Criteria("Name");

        var exception = Assert.Throws<InvalidOperationException>(() => (name == "x").ToString());
        Assert.Equal("Criteria should not have parameters!", exception.Message);
    }

    [Fact]
    public void ToString_WithQuery_AddsParamsToQuery()
    {
        var name = new Criteria("Name");
        var query = new SqlQuery();

        Assert.Equal("(Name = @p1)", (name == "x").ToString(query));
        Assert.Equal("x", query.Params["@p1"]);
    }

    [Fact]
    public void ToStringIgnoreParams_ReplacesParamsWithAutoGeneratedNames()
    {
        // Each render uses a fresh counter, so numbering always starts at @p0.
        var name = new Criteria("Name");

        Assert.Equal("(Name = @p0)", (name == "x").ToStringIgnoreParams());
        Assert.Equal("(Name = @p0)", (name == "y").ToStringIgnoreParams());
    }

    [Fact]
    public void ToStringIgnoreParams_WithMultipleParams_UsesConsecutiveNumbers()
    {
        var name = new Criteria("Name");
        var text = (name == "x" & name == "y").ToStringIgnoreParams();

        Assert.Equal("((Name = @p0) AND (Name = @p1))", text);
    }

    // Equals / GetHashCode

    [Fact]
    public void Equals_TwoDistinctInstancesWithSameContent_AreNotEqual()
    {
        // BaseCriteria has no value equality; Equals is reference equality.
        var first = new Criteria("Name");
        var second = new Criteria("Name");

        Assert.NotSame(first, second);
        Assert.False(first.Equals(second));
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void GetHashCode_SameInstance_ReturnsSameValue()
    {
        var name = new Criteria("Name");

        Assert.Equal(name.GetHashCode(), name.GetHashCode());
    }

    // Null handling: null is never mapped to IS NULL (use IsNull() for that).
    // A null *value* renders as "= NULL" (SQL UNKNOWN, matching SQL behavior),
    // while null criteria / field / parameter objects throw. Statically typing
    // the null sides below is what selects each overload (bare `== null` does
    // not compile, which is intentional).

    [Fact]
    public void Equal_NullStringValue_Renders_Equals_Null_Param()
    {
        var query = new SqlQuery();
        var result = new Criteria("Name") == (string)null;

        var binary = Assert.IsType<BinaryCriteria>(result);
        Assert.Equal(CriteriaOperator.EQ, binary.Operator);
        Assert.Null(Assert.IsType<ValueCriteria>(binary.RightOperand).Value);
        Assert.Equal("(Name = @p1)", result.ToString(query));
        Assert.Null(query.Params["@p1"]);
    }

    [Fact]
    public void NotEqual_NullStringValue_Renders_NotEquals_Null_Param()
    {
        var query = new SqlQuery();
        var result = new Criteria("Name") != (string)null;

        Assert.Equal(CriteriaOperator.NE, Assert.IsType<BinaryCriteria>(result).Operator);
        Assert.Equal("(Name != @p1)", result.ToString(query));
        Assert.Null(query.Params["@p1"]);
    }

    [Fact]
    public void Equal_NullCriteria_ThrowsArgumentNullException()
    {
        BaseCriteria left = new Criteria("Name");
        BaseCriteria right = null!;

        Assert.Throws<ArgumentNullException>(() => left == right);
        Assert.Throws<ArgumentNullException>(() => left != right);
        Assert.Throws<ArgumentNullException>(() => right == left);
    }

    [Fact]
    public void Equal_NullField_ThrowsArgumentNullException()
    {
        var criteria = new Criteria("Name");
        Field field = null!;

        Assert.Throws<ArgumentNullException>(() => criteria == field);
        Assert.Throws<ArgumentNullException>(() => field == criteria);
    }

    [Fact]
    public void Comparison_DefaultParameter_ThrowsArgumentNullException()
    {
        // Parameter is a struct and cannot be null; default has a null name,
        // which the ParamCriteria constructor rejects.
        BaseCriteria left = new Criteria("Name");
        var param = default(Parameter);

        Assert.Throws<ArgumentNullException>(() => left == param);
        Assert.Throws<ArgumentNullException>(() => left != param);
        Assert.Throws<ArgumentNullException>(() => left > param);
        Assert.Throws<ArgumentNullException>(() => left >= param);
        Assert.Throws<ArgumentNullException>(() => left < param);
        Assert.Throws<ArgumentNullException>(() => left <= param);
    }

    [Fact]
    public void Parameter_NullOrEmptyName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Parameter(null!));
        Assert.Throws<ArgumentNullException>(() => new Parameter(""));
    }

    [Fact]
    public void ToStringIgnoreParams_ConcurrentCalls_AreDeterministic()
    {
        // Each render owns its counter (no shared static state), so concurrent
        // renders of equal criteria produce identical text without races.
        const int count = 4096;
        var names = new ConcurrentBag<string>();

        Parallel.For(0, count, i => names.Add(new ValueCriteria(i).ToStringIgnoreParams()));

        Assert.Equal(count, names.Count);
        Assert.Single(names.Distinct());
    }
}
