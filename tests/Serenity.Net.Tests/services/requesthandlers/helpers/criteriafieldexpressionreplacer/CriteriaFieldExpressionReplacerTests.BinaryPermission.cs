namespace Serenity.Data;

public partial class CriteriaFieldExpressionReplacerTests
{
    [Fact]
    public void Throws_If_BinaryCriteria_On_DenyFiltering_NonString_Field()
    {
        var mockPermissions = new MockPermissions(perm => true);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() =>
            replacer.Process(new Criteria(TestRow.Fields.DenyFilteringIntField.PropertyName) == 5));
    }

    [Fact]
    public void Throws_If_BinaryCriteria_On_ReadPermission_NonString_Field()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() =>
            replacer.Process(new Criteria(TestRow.Fields.ExtraReadPermissionIntField.PropertyName) > 10));
    }

    [Fact]
    public void Throws_If_In_Criteria_On_ReadPermission_NonString_Field()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        var criteria = new BinaryCriteria(
            new Criteria(TestRow.Fields.ExtraReadPermissionIntField.PropertyName),
            CriteriaOperator.In, new ValueCriteria(new object[] { 1, 2 }));

        Assert.Throws<ValidationError>(() => replacer.Process(criteria));
    }

    [Fact]
    public void Throws_If_Like_On_ReadPermission_String_Field()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions,
            dialect: PostgresDialect.Instance);

        Assert.Throws<ValidationError>(() =>
            replacer.Process(new Criteria(TestRow.Fields.ExtraReadPermissionField.PropertyName).Contains("ab")));
    }

    [Fact]
    public void Allows_BinaryCriteria_On_Permitted_NonString_Field()
    {
        var mockPermissions = new MockPermissions(perm => perm is ExtraReadPermission);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        var result = replacer.Process(new Criteria(TestRow.Fields.ExtraReadPermissionIntField.PropertyName) > 10);

        Assert.NotNull(result);
    }

    [Fact]
    public void Allows_Like_On_Permitted_String_Field()
    {
        var mockPermissions = new MockPermissions(perm => perm is ExtraReadPermission);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions,
            dialect: PostgresDialect.Instance);

        var result = replacer.Process(
            new Criteria(TestRow.Fields.ExtraReadPermissionField.PropertyName).Contains("ab"));

        Assert.NotNull(result);
    }

    [Fact]
    public void Throws_If_Nested_Criteria_In_In_List()
    {
        var mockPermissions = new MockPermissions(perm => true);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        var criteria = new BinaryCriteria(
            new Criteria(TestRow.Fields.NormalIntField.PropertyName),
            CriteriaOperator.In, new ValueCriteria(new object[] { new Criteria("Name") }));

        Assert.Throws<ValidationError>(() => replacer.Process(criteria));
    }

    [Fact]
    public void Honors_Subclass_VisitCriteria_Override_On_Binary_Criteria()
    {
        var mockPermissions = new MockPermissions(perm => true);
        var replacer = new RejectingReplacer(new TestRow(), mockPermissions);

        Assert.Throws<RejectingReplacer.RejectedException>(() =>
            replacer.Process(new Criteria(TestRow.Fields.NormalIntField.PropertyName) == 5));
    }

    private class RejectingReplacer : CriteriaFieldExpressionReplacer
    {
        public RejectingReplacer(IRow row, IPermissionService permissions)
            : base(row, permissions)
        {
        }

        public class RejectedException : Exception
        {
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            throw new RejectedException();
        }
    }
}
