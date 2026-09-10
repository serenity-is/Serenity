namespace Serenity.Data;

public class OriginPropertyTests
{
    #region Rows used in tests

    /// <summary>
    /// Origin row used by most tests. Contains properties with display name,
    /// size, scale, a renamed column and a nested origin (RegionName) to
    /// exercise all OriginPropertyDictionary behaviors.
    /// </summary>
    public class CountryRow : Row<CountryRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public StringField Code;
            public DecimalField Rate;
            public StringField Renamed;
            public Int32Field RegionID;
            public StringField RegionName;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
                Code = new StringField(this, "Code");
                Rate = new DecimalField(this, "Rate");
                Renamed = new StringField(this, "CustomCol");
                RegionID = new Int32Field(this, "RegionID");
                RegionName = new StringField(this, "RegionName");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [DisplayName("Country Name")]
        public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [Size(100)]
        public string Code { get => fields.Code[this]; set => fields.Code[this] = value; }

        [Scale(2)]
        public decimal? Rate { get => fields.Rate[this]; set => fields.Rate[this] = value; }

        [Column("CustomCol")]
        public string Renamed { get => fields.Renamed[this]; set => fields.Renamed[this] = value; }

        [ForeignKey("TheRegionTable", "RegionID"), LeftJoin("jr", RowType = typeof(RegionRow))]
        public int? RegionID { get => fields.RegionID[this]; set => fields.RegionID[this] = value; }

