namespace Serenity.CodeGenerator
{
    public partial class ApplicationMetadataTests
    {
        [Fact]
        public void Reads_Row_Metadata_From_RowTypes()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Equal("ApplicationMetadataTest", row.Namespace);
            Assert.Equal("OrderRow", row.ClassName);
            Assert.Equal("ApplicationMetadataTest.OrderRow", row.FullName);
            Assert.Equal("OrdersModule", row.Module);
            Assert.True(row.HasLookupScriptAttribute);
            Assert.Equal("OrderID", row.IdProperty);
            Assert.Equal("OrderName", row.NameProperty);
            Assert.Equal("AppMeta/Order/List", row.ListServiceRoute);
        }

        [Fact]
        public void Caches_Resolved_Row_Metadata_By_Tablename()
        {
            var metadata = CreateMetadata();
            var row1 = metadata.GetRowByTablename("dbo.AppMetaOrders")!;
            var row2 = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Same(row1, row2);
        }

        [Fact]
        public void Matches_Quoted_TableName_IgnoreCase()
        {
            var metadata = CreateMetadata();
            Assert.NotNull(metadata.GetRowByTablename("[dbo].[appmetaorders]"));
        }

        [Fact]
        public void Matches_Single_Part_Tablename_Using_DefaultSchema()
        {
            var metadata = CreateMetadata(metadata =>
            {
                metadata.DefaultSchema = "dbo";
            });
            Assert.NotNull(metadata.GetRowByTablename("AppMetaOrders"));
        }

        [Fact]
        public void Matches_Row_By_Class_Name_When_TableName_Attribute_Is_Missing()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("CUSTOMER")!;

            Assert.Equal("CustomerRow", row.ClassName);
            Assert.Equal("ApplicationMetadataTest.CustomerRow", row.FullName);
        }

        [Fact]
        public void Returns_Null_And_Caches_For_Unknown_Table()
        {
            var metadata = CreateMetadata();
            Assert.Null(metadata.GetRowByTablename("NoSuchTable"));
            Assert.Null(metadata.GetRowByTablename("NoSuchTable"));
        }

        [Fact]
        public void Returns_Null_For_Tablename_With_More_Than_Two_Parts()
        {
            var metadata = CreateMetadata();
            Assert.Null(metadata.GetRowByTablename("dbo.a.b"));
        }

        [Fact]
        public void GetTableField_Matches_By_Property_Name_IgnoreCase_And_Unquotes_Input()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            var byCase = row.GetTableField("orderid")!;
            Assert.Equal("OrderID", byCase.PropertyName);
            Assert.Equal("OrderID", byCase.ColumnName);

            var byQuoted = row.GetTableField("[OrderID]")!;
            Assert.Equal("OrderID", byQuoted.PropertyName);
        }

        [Fact]
        public void GetTableField_Reads_ColumnName_From_ColumnAttribute()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            var field = row.GetTableField("Meta Description")!;
            Assert.Equal("MetaDescription", field.PropertyName);
            Assert.Equal("Meta Description", field.ColumnName);

            Assert.NotNull(row.GetTableField("meta description"));
        }

        [Fact]
        public void GetTableField_Returns_Null_For_Empty_Or_Null_ColumnName()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Null(row.GetTableField(""));
            Assert.Null(row.GetTableField(null!));
        }

        [Fact]
        public void GetTableField_Returns_Null_When_Multiple_Properties_Match()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("Ambig")!;

            Assert.Null(row.GetTableField("Same"));
            Assert.Null(row.GetTableField("Same"));
        }

        [Fact]
        public void GetTableField_Does_Not_Match_Origin_And_Expression_Properties()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("Props")!;

            Assert.Null(row.GetTableField("LinkedProp"));
            Assert.Null(row.GetTableField("ExprProp"));

            var plain = row.GetTableField("PlainProp")!;
            Assert.Equal("PlainProp", plain.PropertyName);
            Assert.Equal("PlainProp", plain.ColumnName);
        }

        [Fact]
        public void GetProperty_Matches_By_Name_And_Caches_Result()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            var prop1 = row.GetProperty("OrderID")!;
            Assert.Equal("OrderID", prop1.PropertyName);
            Assert.Equal("OrderID", prop1.ColumnName);
            Assert.Same(prop1, row.GetProperty("OrderID"));

            var originProp = row.GetProperty("LinkedProp")!;
        }

        [Fact]
        public void GetProperty_Does_Not_Inherit_ColumnName_For_OriginAndExpression()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("Props")!;

            var linked = row.GetProperty("LinkedProp")!;
            Assert.Equal("LinkedProp", linked.PropertyName);
            Assert.Null(linked.ColumnName);

            var expr = row.GetProperty("ExprProp")!;
            Assert.Equal("ExprProp", expr.PropertyName);
            Assert.Null(expr.ColumnName);
        }

        [Fact]
        public void GetProperty_Returns_Null_For_Empty_Null_Or_Unknown_Property()
        {
            var metadata = CreateMetadata();
            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Null(row.GetProperty(null!));
            Assert.Null(row.GetProperty(""));
            Assert.Null(row.GetProperty("Nope"));
        }
    }
}
