namespace Serenity.Data;

/// <summary>
/// Interface for rows that have UpdateUserId and UpdateDate fields
/// </summary>
public interface IUpdateLogRow : IUpdateDateRow, IUpdateUserIdRow
{
}