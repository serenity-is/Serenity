namespace Serenity.Data;

using Serenity.Data.Mapping;
using SelfNavigationRow = EntitySqlQueryProjection_Tests.SelfNavigationRow;

public class EntitySqlQueryExtensions_Joins_Tests
{
    [Fact]
    public void JoinViaUsesForeignJoinAliasAndPreservesRealiasedSource()
    {
        var query = new SqlQuery().From("Anchor", new Alias("Anchor", "T0"));
        query.From(new SelfNavigationRow(), out var sourceFields);

        query.LeftJoinVia<SelfNavigationRow.RowFields>(sourceFields.ManagerID, out var managerFields);

        Assert.Equal("T1", sourceFields.AliasName);
        Assert.Equal("T2", managerFields.AliasName);
        var sql = query.ToString();
        Assert.True(sql.Contains("LEFT JOIN [SelfNavigation] T2 ON (T2.[ID] = T1.[ManagerID])", StringComparison.Ordinal), sql);
    }

    [Fact]
    public void JoinViaFallsBackToForeignKeyMetadata()
    {
        var query = new SqlQuery().From(ForeignKeyOnlyRow.Fields, out var sourceFields);
        query.LeftJoinVia<SelfNavigationRow.RowFields>(sourceFields.RelatedID, out var leftFields);
        query.RightJoinVia<SelfNavigationRow.RowFields>(sourceFields.RelatedID, out var rightFields);
        query.InnerJoinVia<SelfNavigationRow.RowFields>(sourceFields.RelatedID, out var innerFields);

        Assert.Equal("T1", leftFields.AliasName);
        Assert.Equal("T2", rightFields.AliasName);
        Assert.Equal("T3", innerFields.AliasName);

        var sql = query.ToString();
        Assert.Contains("LEFT JOIN [SelfNavigation] T1 ON (T1.[ID] = T0.[RelatedID])", sql);
        Assert.Contains("RIGHT JOIN [SelfNavigation] T2 ON (T2.[ID] = T0.[RelatedID])", sql);
        Assert.Contains("INNER JOIN [SelfNavigation] T3 ON (T3.[ID] = T0.[RelatedID])", sql);
    }

    [Fact]
    public void JoinViaRejectsFieldsForDifferentTargetRowType()
    {
        var query = new SqlQuery().From(ForeignKeyOnlyRow.Fields, out var sourceFields);

        var error = Assert.Throws<ArgumentException>(() =>
            query.LeftJoinVia<OtherTargetRow.RowFields>(sourceFields.RelatedID, out _));

        Assert.Contains(nameof(ForeignKeyOnlyRow.RelatedID), error.Message);
        Assert.Contains(typeof(ForeignKeyOnlyRow.RowFields).FullName!, error.Message);
        Assert.Contains(typeof(OtherTargetRow.RowFields).FullName!, error.Message);
    }

    [TableName("ForeignKeyOnly")]
    public sealed class ForeignKeyOnlyRow : Row<ForeignKeyOnlyRow.RowFields>
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [ForeignKey(typeof(SelfNavigationRow), nameof(SelfNavigationRow.ID))]
        public int? RelatedID { get => fields.RelatedID[this]; set => fields.RelatedID[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
            public Int32Field RelatedID = null!;
        }
    }

    [TableName("OtherTarget")]
    public sealed class OtherTargetRow : Row<OtherTargetRow.RowFields>
    {
        [Identity, IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field ID = null!;
        }
    }
}