using System;

namespace Serenity.ComponentModel
{
    public class LabelWidthAttribute : Attribute
    {
        public LabelWidthAttribute(int value)
        {
            Value = value + "px";
        }

        public LabelWidthAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
        public bool UntilNext { get; set; }
    }
}