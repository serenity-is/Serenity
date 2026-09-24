using Microsoft.Extensions.Options;

namespace Serenity.Data;

/// <summary>
/// Configures the user row type and identifier storage settings, including the user table and ID column names.
/// </summary>
/// <remarks>
/// Configure the identifier type before running the initial database migrations. Changing it after the database is
/// created requires a custom migration and corresponding application changes.
/// </remarks>
public class UserEntityOptions : IOptions<UserEntityOptions>
{
    /// <summary>
    /// Gets or sets the user row type. It must implement <see cref="IRow"/> and have an [IdProperty] property.
    /// That property's type determines the user ID field type, while its [Column] and [Size] attributes provide the
    /// default ID column name and size. Its [TableName] attribute provides the default user table name. Set
    /// <see cref="TableName"/> or <see cref="IdColumnName"/> to override those names. Other user column names are
    /// not configured by this option.
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

                IdColumnName = idProperty.GetCustomAttribute<ColumnAttribute>()?.Name ?? idProperty.Name;
                IdColumnSize = idProperty.GetCustomAttribute<SizeAttribute>()?.Value;

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
    /// Gets or sets the user table name. When not explicitly set, the name comes from the user row's
    /// <see cref="TableNameAttribute"/>; if no row type or attribute is available, it defaults to "Users".
    /// </summary>
    public string TableName
    {
        get => field ?? RowType?.GetCustomAttribute<TableNameAttribute>()?.Name ?? "Users";
        set;
    }

    /// <summary>
    /// Gets or sets the name of the user ID column. Assigning <see cref="RowType"/> initializes this from the
    /// [IdProperty] property's <see cref="ColumnAttribute"/>, or its property name when no column attribute exists.
    /// If no name is available, it defaults to "UserId".
    /// </summary>
    public string? IdColumnName 
    {
        get => field ?? "UserId";
        set;
    }

    /// <summary>
    /// Gets or sets the size of the ID column in the user row. Only meaningful for string columns.
    /// </summary>
    public int? IdColumnSize { get; set; }

    /// <summary>
    /// Gets or sets the type of the ID column in the user row. This is used to determine the data type of the unique identifier for users.
    /// </summary>
    public Type? IdFieldType { get; set; } = typeof(Int32Field);

    /// <inheritdoc />
    public UserEntityOptions Value => this;

    /// <summary>
    /// Assigns the values from another <see cref="UserEntityOptions"/> instance to this instance. This method is used to copy the configuration settings from one instance to another.
    /// </summary>
    /// <param name="from"></param>
    public void AssignFrom(IOptions<UserEntityOptions>? from)
    {
        if (from?.Value is not { } other)
            return;
        RowType = other.RowType;
        IdColumnName = other.IdColumnName;
        IdColumnSize = other.IdColumnSize;
        IdFieldType = other.IdFieldType;
        TableName = other.TableName;
    }
}