namespace Serenity.Data.Mapping;

/// <summary>
/// Declares that the the join key (ForeignKeyAttribute) for this property should be 
/// based on  <see cref="UserEntityOptions"/>, e.g. use its TableName and IdColumnName properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class UserIdJoinKeyAttribute() : ForeignKeyAttribute("Users", "UserId")
{
}