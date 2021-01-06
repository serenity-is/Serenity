namespace Serenity.Reporting
{
    public interface ICellDecorator
    {
        object Item { get; set; }
        string Name { get; set; }

        object Value { get; set; }
        string Background { get; set; }
        string Foreground { get; set; }
        string Format { get; set; }

        void Decorate();
    }
}