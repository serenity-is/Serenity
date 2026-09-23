using System.Reflection;

namespace Serenity.Data;

/// <summary>
/// Configures user row settings for the application. This class is used to specify the type of the user row and its ID field.
/// </summary>
public class UserRowSettings : IOptions<UserRowSettings>
{
    /// <summary>
    /// Gets or sets the type of the user row. This is used to determine which row type represents users in the system.
    /// </summary>
    public Type? RowType
    {
        get;
        set
        {
            if (value is not null)
            {
                if (!typeof(IRow).IsAssignableFrom(value))
                    throw new ArgumentException("UserRowSettings.RowType must be a type that implements IRow.");

                if (value.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => x.GetCustomAttribute<IdPropertyAttribute>() is not null) is not { } idProperty)
                {
                    throw new ArgumentException("UserRowSettings.RowType must have a property with the [IdProperty] attribute.");
                }

                IdFieldName = idProperty.GetCustomAttribute<ColumnAttribute>()?.Name ?? idProperty.Name;

                var valueType = Nullable.GetUnderlyingType(idProperty.PropertyType) ?? idProperty.PropertyType;

                IdFieldType = valueType switch
                {
                    Type t when t == typeof(int) => typeof(Int32Field),
                    Type t when t == typeof(long) => typeof(Int64Field),
                    Type t when t == typeof(Guid) => typeof(GuidField),
                    Type t when t == typeof(string) => typeof(StringField),
                    _ => throw new ArgumentException("UserRowSettings.RowType must have an Id property of type int, long, Guid or string.")
                };
            }

            field = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of the ID field in the user row. This is used to determine which field represents the unique identifier for users.
    /// </summary>
    public string? IdFieldName { get; set; } = "UserId";

    /// <summary>
    /// Gets or sets the type of the ID field in the user row. This is used to determine the data type of the unique identifier for users.
    /// </summary>
    public Type? IdFieldType { get; set; } = typeof(Int32Field);

    /// <inheritdoc />
    public UserRowSettings Value => this;
}