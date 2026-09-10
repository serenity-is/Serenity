#pragma warning disable CS0649
#pragma warning disable CS0169

namespace Serenity.Data;

public class RowFieldsBaseTestsMore
{
    #region test rows

    public class ExtraFieldRow : Row<ExtraFieldRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Extra;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Extra = new StringField(this, "Extra");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public class GenRow : Row<GenRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
            }
        }

        private int? _Id;
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public class ColumnMismatchRow : Row<ColumnMismatchRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Name;
            public RowFields() : base()
            {
                Name = new StringField(this, "Name");
            }
        }

        [Column("Other")]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    public class MultipleIdRow : Row<MultipleIdRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field Other;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Other = new Int32Field(this, "Other");
            }
        }

        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [IdProperty]
        public int? Other { get => fields.Other[this]; set => fields.Other[this] = value; }
    }

    public class MultipleNameRow : Row<MultipleNameRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Name;
            public StringField Other;
            public RowFields() : base()
            {
                Name = new StringField(this, "Name");
                Other = new StringField(this, "Other");
            }
        }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        [NameProperty]
        public string? Other { get => fields.Other[this]; set => fields.Other[this] = value; }
    }

    public class PrimaryKeyOnlyRow : Row<PrimaryKeyOnlyRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
            }
        }

        [PrimaryKey]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public enum TestStatus { None = 0, Active = 1 }

    public class EnumRow : Row<EnumRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Status;
            public Int32Field StatusN;
            public RowFields() : base()
            {
                Status = new Int32Field(this, "Status");
                StatusN = new Int32Field(this, "StatusN");
            }
        }

        public TestStatus Status
        {
            get => (TestStatus)(fields.Status[this] ?? 0);
            set => fields.Status[this] = (int)value;
        }

        public TestStatus? StatusN
        {
            get => (TestStatus?)fields.StatusN[this];
            set => fields.StatusN[this] = (int?)value;
        }
    }

    [FieldReadPermission("Perm", ApplyToLookups = false)]
    public class PermissionRow : Row<PermissionRow.RowFields>, IIdRow, INameRow
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Name;
            public StringField Extra;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Name = new StringField(this, "Name");
                Extra = new StringField(this, "Extra");
            }
        }

        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string? Extra { get => fields.Extra[this]; set => fields.Extra[this] = value; }
    }

    public class NoIdPropertyRow : Row<NoIdPropertyRow.RowFields>, IIdRow
    {
        public class RowFields : RowFieldsBase
        {
            public StringField Name;
            public RowFields() : base()
            {
                Name = new StringField(this, "Name");
            }
        }

        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    }

    public class NoNamePropertyRow : Row<NoNamePropertyRow.RowFields>, INameRow
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
            }
        }

        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public class LeftJoinNoForeignRow : Row<LeftJoinNoForeignRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [LeftJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    public class JoinNoForeignFieldRow : Row<JoinNoForeignFieldRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("COUNTRY", ""), LeftJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    public class InnerJoinRow : Row<InnerJoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("COUNTRY", "ID"), InnerJoin("c")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    public class TextualFieldRow : Row<TextualFieldRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("COUNTRY", "ID"), LeftJoin("c"), TextualField("CountryName")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    public class AmbiguousPropJoinRow : Row<AmbiguousPropJoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field CountryID;
            public RowFields() : base()
            {
                CountryID = new Int32Field(this, "CountryID");
            }
        }

        [ForeignKey("COUNTRY", "ID"), LeftJoin("c", "T1", "c.ID = T0.ID"), LeftJoin("c", "T2", "c.ID = T0.ID")]
        public int? CountryID { get => fields.CountryID[this]; set => fields.CountryID[this] = value; }
    }

    [LeftJoin("c", "Other", "A = B")]
    public class RowLevelJoinRow : Row<RowLevelJoinRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Country;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Country = new StringField(this, "Country");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Expression("c.Name")]
        public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }
    }

    public class FactoryCtorRow : Row<FactoryCtorRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
            }
        }

        public FactoryCtorRow() : base()
        {
        }

        public FactoryCtorRow(FactoryCtorRow.RowFields fields) : base(fields)
        {
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    }

    public class BadGenRow : Row<BadGenRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public RowFields() : base()
            {
            }
        }
    }

    public class UnknownAliasRow : Row<UnknownAliasRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField Country;
            public RowFields() : base()
            {
                Id = new Int32Field(this, "Id");
                Country = new StringField(this, "Country");
            }
        }

        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        [Expression("x.field")]
        public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }
    }

    #endregion

    [Fact]
    public void Extra_Field_Without_Property_Uses_Field_Level_Permissions()
    {
        var f = new ExtraFieldRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Same(f.Extra, f.FindField("Extra"));
        Assert.Null(f.FindFieldByPropertyName("Extra"));
    }

    [Fact]
    public void Runtime_Field_Creation_With_Backing_Field_Works()
    {
        var row = new GenRow();
        row.Id = 42;
        Assert.Equal(42, row.Id);
        row.Id = null;
        Assert.Null(row.Id);
    }

    [Fact]
    public void Field_Column_Name_Mismatch_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new ColumnMismatchRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void Multiple_IdProperty_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new MultipleIdRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void Multiple_NameProperty_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new MultipleNameRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void IdField_Inferred_From_Single_PrimaryKey()
    {
        var f = new PrimaryKeyOnlyRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Same(f.Id, f.IdField);
        var pk = Assert.Single(f.PrimaryKeys!);
        Assert.Same(f.Id, pk);
    }

    [Fact]
    public void Enum_Property_Types_Are_Assigned_To_Fields()
    {
        var f = new EnumRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Equal(typeof(TestStatus), f.Status.EnumType);
        Assert.Equal(typeof(TestStatus), f.StatusN.EnumType);
    }

    [Fact]
    public void FieldReadPermission_Applies_To_Fields_On_Row_Created()
    {
        var row = new PermissionRow();

        var f = row.GetFields();
        Assert.Equal("Perm", f.Extra.ReadPermission);
        Assert.Null(f.Id.ReadPermission);
        Assert.Null(f.Name.ReadPermission);
    }

    [Fact]
    public void IIdRow_Without_IdProperty_Throws_On_Creation()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NoIdPropertyRow());
    }

    [Fact]
    public void INameRow_Without_NameProperty_Throws_On_Creation()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NoNamePropertyRow());
    }

    [Fact]
    public void LeftJoin_Without_ForeignTable_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new LeftJoinNoForeignRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void LeftJoin_Without_ForeignField_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new JoinNoForeignFieldRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void InnerJoin_Creates_Join_With_OnCriteria()
    {
        var f = new InnerJoinRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Single(f.Joins);
        Assert.NotNull(f.CountryID.ForeignJoinAlias);
    }

    [Fact]
    public void TextualField_Attribute_Is_Assigned()
    {
        var f = new TextualFieldRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Equal("CountryName", f.CountryID.TextualField);
    }

    [Fact]
    public void Ambiguous_Property_Join_Attributes_Throw()
    {
        Assert.Throws<AmbiguousMatchException>(() =>
            new AmbiguousPropJoinRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void ReplaceAliasWith_Handles_Row_Level_Joins_And_Null_Criteria()
    {
        var f = new RowLevelJoinRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        f.Joins.Add("manual", new LeftJoin("Other", "manual", null));

        f.ReplaceAliasWith("base");

        var sql = new SqlQuery().From(f).Select(f.Id).Select(f.Country).ToString();
        Assert.Contains("base_c", sql);
    }

    [Fact]
    public void ReplaceAliasWith_Same_Alias_Only_Locks()
    {
        var f = new ExtraFieldRow.RowFields();
        f.ReplaceAliasWith(f.AliasName);
        Assert.Throws<InvalidOperationException>(() => f.ReplaceAliasWith("other"));
    }

    [Fact]
    public void FieldCollection_Modifications_Before_Initialization()
    {
        var f = new ExtraFieldRow.RowFields();
        Assert.Equal(2, f.Count);

        f.RemoveAt(0);
        Assert.Single(f);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            f[0] = new Int32Field(null, "Extra"));

        var replaced = new Int32Field(null, "Replaced");
        f[0] = replaced;
        Assert.Same(replaced, f.FindField("Replaced"));

        Assert.Throws<ArgumentNullException>(() => f.Add(null!));
    }

    [Fact]
    public void FieldCollection_Modification_After_Initialization_Throws()
    {
        var f = new ExtraFieldRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);

        Assert.Throws<InvalidOperationException>(() => f.Add(new Int32Field(null, "X")));
    }

    [Fact]
    public void PropertyReadError_For_Uninitialized_Fields()
    {
        var f = new ExtraFieldRow.RowFields();
        Assert.Throws<InvalidOperationException>(() => _ = f.Dialect);
    }

    [Fact]
    public void FieldPrefix_And_LocalTextPrefix_Setters_Work()
    {
        var f = new ExtraFieldRow.RowFields();
        f.FieldPrefix = "f_";
        Assert.Equal("f_", f.FieldPrefix);

        f.LocalTextPrefix = "Custom";
        Assert.Equal("Custom", f.LocalTextPrefix);
    }

    [Fact]
    public void Row_Factory_Uses_Fields_Constructor()
    {
        var row = new FactoryCtorRow();
        var created = ((IRow)row).CreateNew();
        Assert.IsType<FactoryCtorRow>(created);
    }

    [Fact]
    public void TopLevel_Fields_Type_Throws()
    {
        Assert.Throws<InvalidProgramException>(() => new TopLevelFieldsForTest());
    }

    [Fact]
    public void Null_Field_Without_Property_Throws()
    {
        Assert.Throws<InvalidProgramException>(() =>
            new BadGenRow.RowFields().Initialize(null, SqlSettings.DefaultDialect));
    }

    [Fact]
    public void ReplaceAliasWith_Keeps_Unknown_Aliases()
    {
        var f = new UnknownAliasRow.RowFields();
        f.Initialize(null, SqlSettings.DefaultDialect);
        f.ReplaceAliasWith("base");

        Assert.Equal("x.field", f.Country.Expression);
    }

    [Fact]
    public void IAlias_NameDot_Is_Exposed()
    {
        var f = new ExtraFieldRow.RowFields();
        Assert.Equal("T0.", ((IAlias)f).NameDot);
    }

    [Fact]
    public void ModuleIdentifier_Strips_Entities_Namespace_Suffix()
    {
        var f = new Serenity.Data.Tests.Entities.EntitiesNamespaceRow.RowFields();
        Assert.Equal("Data.Tests", f.ModuleIdentifier);
    }
}

public class TopLevelFieldsForTest : RowFieldsBase
{
}
