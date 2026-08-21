using FluentMigrator;

namespace Serenity.Extensions;

/// <summary>
/// Marks a migration to run only on the specified database type.
/// </summary>
public class TargetDBAttribute(string db) : TagsAttribute(db + "DB")
{
}