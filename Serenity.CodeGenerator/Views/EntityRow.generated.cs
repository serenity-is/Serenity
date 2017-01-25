namespace Serenity.CodeGenerator.Views
{
    using System;
    using System.Collections.Generic;

    public class EntityRow : RazorGenerator.Templating.RazorTemplateBase
    {
        private EntityModel model;

        public dynamic Model
        {
            get { return model; }
            set { model = value; }
        }

        public override void Execute()
        {
            var dotModule = model.Module == null ? "" : ("." + model.Module);
            var moduleDot = model.Module == null ? "" : (model.Module + ".");
            var schemaDot = model.Schema == null ? "" : ("[" + model.Schema + "].");

            Func<string, string, string> jf = (x, y) =>
            {
                if (x.ToLowerInvariant() == y.ToLowerInvariant())
                    return y;
                else
                    return x + y;
            };

            WriteLiteral(Environment.NewLine);
            WriteLiteral("namespace ");
            Write(model.RootNamespace);
            Write(dotModule);
            WriteLiteral(".Entities");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("{");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity.ComponentModel;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity.Data;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using Serenity.Data.Mapping;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using System;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using System.ComponentModel;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    using System.IO;");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    [ConnectionKey(\"");
            WriteLiteral(model.ConnectionKey);
            WriteLiteral("\"), TableName(\"");
            WriteLiteral(string.IsNullOrEmpty(schemaDot) ? model.Tablename : schemaDot + "[" + model.Tablename + "]");
            WriteLiteral("\"), DisplayName(\"");
            WriteLiteral(model.Title);
            WriteLiteral("\"), InstanceName(\"");
            WriteLiteral(model.Title);
            WriteLiteral("\"), TwoLevelCached]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    [ReadPermission(\"");
            WriteLiteral(model.Permission);
            WriteLiteral("\")]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    [ModifyPermission(\"");
            WriteLiteral(model.Permission);
            WriteLiteral("\")]");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    public sealed class ");
            WriteLiteral(model.RowClassName);
            WriteLiteral(" : ");
            WriteLiteral(model.RowBaseClass);
            WriteLiteral(", IIdRow");
            WriteLiteral(model.IsLookup ? ", IDbLookupRow" : "");
            WriteLiteral(model.NameField == null ? "" : ", INameRow");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    {");

            foreach (EntityField x in model.Fields)
            {
                var attrs = new List<string>();
                attrs.Add("DisplayName(\"" + x.Title + "\")");

                if (x.Ident != x.Name)
                    attrs.Add("Column(\"" + x.Name + "\")");

                if ((x.Size ?? 0) != 0)
                    attrs.Add("Size(" + x.Size + ")");

                if (x.Scale != 0)
                    attrs.Add("Scale(" + x.Scale + ")");

                if (!String.IsNullOrEmpty(x.Flags))
                    attrs.Add(x.Flags);

                if (!String.IsNullOrEmpty(x.PKTable))
                {
                    attrs.Add("ForeignKey(\"" + (string.IsNullOrEmpty(x.PKSchema) ? x.PKTable : ("[" + x.PKSchema + "].[" + x.PKTable + "]")) + "\", \"" + x.PKColumn + "\")");
                    attrs.Add("LeftJoin(\"j" + x.ForeignJoinAlias + "\")");
                }

                if (model.NameField == x.Ident)
                    attrs.Add("QuickSearch");

                if (x.TextualField != null)
                    attrs.Add("TextualField(\"" + x.TextualField + "\")");

                var attrString = String.Join(", ", attrs.ToArray());

                WriteLiteral(Environment.NewLine);

                if (!String.IsNullOrEmpty(attrString))
                {
                    WriteLiteral("        [");
                    Write(attrString);
                    WriteLiteral("]" + Environment.NewLine);
                }

                WriteLiteral("        public ");
                WriteLiteral(x.DataType);
                WriteLiteral(x.IsValueType ? "?" : "");
                WriteLiteral(" ");
                WriteLiteral(x.Ident);
                WriteLiteral(Environment.NewLine);
                WriteLiteral("        {");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("            get { return Fields.");
                WriteLiteral(x.Ident);
                WriteLiteral("[this]; }");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("            set { Fields.");
                WriteLiteral(x.Ident);
                WriteLiteral("[this] = value; }");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("        }");
                WriteLiteral(Environment.NewLine);
            }

            foreach (EntityJoin x in model.Joins)
            {
                foreach (EntityField y in x.Fields)
                {
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("        [DisplayName(\"");
                    WriteLiteral(y.Title);
                    WriteLiteral("\"), Expression(\"");
                    WriteLiteral("j");
                    WriteLiteral(x.Name);
                    WriteLiteral(".[");
                    WriteLiteral(y.Name);
                    WriteLiteral("]");
                    WriteLiteral("\")]");
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("        public ");
                    WriteLiteral(y.DataType);
                    WriteLiteral(y.IsValueType ? "?" : "");
                    WriteLiteral(" ");
                    WriteLiteral(jf(x.Name, y.Ident));
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("        {");
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("            get { return Fields.");
                    WriteLiteral(jf(x.Name, y.Ident));
                    WriteLiteral("[this]; }");
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("            set { Fields.");
                    WriteLiteral(jf(x.Name, y.Ident));
                    WriteLiteral("[this] = value; }");
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("        }");
                    WriteLiteral(Environment.NewLine);
                }
            }

            WriteLiteral(Environment.NewLine);
            WriteLiteral("        IIdField IIdRow.IdField");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            get { return Fields.");
            WriteLiteral(model.Identity);
            WriteLiteral("; }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        }");
            WriteLiteral(Environment.NewLine);

            if (model.NameField != null)
            {
                WriteLiteral(Environment.NewLine);
                WriteLiteral("        StringField INameRow.NameField");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("        {");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("            get { return Fields.");
                WriteLiteral(model.NameField);
                WriteLiteral("; }");
                WriteLiteral(Environment.NewLine);
                WriteLiteral("        }");
                WriteLiteral(Environment.NewLine);
            }

            WriteLiteral(Environment.NewLine);
            WriteLiteral("        public static readonly RowFields Fields = new RowFields().Init();");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        public ");
            WriteLiteral(model.RowClassName);
            WriteLiteral("()");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            : base(Fields)");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        public class RowFields : ");

            WriteLiteral(model.FieldsBaseClass);
            WriteLiteral(Environment.NewLine + "        {");

            foreach (EntityField x in model.Fields)
            {
                WriteLiteral(Environment.NewLine + "            public ");
                WriteLiteral(x.FieldType);
                WriteLiteral("Field ");
                WriteLiteral(x.Ident);
                WriteLiteral(";");
            }

            foreach (EntityJoin x in model.Joins)
            {
                WriteLiteral(Environment.NewLine);

                foreach (EntityField y in x.Fields)
                {
                    WriteLiteral(Environment.NewLine);
                    WriteLiteral("            public ");
                    WriteLiteral(y.FieldType);
                    WriteLiteral("Field ");
                    WriteLiteral(jf(x.Name, y.Ident));
                    WriteLiteral(";");
                }
            }

            WriteLiteral(Environment.NewLine);
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            public RowFields()");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("                : base(");
            WriteLiteral(string.IsNullOrEmpty(model.FieldPrefix) ? "" : ("fieldPrefix: \"" + model.FieldPrefix + "\""));
            WriteLiteral(")");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            {");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("                LocalTextPrefix = \"");
            WriteLiteral(moduleDot);
            WriteLiteral(model.ClassName);
            WriteLiteral("\";");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("            }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("        }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("    }");
            WriteLiteral(Environment.NewLine);
            WriteLiteral("}" + Environment.NewLine);
        }
    }
}