using Serenity.CodeGenerator;
using System.Collections.Generic;
using Xunit;

namespace Serenity.CodeGeneration.Test
{
    public partial class ScribanTemplateTests
    {
        private EntityModel MyEntityModel()
        {
            return new EntityModel
            {
                RootNamespace = "MyApplication",
                ConnectionKey = "MyConnection",
                Module = "MyModule",
                RowClassName = "MyRow",
                RowBaseClass = "MyBaseRow",
                FieldsBaseClass = "MyBaseRowFields",
                Identity = "MyId",
                ClassName = "MyTable",
                NameField = "MyName",
                Permission = "MyPermission",
                Title = "My Table",
                Tablename = "my_table",
                AspNetCore = false,
                Fields = new List<EntityField>
                {
                    new EntityField
                    {
                        Name = "my_id",
                        Ident = "MyId",
                        FieldType = "Int32",
                        IsValueType = true,
                        DataType = "Int32",
                        Flags = "Identity",
                        Attributes = "DisplayName(\"My ID\"), Column(\"my_id\"), Identity"
                    },
                    new EntityField
                    {
                        Name = "my_name",
                        Ident = "MyName",
                        FieldType = "String",
                        IsValueType = false,
                        DataType = "String",
                        Size = 100,
                        Flags = "Required",
                        Attributes = "DisplayName(\"My Name\"), Column(\"my_name\")"
                    },
                    new EntityField
                    {
                        Name = "my_foreign_id",
                        Ident = "MyForeignId",
                        FieldType = "Int32",
                        IsValueType = true,
                        DataType = "Int32",
                        ForeignJoinAlias = "jMyForeign",
                        Attributes = "DisplayName(\"My Foreign\"), Column(\"my_foreign_id\"), ForeignKey(\"my_foreign\", \"foreign_id\"), LeftJoin(\"jMyForeign\")",
                    },
                    new EntityField
                    {
                        Name = "my_another_id",
                        Ident = "MyAnotherId",
                        FieldType = "Int32",
                        IsValueType = true,
                        DataType = "Int32",
                        ForeignJoinAlias = "jMyAnother",
                        Attributes = "DisplayName(\"My Another\"), Column(\"my_another_id\"), ForeignKey(\"my_another\", \"another_id\"), LeftJoin(\"jMyAnother\")",
                    },
                    new EntityField
                    {
                        Name = "my_notes",
                        Ident = "MyNotes",
                        FieldType = "String",
                        IsValueType = false,
                        DataType = "String",
                        Size = 0,
                        Flags = "Required",
                        Attributes = "DisplayName(\"My Notes\"), Column(\"my_notes\")"
                    }
                },
                Joins = new List<EntityJoin>
                {
                    new EntityJoin
                    {
                        Name = "jMyForeign",
                        Fields = new List<EntityField>
                        {
                            new EntityField
                            {
                                Name = "foreign_name",
                                Ident = "MyForeignName",
                                FieldType = "String",
                                IsValueType = false,
                                DataType = "String",
                                Attributes = "DisplayName(\"My Foreign Name\"), Expression(\"jMyForeign.foreign_name\")"
                            },
                            new EntityField
                            {
                                Name = "foreign_value1",
                                Ident = "MyForeignValue1",
                                FieldType = "Decimal",
                                IsValueType = true,
                                DataType = "Decimal",
                                Size = 13,
                                Scale = 4,
                                Attributes = "DisplayName(\"My Foreign Value 1\"), Expression(\"jMyForeign.foreign_value1\")"
                            }
                        }
                    },
                    new EntityJoin
                    {
                        Name = "jMyAnother",
                        Fields = new List<EntityField>
                        {
                            new EntityField
                            {
                                Name = "another_name",
                                Ident = "MyAnotherName",
                                FieldType = "String",
                                IsValueType = false,
                                DataType = "String",
                                Attributes = "DisplayName(\"My Another Name\"), Expression(\"jMyAnother.another_name\")"
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void Row_NoModule_GeneratesProperNamespace()
        {
            var model = MyEntityModel();
            model.Module = null;
            var actual = Templates.Render("Row", model);
            actual = actual.Replace("\r", "");

            var searchText = "namespace MyApplication.Entities\n";

            Assert.Contains(searchText, actual);
            Assert.StrictEqual(actual.IndexOf(searchText), 
                actual.LastIndexOf(searchText));
        }

        [Fact]
        public void Row_SomeModule_GeneratesProperNamespace()
        {
            var model = MyEntityModel();
            model.Module = "SomeModule";
            var actual = Templates.Render("Row", model);
            actual = actual.Replace("\r", "");

            var searchText = "namespace MyApplication.SomeModule.Entities\n";

            Assert.Contains(searchText, actual);
            Assert.StrictEqual(actual.IndexOf(searchText),
                actual.LastIndexOf(searchText));
        }

        [Fact]
        public void Row_GeneratesExpectedCode()
        {
            var model = MyEntityModel();
            var actual = Templates.Render("Row", model);
            actual = actual.Replace("\r", "");

            var searchText =
                @"using Serenity;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;
using System.ComponentModel;
using System.IO;

namespace MyApplication.MyModule.Entities
{
    [ConnectionKey(""MyConnection""), Module(""MyModule""), TableName(""my_table"")]
    [DisplayName(""My Table""), InstanceName(""My Table"")]
    [ReadPermission(""MyPermission"")]
    [ModifyPermission(""MyPermission"")]
    public sealed class MyRow : MyBaseRow, IIdRow, INameRow
    {
        [DisplayName(""My ID""), Column(""my_id""), Identity]
        public Int32? MyId
        {
            get { return Fields.MyId[this]; }
            set { Fields.MyId[this] = value; }
        }

        [DisplayName(""My Name""), Column(""my_name"")]
        public String MyName
        {
            get { return Fields.MyName[this]; }
            set { Fields.MyName[this] = value; }
        }

        [DisplayName(""My Foreign""), Column(""my_foreign_id""), ForeignKey(""my_foreign"", ""foreign_id""), LeftJoin(""jMyForeign"")]
        public Int32? MyForeignId
        {
            get { return Fields.MyForeignId[this]; }
            set { Fields.MyForeignId[this] = value; }
        }

        [DisplayName(""My Another""), Column(""my_another_id""), ForeignKey(""my_another"", ""another_id""), LeftJoin(""jMyAnother"")]
        public Int32? MyAnotherId
        {
            get { return Fields.MyAnotherId[this]; }
            set { Fields.MyAnotherId[this] = value; }
        }

        [DisplayName(""My Notes""), Column(""my_notes"")]
        public String MyNotes
        {
            get { return Fields.MyNotes[this]; }
            set { Fields.MyNotes[this] = value; }
        }

        [DisplayName(""My Foreign Name""), Expression(""jMyForeign.foreign_name"")]
        public String MyForeignName
        {
            get { return Fields.MyForeignName[this]; }
            set { Fields.MyForeignName[this] = value; }
        }

        [DisplayName(""My Foreign Value 1""), Expression(""jMyForeign.foreign_value1"")]
        public Decimal? MyForeignValue1
        {
            get { return Fields.MyForeignValue1[this]; }
            set { Fields.MyForeignValue1[this] = value; }
        }

        [DisplayName(""My Another Name""), Expression(""jMyAnother.another_name"")]
        public String MyAnotherName
        {
            get { return Fields.MyAnotherName[this]; }
            set { Fields.MyAnotherName[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.MyId; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.MyName; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public MyRow()
            : base(Fields)
        {
        }

        public class RowFields : MyBaseRowFields
        {
            public Int32Field MyId;
            public StringField MyName;
            public Int32Field MyForeignId;
            public Int32Field MyAnotherId;
            public StringField MyNotes;

            public StringField MyForeignName;
            public DecimalField MyForeignValue1;

            public StringField MyAnotherName;
        }
    }
}
".Replace("\r", "");

            Assert.StrictEqual(searchText, actual);
        }

    }
}