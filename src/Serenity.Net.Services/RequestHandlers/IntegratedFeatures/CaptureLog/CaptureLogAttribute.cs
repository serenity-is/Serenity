namespace Serenity.Data
{
    public class CaptureLogAttribute : Attribute
    {
        public CaptureLogAttribute(Type logRow)
        {
            LogRow = logRow ?? throw new ArgumentNullException(nameof(logRow));
        }

        public Type LogRow { get; private set; }
        public string MappedIdField { get; set; }
    }
}