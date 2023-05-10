namespace Serenity.Data;

/// <summary>
/// Base Field class
/// </summary>
/// <seealso cref="IFieldWithJoinInfo" />
public abstract partial class Field : IFieldWithJoinInfo
{
    private string autoTextKey;
    internal LocalText caption;
    internal string expression;
    internal RowFieldsBase fields;
    internal FieldFlags flags;
    private string foreignTable;
    private string foreignField;
    internal int index;
    internal Join join;
    internal string joinAlias;
    internal string name;
    internal string origin;
    internal string propertyName;
    internal HashSet<string> referencedAliases;
    private readonly FieldType type;
    internal object defaultValue;
    internal SelectLevel minSelectLevel;
    internal int naturalOrder;
    internal string textualField;
    private Criteria criteria;
    internal string readPermission;
    internal string insertPermission;
    internal string updatePermission;

    /// <summary>
    /// Initializes a new instance of the <see cref="Field"/> class.
    /// </summary>
    /// <param name="fields">The fields.</param>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="caption">The caption.</param>
    /// <param name="size">The size.</param>
    /// <param name="flags">The flags.</param>
    protected Field(ICollection<Field> fields, FieldType type, string name, LocalText caption, int size, FieldFlags flags)
    {
        this.name = name;
        expression = "T0." + SqlSyntax.AutoBracket(name);
        Size = size;
        this.flags = flags;
        this.type = type;
        index = -1;
        minSelectLevel = SelectLevel.Auto;
        naturalOrder = 0;
        this.caption = caption;
        customAttributes = Array.Empty<object>();
        fields?.Add(this);
    }

    /// <summary>
    /// Gets the fields.
    /// </summary>
    /// <value>
    /// The fields.
    /// </value>
    public RowFieldsBase Fields => fields;

    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index
    {
        get { return index; }
        internal set { index = value; }
    }

    /// <summary>
    /// Column name
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public FieldType Type => type;

    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    /// <value>
    /// The caption.
    /// </value>
    public LocalText Caption
    {
        get { return caption; }
        set { caption = value; }
    }

    /// <summary>
    /// Gets or sets the default value.
    /// </summary>
    /// <value>
    /// The default value.
    /// </value>
    public object DefaultValue
    {
        get { return defaultValue; }
        set { defaultValue = value; }
    }

    /// <summary>
    /// Gets or sets the referenced aliases.
    /// </summary>
    /// <value>
    /// The referenced aliases.
    /// </value>
    public HashSet<string> ReferencedAliases
    {
        get
        {
            return referencedAliases;
        }
        set
        {
            referencedAliases = value;
        }
    }

    /// <summary>
    /// Gets the automatic text key.
    /// </summary>
    /// <value>
    /// The automatic text key.
    /// </value>
    public string AutoTextKey => autoTextKey ??= "Db." + fields.LocalTextPrefix + "." + (propertyName ?? name);

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the scale.
    /// </summary>
    /// <value>
    /// The scale.
    /// </value>
    public int Scale { get; set; }

    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    /// <value>
    /// The flags.
    /// </value>
    public FieldFlags Flags
    {
        get { return flags; }
        set { flags = value; }
    }

    /// <summary>
    /// Gets or sets the name of the property.
    /// </summary>
    /// <value>
    /// The name of the property.
    /// </value>
    public string PropertyName
    {
        get { return propertyName; }
        set { propertyName = value; }
    }

    internal object[] customAttributes;

    /// <summary>
    /// Gets or sets the custom attributes.
    /// </summary>
    /// <value>
    /// The custom attributes.
    /// </value>
    public object[] CustomAttributes
    {
        get { return customAttributes; }
        set
        {
            if (customAttributes != value)
            {
                customAttributes = value ?? Array.Empty<object>();
                if (fields != null)
                    fields.byAttribute = null;
            }
        }
    }

    /// <summary>
    /// Jsons the unexpected token.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    /// <exception cref="JsonSerializationException">Unexpected token when deserializing row: " + reader.TokenType</exception>
    protected Exception JsonUnexpectedToken(JsonReader reader)
    {
        throw new JsonSerializationException("Unexpected token when deserializing row: " + reader.TokenType);
    }

    /// <summary>
    /// Copies the no assignment.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public void CopyNoAssignment(IRow source, IRow target)
    {
        Copy(source, target);
        target.ClearAssignment(this);
    }

