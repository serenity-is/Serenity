namespace Serenity.Data;

public class OriginPropertyTestsMore
{
    #region test rows

    [LeftJoin("rj", "TheCountryTable", "rj.[Id] = 1", RowType = typeof(OriginPropertyTests.CountryRow), PropertyPrefix = "Pfx")]
    public class OriginRowJoinPrefixRow : Row<OriginRowJoinPrefixRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField CountryName;

            public RowFields()
            {
                CountryName = new StringField(this, "CountryName");
            }
        }

        [Origin("rj", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }
    }

    public class OriginSinglePropertyRow : Row<OriginSinglePropertyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField Name;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                Name = new StringField(this, "Name");
            }
        }

        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("sp", RowType = typeof(OriginPropertyTests.CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("sp")]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    public class OriginMissingPropertyRow : Row<OriginMissingPropertyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
            }
        }

        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("mp", RowType = typeof(OriginPropertyTests.CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("mp", "MissingProperty")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }
    }

    public class ExprOriginRow : Row<ExprOriginRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField ExprProp;

            public RowFields()
            {
                ExprProp = new StringField(this, "ExprProp");
            }
        }

        [Expression("X")]
        public string ExprProp { get => fields.ExprProp[this]; set => fields.ExprProp[this] = value; }
    }

    public class OriginFromExpressionRow : Row<OriginFromExpressionRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
            }
        }

        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("eo", RowType = typeof(ExprOriginRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("eo", "ExprProp")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }
    }

    #endregion

    [Fact]
    public void Row_Join_PropertyPrefix_Is_Used_For_Origin_Alias()
    {
        var fields = new OriginRowJoinPrefixRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("rj.[Name]", fields.CountryName.Expression);
    }

    [Fact]
    public void Origin_Without_Property_Uses_Property_Name_When_No_Prefix()
    {
        var fields = new OriginSinglePropertyRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("sp.[Name]", fields.Name.Expression);
    }

    [Fact]
    public void Origin_Property_Not_Found_Throws()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OriginMissingPropertyRow.RowFields().Initialize(null, SqlServer2012Dialect.Instance));

        Assert.Contains("'MissingProperty'", ex.Message);
    }

    [Fact]
    public void Origin_Expression_Uses_Expression_Attribute_Of_Origin_Property()
    {
        var fields = new OriginFromExpressionRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("X", fields.CountryName.Expression);
    }

    [Fact]
    public void PrefixAliases_Guards_Arguments()
    {
        var dictionary = OriginPropertyDictionary.GetPropertyDictionary(typeof(OriginPropertyTests.CountryRow));
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        Assert.Equal("  ", dictionary.PrefixAliases("  ", "a", selector, []));
        Assert.Throws<ArgumentNullException>(() => dictionary.PrefixAliases("x", " ", selector, []));
    }

    [LeftJoin("lj", "TableL", "1 = 1", RowType = typeof(OriginPropertyTests.CountryRow))]
    [InnerJoin("ij", "TableI", "1 = 1", RowType = typeof(OriginPropertyTests.CountryRow))]
    [OuterApply("oa", "SELECT 1", RowType = typeof(OriginPropertyTests.CountryRow))]
    public class ExprJoinsOriginRow : Row<ExprJoinsOriginRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField ExprProp;
            public Int32Field PjCol;
            public Int32Field PjExpr;
            public StringField PjOrigin;

            public RowFields()
            {
                ExprProp = new StringField(this, "ExprProp");
                PjCol = new Int32Field(this, "PjCol");
                PjExpr = new Int32Field(this, "PjExpr");
                PjOrigin = new StringField(this, "PjOrigin");
            }
        }

        [Expression("lj.X + lj.Z + ij.X + oa.X + T0.Y + pj.X + zz.W + pj2.X + pj3.X")]
        public string ExprProp { get => fields.ExprProp[this]; set => fields.ExprProp[this] = value; }

        [Column("CustomCol"), ForeignKey("TheCountryTable", "CountryID"), LeftJoin("pj", RowType = typeof(OriginPropertyTests.CountryRow))]
        public int? PjCol { get => fields.PjCol[this]; set => fields.PjCol[this] = value; }

        [Expression("T0.Id"), ForeignKey("TheCountryTable", "CountryID2"), LeftJoin("pj2", RowType = typeof(OriginPropertyTests.CountryRow))]
        public int? PjExpr { get => fields.PjExpr[this]; set => fields.PjExpr[this] = value; }

        [Origin("lj", "Name"), ForeignKey("TheCountryTable", "CountryID3"), LeftJoin("pj3", RowType = typeof(OriginPropertyTests.CountryRow))]
        public string? PjOrigin { get => fields.PjOrigin[this]; set => fields.PjOrigin[this] = value; }
    }

    public class OriginFromJoinsExpressionRow : Row<OriginFromJoinsExpressionRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
            }
        }

        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("eo", RowType = typeof(ExprJoinsOriginRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("eo", "ExprProp")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }
    }

    [Fact]
    public void PrefixAliases_Maps_Row_And_Property_Join_Aliases()
    {
        var fields = new OriginFromJoinsExpressionRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Contains("eo_lj", fields.CountryName.Expression);
        Assert.Contains("eo_ij", fields.CountryName.Expression);
        Assert.Contains("eo_oa", fields.CountryName.Expression);
    }
}
