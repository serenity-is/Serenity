using System;

namespace Serenity.Reporting
{
    public class CellDecoratorAttribute : Attribute
    {
        public CellDecoratorAttribute(Type decorator)
        {
            DecoratorType = decorator;
        }

        public Type DecoratorType { get; set; }
    }
}
