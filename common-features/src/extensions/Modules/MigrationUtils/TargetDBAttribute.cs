using FluentMigrator;

namespace Serenity.Extensions;

public class TargetDBAttribute(string db) : TagsAttribute(db + "DB")
{
}