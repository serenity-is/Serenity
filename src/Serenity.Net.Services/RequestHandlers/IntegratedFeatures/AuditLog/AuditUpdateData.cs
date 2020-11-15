using Serenity.Data;

public class AuditUpdateData<T> where T : Row
{
    public T Old { get; set; }
    public T New { get; set; }
}