
namespace Serenity.Reporting
{
    public abstract class BaseCellDecorator : ICellDecorator
    {
        public object Item { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string Format { get; set; }

        public abstract void Decorate();
    }
}
