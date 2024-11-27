namespace Serenity.Tests.Services;

public partial class CriteriaFieldExpressionReplacerTests
{
    [Fact]
    public void Throws_If_Field_Has_DenyFiltering_Flag()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.DenyFilteringField.PropertyName)));
    }

    [Fact]
    public void Throws_If_Field_Has_NotMapped_Flag()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.NotMappedField.PropertyName)));
    }

    [Fact]
    public void Throws_If_Field_Has_MinSelectLevel_Of_Never()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.MinSelectLevelNeverField.PropertyName)));
    }

    [Fact]
    public void Throws_If_Field_ReadPermission_And_User_Doesnt_Have_Permission()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.ExtraReadPermissionField.PropertyName)));
    }

    [Fact]
    public void Throws_If_Field_Is_Not_A_Lookup_Permission_And_Lookup_Mode_Is_True()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, true);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.NormalField.PropertyName)));
    }

    [Fact]
    public void Throws_If_Field_Is_A_Lookup_Permission_And_Lookup_Mode_Is_True_And_User_Doesnt_Have_Fields_Permission()
    {
        var mockPermissions = new MockPermissions(perm => false);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, true);

        Assert.Throws<ValidationError>(() => replacer.Validate(new Criteria(TestRow.Fields.ExtraReadPermissionWithLookupIncludeField.PropertyName)));
    }

    [Fact]
    public void Allows_If_Field_Is_A_Normal_Field_And_User_Have_Fields_Permission()
    {
        var mockPermissions = new MockPermissions(perm => perm == ReadPermission);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        replacer.Validate(new Criteria(TestRow.Fields.NormalField.PropertyName));
    }

    [Fact]
    public void Allows_If_Field_Has_ReadPermission_And_User_Also_Have_That_Permission()
    {
        var mockPermissions = new MockPermissions(perm => perm is ReadPermission or ExtraReadPermission);
        var replacer = new CriteriaFieldExpressionReplacer(new TestRow(), mockPermissions, false);

        replacer.Validate(new Criteria(TestRow.Fields.ExtraReadPermissionField.PropertyName));
    }
}