
namespace Serenity.Data
{
    public delegate void SchemaChangeEventObserver(string schema, string table);

    public class SchemaChangeSource
    {
        public static SchemaChangeEventObserver Observers;

        public static void Notify(string schema, string table)
        {
            var observers = Observers;
            if (observers != null)
                observers(schema, table);
        }
    }
}