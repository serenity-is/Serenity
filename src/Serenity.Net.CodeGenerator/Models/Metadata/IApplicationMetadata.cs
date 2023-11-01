namespace Serenity.CodeGenerator;

public interface IApplicationMetadata
{
    IRowMetadata GetRowByTablename(string tablename);
}