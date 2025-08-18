namespace Serenity.Data;

/// <summary>
/// Interface for rows that have InsertUserId and InsertDate fields
/// </summary>
public interface IInsertLogRow : IInsertDateRow, IInsertUserIdRow
{
}