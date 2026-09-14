namespace Serenity.CodeGenerator
{
    public partial class ApplicationMetadataTests
    {
        private static EntityModel CreateOrderEntityModel()
        {
            var model = new EntityModel
            {
                ConnectionKey = "TestConnection",
                ClassName = "Ord",
                RowClassName = "OrdRow",
                Schema = "dbo",
                Tablename = "AppMetaOrders",
                Module = "Orders",
                RootNamespace = "ApplicationMetadataTest.Models",
                IdField = "ZOuid",
                NameField = "ZOname"
            };
            model.Fields.Add(new EntityField
            {
                PropertyName = "ZOuid",
                Name = "ZOuid",
                FieldType = "Int32",
                DataType = "int",
                TSType = "number"
            });
            model.Fields.Add(new EntityField
            {
                PropertyName = "ZOname",
                Name = "ZOname",
                FieldType = "String",
                DataType = "string",
                TSType = "string"
            });
            return model;
        }

        [Fact]
        public void Reads_Metadata_From_EntityModel()
        {
            var metadata = CreateMetadata();
            metadata.EntityModels.Add(CreateOrderEntityModel());

            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Equal("ApplicationMetadataTest.Models.Orders", row.Namespace);
            Assert.Equal("OrdRow", row.ClassName);
            Assert.Equal("Orders", row.Module);
            Assert.False(row.HasLookupScriptAttribute);
            Assert.Equal("ZOuid", row.IdProperty);
            Assert.Equal("ZOname", row.NameProperty);
            Assert.Equal("Services/Orders/Ord", row.ListServiceRoute);
        }

        [Fact]
        public void EntityModels_Take_Procedence_Over_RowTypes()
        {
            var metadata = CreateMetadata();
            metadata.EntityModels.Add(CreateOrderEntityModel());

            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            Assert.Equal("ZOuid", row.IdProperty);
            Assert.NotEqual("OrderID", row.IdProperty);
        }

        [Fact]
        public void EntityModel_Matches_Quoted_SchemaAndTable()
        {
            var metadata = CreateMetadata();
            metadata.EntityModels.Add(CreateOrderEntityModel());

            var row1 = metadata.GetRowByTablename("[dbo].[AppMetaOrders]")!;
            var row2 = metadata.GetRowByTablename("[dbo].[AppMetaOrders]")!;

            Assert.Equal("OrdRow", row1.ClassName);
            Assert.Same(row1, row2);
        }

        [Fact]
        public void EntityModel_Without_Schema_Matches_Plain_Tablename()
        {
            var metadata = CreateMetadata();
            var model = CreateOrderEntityModel();
            model.Schema = null;
            metadata.EntityModels.Add(model);

            Assert.NotNull(metadata.GetRowByTablename("AppMetaOrders"));
        }

        [Fact]
        public void EntityModelRowMetadata_GetProperty_And_GetTableField()
        {
            var metadata = CreateMetadata();
            metadata.EntityModels.Add(CreateOrderEntityModel());

            var row = metadata.GetRowByTablename("dbo.AppMetaOrders")!;

            var idProp = row.GetProperty("ZOuid")!;
            Assert.Equal("ZOuid", idProp.PropertyName);
            Assert.Equal("ZOuid", idProp.ColumnName);
            Assert.Same(idProp, row.GetProperty("ZOuid"));

            var nameField = row.GetTableField("ZONAME")!;
            Assert.Equal("ZOname", nameField.ColumnName);
            Assert.Equal("ZOname", nameField.PropertyName);
            Assert.Same(nameField, row.GetTableField("ZONAME"));

            Assert.Null(row.GetProperty("Nope"));
            Assert.Null(row.GetTableField("Nope"));
            Assert.Null(row.GetProperty(null!));
            Assert.Null(row.GetTableField(""));
        }
    }
}
