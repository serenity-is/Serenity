using System;

namespace Serenity.Data
{
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public class ExpressionAttribute : Attribute
    {
        public ExpressionAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }

    public class AddFlagsAttribute : Attribute
    {
        public AddFlagsAttribute(FieldFlags value)
        {
            this.Value = value;
        }

        public FieldFlags Value { get; private set; }
    }

    public class NotNullAttribute : Attribute
    {
    }

    public class MinSelectLevelAttribute : Attribute
    {
        public MinSelectLevelAttribute(SelectLevel value)
        {
            this.Value = value;
        }

        public SelectLevel Value { get; private set; }
    }

    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string table, string field)
        {
            this.Field = field;
            this.Table = table;
        }

        public String Field { get; private set; }
        public String Table { get; private set; }
    }

    public class JoinAliasAttribute : Attribute
    {
        public JoinAliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        public String Alias { get; private set; }
    }

    public class SizeAttribute : Attribute
    {
        public SizeAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }

    public class ScaleAttribute : Attribute
    {
        public ScaleAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }

    public class PrimaryKeyAttribute : Attribute
    {
    }

    public class AutoIncrementAttribute : Attribute
    {
    }

    public class IdentityAttribute : Attribute
    {
    }
}