    /// <summary>
    /// The expression (can be equal to name if no expression)
    /// </summary>
    public string Expression
    {
        get { return expression; }
        set
        {
            value = value.TrimToNull();
            if (expression != value)
            {
                expression = value;
                referencedAliases = null;
                joinAlias = null;
                origin = null;
                join = null;

                if (value != null)
                {
                    if (expression != null &&
                        expression.StartsWith("T0.", StringComparison.OrdinalIgnoreCase) &&
                        SqlSyntax.IsValidQuotedIdentifier(expression[3..]))
                    {
                        if (flags.HasFlag(FieldFlags.Calculated))
                            flags -= FieldFlags.Calculated;

                        if (flags.HasFlag(FieldFlags.Foreign))
                            flags -= FieldFlags.Foreign;

                        return;
                    }

                    var aliases = JoinAliasLocator.Locate(value);
                    if (aliases != null && aliases.Count > 0)
                    {
                        referencedAliases = aliases;

                        if (aliases.Count == 1)
                        {
                            var enumerator = aliases.GetEnumerator();
                            enumerator.MoveNext();
                            var theJoin = enumerator.Current;

                            if (theJoin == "t0" || theJoin == "T0")
                                flags = flags ^ FieldFlags.Foreign | FieldFlags.Calculated;
                            else
                            {
                                flags |= FieldFlags.Foreign;

                                var split = expression.Split('.');
                                if (split.Length == 2 &&
                                    split[0] == theJoin &&
                                    SqlSyntax.IsValidQuotedIdentifier(split[1]))
                                {
                                    joinAlias = theJoin;
                                    origin = split[1];
                                }
                                else
                                    flags |= FieldFlags.Calculated;
                            }
                        }
                        else
                            flags = flags | FieldFlags.Calculated | FieldFlags.Foreign;
                    }
                    else if (!SqlSyntax.IsValidQuotedIdentifier(value))
                        flags |= FieldFlags.Calculated;
                }
                else
                {
                    expression = "T0." + name;

                    if (flags.HasFlag(FieldFlags.Calculated))
                        flags -= FieldFlags.Calculated;

                    if (flags.HasFlag(FieldFlags.Foreign))
                        flags -= FieldFlags.Foreign;
                }
            }
        }
    }

    /// <summary>
    /// Gets the join alias.
    /// </summary>
    /// <value>
    /// The join alias.
    /// </value>
    public string JoinAlias => joinAlias;

    /// <summary>
    /// Gets the join.
    /// </summary>
    /// <value>
    /// The join.
    /// </value>
    public Join Join
    {
        get
        {
            if (join == null &&
                joinAlias != null)
            {
                if (fields.Joins.TryGetValue(joinAlias, out Join theJoin))
                    join = theJoin;
            }

            return join;
        }
    }

    /// <summary>
    /// Gets the origin.
    /// </summary>
    /// <value>
    /// The origin.
    /// </value>
    public string Origin => origin;

    /// <summary>
    /// Gets or sets the foreign table.
    /// </summary>
    /// <value>
    /// The foreign table.
    /// </value>
    public string ForeignTable
    {
        get { return foreignTable; }
        set { foreignTable = value.TrimToNull(); }
    }

    /// <summary>
    /// Gets or sets the foreign field.
    /// </summary>
    /// <value>
    /// The foreign field.
    /// </value>
    public string ForeignField
    {
        get { return foreignField; }
        set { foreignField = value.TrimToNull(); }
    }

    /// <summary>
    /// Gets or sets the foreign join alias.
    /// </summary>
    /// <value>
    /// The foreign join alias.
    /// </value>
    public Join ForeignJoinAlias
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the insert permission.
    /// </summary>
    /// <value>
    /// The insert permission.
    /// </value>
    public string InsertPermission
    {
        get { return insertPermission; }
        set { insertPermission = value; }
    }

    /// <summary>
    /// Gets or sets the minimum select level.
    /// </summary>
    /// <value>
    /// The minimum select level.
    /// </value>
    public SelectLevel MinSelectLevel
    {
        get { return minSelectLevel; }
        set { minSelectLevel = value; }
    }

    /// <summary>
    /// Gets or sets the natural order.
    /// </summary>
    /// <value>
    /// The natural order.
    /// </value>
    public int NaturalOrder
    {
        get { return naturalOrder; }
        set { naturalOrder = value; }
    }

    /// <summary>
    /// Gets or sets the read permission.
    /// </summary>
    /// <value>
    /// The read permission.
    /// </value>
    public string ReadPermission
    {
        get { return readPermission; }
        set { readPermission = value; }
    }

    /// <summary>
    /// Gets or sets the textual field.
    /// </summary>
    /// <value>
    /// The textual field.
    /// </value>
    public string TextualField
    {
        get { return textualField; }
        set { textualField = value; }
    }

    /// <summary>
    /// Gets or sets the update permission.
    /// </summary>
    /// <value>
    /// The update permission.
    /// </value>
    public string UpdatePermission
    {
        get { return updatePermission; }
        set { updatePermission = value; }
    }

