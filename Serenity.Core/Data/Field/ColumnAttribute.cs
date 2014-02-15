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

    public class ForeignAttribute : Attribute
    {
        public string Table { get; set; }
        public string Alias { get; set; }
        public string Field { get; set; }
    }
}