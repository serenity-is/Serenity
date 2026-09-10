namespace Serenity.Data;

public class RowFieldsBaseTests
{
    #region test rows

    public class PlainRow : Row<PlainRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    public class StorageRow : Row<StorageRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public RowFields() : base("explicit_table", "f_")
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    public class SchemaOnlyRow : Row<SchemaOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base("sales.orders")
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public class DatabaseSchemaRow : Row<DatabaseSchemaRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base("dbsrv.dbo.orders")
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    [TableName("OverriddenTable")]
    public class AttrTableNameRow : Row<AttrTableNameRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    [TableName("OtherTable")]
    public class WrongTableNameAttrRow : Row<WrongTableNameAttrRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base("other")
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    [ConnectionKey("MyConn"), Module("MyModule"), LocalTextPrefix("MyPrefix")]
    public class AttributeMetadataRow : Row<AttributeMetadataRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CustomTestAttribute(string value) : Attribute
    {
        public string Value = value;
    }

    public class FlaggedRow : Row<FlaggedRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Test;
            public StringField Test2;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Test = new StringField(this, "Test");
                Test2 = new StringField(this, "Test2");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [CustomTest("x")]
        public string? Test { get => fields.Test[this]; set => fields.Test[this] = value; }

        [LookupInclude]
        public string? Test2 { get => fields.Test2[this]; set => fields.Test2[this] = value; }
    }

    public class JoinRow : Row<JoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field CountryID;
            public StringField Country;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                CountryID = new Int32Field(this, "CountryID");
                Country = new StringField(this, "Country");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ForeignKey("COUNTRY", "ID"), LeftJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Expression("c.DisplayName")]
        public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }
    }

    #endregion

    [Fact]
    public void ParseDatabaseAndSchema_Variants()
    {
        Assert.Null(RowFieldsBase.ParseDatabaseAndSchema(null, out string? db1, out string? schema1));
        Assert.Null(db1);
        Assert.Null(schema1);

        Assert.Equal("Plain", RowFieldsBase.ParseDatabaseAndSchema("Plain", out string? db2, out string? schema2));
        Assert.Null(db2);
        Assert.Null(schema2);

        Assert.Equal("Orders", RowFieldsBase.ParseDatabaseAndSchema("Sales.Orders", out string? db3, out string? schema3));
        Assert.Null(db3);
        Assert.Equal("Sales", schema3);

        Assert.Equal("Orders", RowFieldsBase.ParseDatabaseAndSchema("db.Sales.Orders", out string? db4, out string? schema4));
        Assert.Equal("db", db4);
        Assert.Equal("Sales", schema4);
    }

    [Fact]
    public void TableName_WithoutAttribute_UsesRowNameMinusRowSuffix()
    {
        var f = new PlainRow.RowFields();
        Assert.Equal("Plain", f.TableName);
    }

    [Fact]
    public void CtorArguments_TableNameAndFieldPrefix()
    {
        var f = new StorageRow.RowFields();
        Assert.Equal("explicit_table", f.TableName);
        Assert.Equal("f_", f.FieldPrefix);
        Assert.Equal("T0", f.AliasName);
    }

    [Fact]
    public void Ctor_ParsesSchema()
    {
        var f = new SchemaOnlyRow.RowFields();
        Assert.Equal("orders", f.TableOnly);
        Assert.Null(f.Database);
        Assert.Equal("sales", f.Schema);
        Assert.Equal("sales.orders", f.TableName);
    }

    [Fact]
    public void Ctor_ParsesDatabaseAndSchema()
    {
        var f = new DatabaseSchemaRow.RowFields();
        Assert.Equal("dbsrv.dbo.orders", f.TableName);
        Assert.Equal("orders", f.TableOnly);
        Assert.Equal("dbsrv", f.Database);
        Assert.Equal("dbo", f.Schema);
    }

    [Fact]
    public void TableName_AttributeOverridesDefault()
    {
        Assert.Equal("OverriddenTable", new AttrTableNameRow.RowFields().TableName);
    }

    [Fact]
    public void TableName_AttributeOnExplicitName_Throws()
    {
        Assert.Throws<InvalidProgramException>(() => new WrongTableNameAttrRow.RowFields());
    }

    [Fact]
    public void AttributeMetadata_FromAttributes()
    {
        var f = new AttributeMetadataRow.RowFields();
        Assert.Equal("MyConn", f.ConnectionKey);

        f.Initialize(null, SqlSettings.DefaultDialect);
        Assert.Equal("MyModule", f.ModuleIdentifier);
        Assert.Equal("MyPrefix", f.LocalTextPrefix);
        Assert.Equal("MyConn.AttributeMetadata", f.GenerationKey);
    }

    [Fact]
    public void DefaultMetadata_WithoutAttributes()
    {
        var f = new PlainRow.RowFields();
        Assert.Equal("Default", f.ConnectionKey);

        f.Initialize(null, SqlSettings.DefaultDialect);
        Assert.Equal("Data", f.ModuleIdentifier);
        Assert.Equal("Data.Plain", f.LocalTextPrefix);
        Assert.Equal("Data.Plain", f.RowIdentifier);
        Assert.Equal("Default.Plain", f.GenerationKey);
    }

    [Fact]
    public void Initialize_SecondCall_IsIgnored()
    {
        var f = new PlainRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);
        Assert.Equal(2, f.Count);
        f.Initialize(null, SqlSettings.DefaultDialect);
        Assert.Equal(2, f.Count);
    }

    [Fact]
    public void FindField_NullUnknownAndMatch()
    {
        var f = new PlainRow.RowFields();
        Assert.Null(f.FindField(null));
        Assert.Null(f.FindField("unknown"));
        Assert.Null(f.FindFieldByPropertyName(null));
        Assert.Null(f.FindFieldByPropertyName("Unknown"));

        f.Initialize(null, SqlSettings.DefaultDialect);
        Assert.Equal(f.Name, f.FindField("name"));
        Assert.Equal(f.Name, f.FindFieldByPropertyName("Name"));
        Assert.Equal(f.Id, f.FindFieldByPropertyName("Id"));
    }

    [Fact]
    public void Modification_AfterInitialization_Throws()
    {
        var f = new PlainRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Throws<InvalidOperationException>(() => f.RemoveAt(0));
        Assert.Throws<InvalidOperationException>(() => f[0] = f.Id);
    }

    [Fact]
    public void InsertItem_DuplicateName_Throws()
    {
        var f = new PlainRow.RowFields();

        _ = new Int32Field(f, "dup_name");
        Assert.Equal(3, f.Count);

        Assert.Throws<ArgumentOutOfRangeException>(() => new Int32Field(f, "dup_name"));
    }

    [Fact]
    public void ReplaceAliasWith_GuardsAndBehavior()
    {
        var f = new PlainRow.RowFields();
        Assert.Throws<ArgumentNullException>(() => f.ReplaceAliasWith(""));

        f.ReplaceAliasWith("t1");
        Assert.Equal("t1", f.AliasName);

        Assert.Throws<InvalidOperationException>(() => f.ReplaceAliasWith("t2"));
    }

    [Fact]
    public void GetFieldsByAttribute_CustomAttribute()
    {
        var f = new FlaggedRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        var testFields = f.GetFieldsByAttribute(typeof(CustomTestAttribute));
        Assert.Single(testFields);
        Assert.Equal(f.Test, testFields[0]);

        Assert.Single(f.GetFieldsByAttribute<LookupIncludeAttribute>());

        var cached = f.GetFieldsByAttribute(typeof(CustomTestAttribute));
        Assert.Same(testFields, cached);
    }

    [Fact]
    public void LookupIncludeAndInferTextualFields()
    {
        var flaggedFields = new FlaggedRow.RowFields();
        flaggedFields.Initialize(null, SqlSettings.DefaultDialect);
        Assert.True(flaggedFields.Test2.IsLookup);

        var f = new JoinRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Equal(3, f.Count);
        Assert.Equal("COUNTRY", f.CountryID.ForeignTable);
        Assert.Equal("ID", f.CountryID.ForeignField);
        Assert.NotNull(f.CountryID.ForeignJoinAlias);
        Assert.Single(f.Joins);

        var query = new SqlQuery().From(f).Select(f.Id).Select(f.Country);
        Assert.Contains("c.DisplayName", query.ToString(), StringComparison.Ordinal);
    }

    #region more test rows

    public class AmbiguousJoinColumnRow : Row<AmbiguousJoinColumnRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("COUNTRY", "ID"), LeftJoin("c"), LeftJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    public class AmbiguousExpressionRow : Row<AmbiguousExpressionRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Test;
            public RowFields() : base()
            {
                Test = new StringField(this, "Test");
            }
        }

        [Expression("A"), Expression("B")]
        public string? Test { get => fields.Test[this]; set => fields.Test[this] = value; }
    }

    public class NotARowWontWork
    {
        public class BadFields : RowFieldsBase
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NestedRowAttribute : Attribute { }

    public class OuterFields : RowFieldsBase
    {
    }

    public class PrimaryKeyRow : Row<PrimaryKeyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
            }
        }

        [PrimaryKey, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [SortOrder(-1)]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    #endregion

    [Fact]
    public void TopLevelFieldsType_Throws()
    {
        Assert.Throws<InvalidProgramException>(() => new OuterFields());
    }

    [Fact]
    public void RowFields_InsideNonRowType_Throws()
    {
        Assert.Throws<InvalidProgramException>(() => new NotARowWontWork.BadFields());
    }

    [Fact]
    public void AmbiguousLeftJoin_ThrowsAmbiguousMatchException()
    {
        Assert.Throws<AmbiguousMatchException>(() =>
        {
            var f = new AmbiguousJoinColumnRow.RowFields();
            f.Initialize(null, SqlSettings.DefaultDialect);
        });
    }

    [Fact]
    public void AmbiguousExpression_ThrowsAmbiguousMatchException()
    {
        Assert.Throws<AmbiguousMatchException>(() =>
        {
            var f = new AmbiguousExpressionRow.RowFields();
            f.Initialize(null, SqlSettings.DefaultDialect);
        });
    }

    [Fact]
    public void PrimaryKeys_IdField_AndSortOrders()
    {
        var f = new PrimaryKeyRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        var pks = f.PrimaryKeys;
        Assert.Single(pks);
        Assert.Equal(f.Id, pks[0]);
        Assert.Equal(f.Id, f.IdField);

        var sort = f.SortOrders;
        Assert.Single(sort);
        Assert.Equal(f.Name, sort[0].Item1);
        Assert.True(sort[0].Item2); // negative sort order => descending

        f.GenerationKey = "custom";
        Assert.Equal("custom", f.GenerationKey);
    }

    [Fact]
    public void RemoveAndSetItem_BeforeInitialization_Works()
    {
        var f = new PlainRow.RowFields();

        var removedName = f.Name;
        f.Remove(removedName);
        Assert.Single(f);
        Assert.Equal(-1, removedName.Index);
        Assert.Null(removedName.Fields);

        var replaced = new Int32Field(null, "Replaced");
        f[0] = replaced;
        Assert.Equal(0, replaced.Index);
        Assert.Same(f, replaced.Fields);
        Assert.Null(f.FindField("Id"));
    }

    public class AliasReplaceJoinRow : Row<AliasReplaceJoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field CountryID;
            public StringField Country;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                CountryID = new Int32Field(this, "CountryID");
                Country = new StringField(this, "Country");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [ForeignKey("COUNTRY", "ID"), LeftJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }

        [Expression("c.DisplayName")]
        public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }
    }

    [Fact]
    public void ReplaceAliasWith_JoinAliasesArePrefixedAndLocked()
    {
        var f = new AliasReplaceJoinRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        f.ReplaceAliasWith("base");

        Assert.Equal("base", f.AliasName);
        Assert.Equal("base_c", f.Joins["base_c"].Name);
        Assert.Equal("base_c.DisplayName", f.Country.Expression);
        Assert.Throws<InvalidOperationException>(() => f.ReplaceAliasWith("other2"));
    }
}
