namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    [LocalTextPrefix("MyRow")]
    private class RulesRow : Row<RulesRow.RowFields>
    {
        [ValidateRange(1, 10, ErrorMessage = "Pick 1 to 10")]
        public int? RowRule { get => fields.RowRule[this]; set => fields.RowRule[this] = value; }

        public int? FormRule { get => fields.FormRule[this]; set => fields.FormRule[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field RowRule = null!;
            public Int32Field FormRule = null!;
        }
    }

    [FormScript("MyForm"), BasedOnRow(typeof(RulesRow))]
    private class RulesForm
    {
        public int? RowRule { get; set; }

        [ValidateRange(1, 5, ErrorMessage = "Pick 1 to 5")]
        public int? FormRule { get; set; }

        [ValidateLength(10, TextKey = "Site.MyLengthKey", ErrorMessage = "Too long")]
        public string? Explicit { get; set; }

        [ValidateEmail]
        public string? NoMessage { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1, 2, ErrorMessage = "Plain")]
        public int? DataAnnotations { get; set; }

        [ValidateRange(1, 2, Side = ValidationSide.Server)]
        public int? ServerOnly { get; set; }

        [EmailEditor]
        public string? Implied { get; set; }

        public string? None { get; set; }
    }

    private static List<ClientValidationRule>? ProcessRules(string propertyName)
    {
        var item = new PropertyItem();
        new BasicPropertyProcessor().Process(new PropertyInfoSource(typeof(RulesForm).GetProperty(propertyName)!, new RulesRow()), item);
        return item.ValidationRules;
    }

    [Fact]
    public void ValidationRules_Of_The_Row_Have_The_Implied_Key_Of_The_Field()
    {
        var rule = Assert.Single(ProcessRules(nameof(RulesForm.RowRule))!);
        Assert.Equal("Range", rule.Rule);
        Assert.Equal("Db.MyRow.RowRule_ValidateRange", rule.TextKey);
        Assert.Equal("Pick 1 to 10", rule.Message);
    }

    [Fact]
    public void ValidationRules_Of_The_Form_Have_The_Implied_Key_Of_The_Form()
    {
        var rule = Assert.Single(ProcessRules(nameof(RulesForm.FormRule))!);
        Assert.Equal("Forms.MyForm.FormRule_ValidateRange", rule.TextKey);
        Assert.Equal([1, 5], (object[])rule.Params!);
    }

    [Fact]
    public void ValidationRules_Keep_An_Explicit_Key_And_Have_None_Without_A_Message()
    {
        Assert.Equal("Site.MyLengthKey", Assert.Single(ProcessRules(nameof(RulesForm.Explicit))!).TextKey);

        var noMessage = Assert.Single(ProcessRules(nameof(RulesForm.NoMessage))!);
        Assert.Null(noMessage.TextKey);
        Assert.Null(noMessage.Message);
    }

    [Fact]
    public void ValidationRules_Of_DataAnnotations_Have_No_Implied_Key()
    {
        var rule = Assert.Single(ProcessRules(nameof(RulesForm.DataAnnotations))!);
        Assert.Null(rule.TextKey);
        Assert.Equal("Plain", rule.Message);
    }

    [Fact]
    public void ValidationRules_Leave_Out_The_Ones_Only_Checked_On_The_Server_And_Have_The_Implied_Ones()
    {
        Assert.Null(ProcessRules(nameof(RulesForm.ServerOnly)));
        Assert.Equal("Email", Assert.Single(ProcessRules(nameof(RulesForm.Implied))!).Rule);
        Assert.Null(ProcessRules(nameof(RulesForm.None)));
    }
}