        [Origin("jr", "Title")]
        public string RegionName { get => fields.RegionName[this]; set => fields.RegionName[this] = value; }
    }

    public class RegionRow : Row<RegionRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Title;

            public RowFields()
            {
                Id = new Int32Field(this, "Id");
                Title = new StringField(this, "Title");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public string Title { get => fields.Title[this]; set => fields.Title[this] = value; }
    }

    public class OriginBasicRow : Row<OriginBasicRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("Country", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("Country", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        [Origin("CountryID", "Name")]
        public string CountryNameByProperty { get => fields.CountryNameByProperty[this]; set => fields.CountryNameByProperty[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;
            public StringField CountryNameByProperty;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
                CountryNameByProperty = new StringField(this, "CountryNameByProperty");
            }
        }
    }

    public class OriginColumnRow : Row<OriginColumnRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("cc", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("cc", "Renamed")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        [Origin("cc", "Renamed"), Column("CustomCol2")]
        public string CountryRenamed { get; set; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;
            public StringField CountryRenamed;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
            }
        }
    }

    [LeftJoin("dr", "TheCountryTable", "dr.[Id] = T0.[SomeID]", RowType = typeof(CountryRow), TitlePrefix = "Row Title")]
    public class OriginDisplayNameRow : Row<OriginDisplayNameRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("dn", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("dn", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        [ForeignKey("TheCountryTable", "CountryID2"), LeftJoin("dt", RowType = typeof(CountryRow), TitlePrefix = "Land")]
        public int? CountryID2 { get => fields.CountryID2[this]; set => fields.CountryID2[this] = value; }

        [Origin("dt", "Name")]
        public string CountryTitle { get => fields.CountryTitle[this]; set => fields.CountryTitle[this] = value; }

        [DisplayName("The Country")]
        [ForeignKey("TheCountryTable", "CountryID3"), LeftJoin("dd", RowType = typeof(CountryRow))]
        public int? CountryID3 { get => fields.CountryID3[this]; set => fields.CountryID3[this] = value; }

        [Origin("dd", "Name")]
        public string CountryDisplay { get => fields.CountryDisplay[this]; set => fields.CountryDisplay[this] = value; }

        [Origin("dr", "Name")]
        public string CountryRowTitle { get => fields.CountryRowTitle[this]; set => fields.CountryRowTitle[this] = value; }

        public int? SomeID { get => fields.SomeID[this]; set => fields.SomeID[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;
            public Int32Field CountryID2;
            public StringField CountryTitle;
            public Int32Field CountryID3;
            public StringField CountryDisplay;
            public StringField CountryRowTitle;
            public Int32Field SomeID;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
                CountryID2 = new Int32Field(this, "CountryID2");
                CountryTitle = new StringField(this, "CountryTitle");
                CountryID3 = new Int32Field(this, "CountryID3");
                CountryDisplay = new StringField(this, "CountryDisplay");
                CountryRowTitle = new StringField(this, "CountryRowTitle");
                SomeID = new Int32Field(this, "SomeID");
            }
        }
    }

    public class OriginNestedRow : Row<OriginNestedRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("j1", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("j1", "RegionName")]
        public string CountryRegionName { get => fields.CountryRegionName[this]; set => fields.CountryRegionName[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryRegionName;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryRegionName = new StringField(this, "CountryRegionName");
            }
        }
    }

    public class OriginAttributePropagationRow : Row<OriginAttributePropagationRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("ap", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("ap", "Code")]
        public string CountryCode { get => fields.CountryCode[this]; set => fields.CountryCode[this] = value; }

        [Origin("ap", "Rate")]
        public decimal? CountryRate { get => fields.CountryRate[this]; set => fields.CountryRate[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryCode;
            public DecimalField CountryRate;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryCode = new StringField(this, "CountryCode");
                CountryRate = new DecimalField(this, "CountryRate");
            }
        }
    }

    public class OriginMissingJoinRow : Row<OriginMissingJoinRow.RowFields>
    {
        [Origin("Missing", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField CountryName;

            public RowFields()
            {
                CountryName = new StringField(this, "CountryName");
            }
        }
    }

    public class OriginNoRowTypeRow : Row<OriginNoRowTypeRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("nr")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("nr", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

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
    }

    [LeftJoin("rn", "TheCountryTable", "rn.[Id] = 1")]
    public class OriginRowJoinNoRowTypeRow : Row<OriginRowJoinNoRowTypeRow.RowFields>
    {
        [Origin("rn", "Name")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField CountryName;

            public RowFields()
            {
                CountryName = new StringField(this, "CountryName");
            }
        }
    }

    public class OriginPrefixRow : Row<OriginPrefixRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryID"), LeftJoin("p9", RowType = typeof(CountryRow))]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Origin("p9")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        [Origin("p9")]
        public string CountryCode { get => fields.CountryCode[this]; set => fields.CountryCode[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public StringField CountryName;
            public StringField CountryCode;

            public RowFields()
            {
                CountryID = new Int32Field(this, "CountryID");
                CountryName = new StringField(this, "CountryName");
                CountryCode = new StringField(this, "CountryCode");
            }
        }
    }

    public class OriginPrefixFromPropertiesRow : Row<OriginPrefixFromPropertiesRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryRef"), LeftJoin("p9b", RowType = typeof(CountryRow))]
        public int? CountryRef { get => fields.CountryRef[this]; set => fields.CountryRef[this] = value; }

        [Origin("p9b")]
        public string CountryName { get => fields.CountryName[this]; set => fields.CountryName[this] = value; }

        [Origin("p9b")]
        public string CountryCode { get => fields.CountryCode[this]; set => fields.CountryCode[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryRef;
            public StringField CountryName;
            public StringField CountryCode;

            public RowFields()
            {
                CountryRef = new Int32Field(this, "CountryRef");
                CountryName = new StringField(this, "CountryName");
                CountryCode = new StringField(this, "CountryCode");
            }
        }
    }

    public class OriginPrefixAttributeRow : Row<OriginPrefixAttributeRow.RowFields>
    {
        [ForeignKey("TheCountryTable", "CountryRef"), LeftJoin("p9c", RowType = typeof(CountryRow), PropertyPrefix = "Pfx")]
        public int? CountryRef { get => fields.CountryRef[this]; set => fields.CountryRef[this] = value; }

        [Origin("p9c")]
        public string PfxName { get => fields.PfxName[this]; set => fields.PfxName[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryRef;
            public StringField PfxName;

            public RowFields()
            {
                CountryRef = new Int32Field(this, "CountryRef");
                PfxName = new StringField(this, "PfxName");
            }
        }
    }

    #endregion

    [Fact]
    public void Origin_Expression_Uses_Join_Alias_And_Origin_Property_Name()
    {
        var fields = new OriginBasicRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("Country.[Name]", fields.CountryName.Expression);
        Assert.Equal("Country.[Name]", fields.CountryNameByProperty.Expression);

        var join = Assert.Contains("Country", fields.Joins);
        Assert.Equal("TheCountryTable", join.Table);
    }

    [Fact]
    public void Origin_Expression_Uses_Column_Name_Of_Origin_Property()
    {
        var fields = new OriginColumnRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("cc.[CustomCol]", fields.CountryName.Expression);
        Assert.Equal("CountryID Renamed", fields.CountryName.Caption?.Key);

        // property with a [Column] attribute but no pre-created field gets
        // its field auto created with the column name
        Assert.Equal("CustomCol2", fields.CountryRenamed.Name);
        Assert.Equal("cc.[CustomCol]", fields.CountryRenamed.Expression);
    }

    [Fact]
    public void Origin_DisplayName_Is_Prefixed_With_Join_Property_Name()
    {
        var fields = new OriginDisplayNameRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        // no TitlePrefix / DisplayName on join property -> property name is used
        Assert.Equal("CountryID Country Name", fields.CountryName.Caption?.Key);

        // TitlePrefix on the join property
        Assert.Equal("Land Country Name", fields.CountryTitle.Caption?.Key);

        // DisplayName on the join property
        Assert.Equal("The Country Country Name", fields.CountryDisplay.Caption?.Key);

        // TitlePrefix on a row level join declaration
        Assert.Equal("Row Title Country Name", fields.CountryRowTitle.Caption?.Key);
    }

    [Fact]
    public void Origin_Expression_For_Nested_Origin_Prefixes_Join_Aliases()
    {
        var fields = new OriginNestedRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        // CountryRow.RegionName has [Origin("jr", "Title")] so the origin of
        // the origin is resolved with a prefixed join alias
        Assert.Equal("j1_jr.[Title]", fields.CountryRegionName.Expression);

        var join = Assert.Contains("j1_jr", fields.Joins);
        Assert.Equal("TheRegionTable", join.Table);
    }

    [Fact]
    public void Origin_Propagates_Size_And_Scale_Attributes()
    {
        var fields = new OriginAttributePropagationRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("ap.[Code]", fields.CountryCode.Expression);
        Assert.Equal(100, fields.CountryCode.Size);

        Assert.Equal("ap.[Rate]", fields.CountryRate.Expression);
        Assert.Equal(2, fields.CountryRate.Scale);
    }

    [Fact]
    public void Raises_Exception_If_Join_Declaration_For_Origin_Is_Not_Found()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new OriginMissingJoinRow.RowFields().Initialize(null, SqlServer2012Dialect.Instance);
        });

        Assert.Contains(nameof(OriginMissingJoinRow), ex.Message);
        Assert.Contains("'CountryName'", ex.Message);
        Assert.Contains("declaration of join", ex.Message);
        Assert.Contains("'Missing'", ex.Message);
    }

    [Fact]
    public void Raises_Exception_If_Join_Property_Has_No_RowType()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new OriginNoRowTypeRow.RowFields().Initialize(null, SqlServer2012Dialect.Instance);
        });

        Assert.Contains(nameof(OriginNoRowTypeRow), ex.Message);
        Assert.Contains("'CountryName'", ex.Message);
        Assert.Contains("doesn't use a typeof", ex.Message);
    }

    [Fact]
    public void Raises_Exception_If_Row_Join_Declaration_Has_No_RowType()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new OriginRowJoinNoRowTypeRow.RowFields().Initialize(null, SqlServer2012Dialect.Instance);
        });

        Assert.Contains(nameof(OriginRowJoinNoRowTypeRow), ex.Message);
        Assert.Contains("'CountryName'", ex.Message);
        Assert.Contains("has no RowType", ex.Message);
    }

    [Fact]
    public void Origin_Property_Prefix_Is_Determined_From_Join_Property_ID_Suffix()
    {
        var fields = new OriginPrefixRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        // join property is "CountryID" so "Country" prefix is stripped
        Assert.Equal("p9.[Name]", fields.CountryName.Expression);
        Assert.Equal("p9.[Code]", fields.CountryCode.Expression);
    }

    [Fact]
    public void Origin_Property_Prefix_Is_Determined_From_Common_Prefix_Of_Origin_Properties()
    {
        var fields = new OriginPrefixFromPropertiesRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        // join property "CountryRef" has no ID suffix so the common prefix of
        // the origin property names ("CountryName", "CountryCode") is used
        Assert.Equal("p9b.[Name]", fields.CountryName.Expression);
        Assert.Equal("p9b.[Code]", fields.CountryCode.Expression);
    }

    [Fact]
    public void Origin_Property_Prefix_Is_Taken_From_Join_PropertyPrefix()
    {
        var fields = new OriginPrefixAttributeRow.RowFields();
        fields.Initialize(null, SqlServer2012Dialect.Instance);

        Assert.Equal("p9c.[Name]", fields.PfxName.Expression);
    }
}