    /// <summary>
    /// Creates a left join from the foreign join index.
    /// </summary>
    /// <param name="foreignIndex">Index of the foreign.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">ForeignTable</exception>
    [Obsolete("This method was used by the old ORM")]
    public LeftJoin ForeignJoin(int? foreignIndex = null)
    {
        if (ForeignTable.IsNullOrEmpty())
            throw new ArgumentNullException("ForeignTable");

        string foreignJoin;
        if (foreignIndex == null)
        {
            foreignJoin = Name;
            if (foreignJoin.EndsWith("Id", StringComparison.Ordinal))
                foreignJoin = foreignJoin[0..^2];
            else if (foreignJoin.EndsWith("_ID", StringComparison.OrdinalIgnoreCase))
                foreignJoin = foreignJoin[0..^3];

            foreignJoin = "j" + foreignJoin;
        }
        else
        {
            foreignJoin = foreignIndex.Value.TableAlias();
        }

        var joinKeyField = ForeignField ?? Name;
        var sourceAlias = "T0";
        var sourceKeyField = Name;

        var join = new LeftJoin(fields.Joins, ForeignTable, foreignJoin,
            new Criteria(foreignJoin, joinKeyField) == new Criteria(sourceAlias, sourceKeyField));

        ForeignJoinAlias = join;
        return join;
    }

    /// <summary>
    /// Called when [row initialization].
    /// </summary>
    protected internal virtual void OnRowInitialization()
    {
    }

    /// <summary>
    /// Checks the unassigned read.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <exception cref="ArgumentNullException">row</exception>
    /// <exception cref="InvalidOperationException"></exception>
    protected void CheckUnassignedRead(IRow row)
    {
        if (row == null)
            throw new ArgumentNullException("row");

        if (!row.TrackWithChecks)
            return;

        if (row.IsAssigned(this))
            return;

        if (!GetIsNull(row))
            return;

        throw new InvalidOperationException(string.Format(
            "{0} field on {1} is read before assigned a value! Make sure this field is selected in your SqlQuery. Extensions like connection.List only loads table fields by default, view / expression fields are not loaded unless explicitly selected.",
                Name, row.GetType().Name));
    }

    /// <summary>
    /// Serializes this fields value to JSON
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public abstract void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer);
    /// <summary>
    /// Deserializes this fields value from JSON
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="row">The row.</param>
    /// <param name="serializer">The serializer.</param>
    public abstract void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer);
    /// <summary>
    /// Copies the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    public abstract void Copy(IRow source, IRow target);
    /// <summary>
    /// Gets field value from a data reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="index">The index.</param>
    /// <param name="row">The row.</param>
    public abstract void GetFromReader(IDataReader reader, int index, IRow row);
    /// <summary>
    /// Gets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public abstract Type ValueType { get; }
    /// <summary>
    /// Converts the value.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public abstract object ConvertValue(object source, IFormatProvider provider);
    /// <summary>
    /// Compares the field values for two rows for an ascending index sort
    /// </summary>
    /// <param name="row1">The row1.</param>
    /// <param name="row2">The row2.</param>
    /// <returns></returns>
    public abstract int IndexCompare(IRow row1, IRow row2);
    /// <summary>
    /// Gets the value of this row as an object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public abstract object AsObject(IRow row);
    /// <summary>
    /// Gets the value of this field in specified row as object.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="value">The value.</param>
    public abstract void AsObject(IRow row, object value);
    /// <summary>
    /// Gets if the field value is null.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    protected abstract bool GetIsNull(IRow row);

    /// <summary>
    /// Gets the value of this row as an SQL value.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    public virtual object AsSqlValue(IRow row)
    {
        return AsObject(row);
    }

    /// <summary>
    /// Determines whether the specified row is null.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>
    ///   <c>true</c> if the specified row is null; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNull(IRow row)
    {
        CheckUnassignedRead(row);
        return GetIsNull(row);
    }

    /// <summary>
    /// Gets the criteria.
    /// </summary>
    /// <value>
    /// The criteria.
    /// </value>
    public Criteria Criteria
    {
        get
        {
            if (criteria is not null)
                return criteria;

            criteria = new Criteria(this);
            return criteria;
        }
    }

    /// <summary>
    /// Gets if this field is one with a LookupInclude attribute or ID or Name field
    /// </summary>
    public bool IsLookup { get; internal set; }

    IDictionary<string, Join> IFieldWithJoinInfo.Joins => fields.Joins;

    /// <summary>
    /// Select as column alias. Can be equal to property name or name.
    /// </summary>
    public string ColumnAlias => propertyName ?? name;

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <param name="localizer">The localizer.</param>
    /// <returns></returns>
    public string GetTitle(ITextLocalizer localizer)
    {
        if (caption is null)
        {
            autoTextKey ??= "Db." + fields.LocalTextPrefix + "." + (propertyName ?? name);
            return localizer?.TryGet(autoTextKey) ?? propertyName ?? name;
        }

        return caption.ToString(localizer);
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Expression;
    }
}