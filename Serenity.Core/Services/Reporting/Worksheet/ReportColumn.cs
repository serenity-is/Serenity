using Serenity.Data;
using System;
using System.Drawing;

namespace Serenity.Reporting
{
    public class ReportColumn
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public double? Width { get; set; }
        public Type DataType { get; set; }
        public string Format { get; set; }
        public bool WrapText { get; set; }
        public ICellDecorator Decorator { get; set; }
    }

    public interface ICellDecorator
    {
        object Item { get; set; }
        string Name { get; set; }

        object Value { get; set; }
        Color Background { get; set; }
        Color Foreground { get; set; }
        string Format { get; set; }

        void Decorate();
    }

    public abstract class BaseCellDecorator : ICellDecorator
    {
        public object Item { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public string Format { get; set; }

        public abstract void Decorate();
    }

    public class CellDecoratorAttribute : Attribute
    {
        public CellDecoratorAttribute(Type decorator)
        {
            this.DecoratorType = decorator;
        }

        public Type DecoratorType { get; set; }
    }

}