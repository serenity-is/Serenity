using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Data
{
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
        private readonly int size;
        private readonly FieldType type;
        internal object defaultValue;
        internal SelectLevel minSelectLevel;
        internal int naturalOrder;
        internal string textualField;
        private Criteria criteria;
        internal string readPermission;
        internal string insertPermission;
        internal string updatePermission;

        protected Field(ICollection<Field> fields, FieldType type, string name, LocalText caption, int size, FieldFlags flags)
        {
            this.name = name;
            expression = "T0." + SqlSyntax.AutoBracket(name);
            this.size = size;
            this.flags = flags;
            this.type = type;
            index = -1;
            minSelectLevel = SelectLevel.Auto;
            naturalOrder = 0;
            this.caption = caption;
            if (fields != null)
                fields.Add(this);
        }

        public RowFieldsBase Fields
        {
            get { return fields; }
        }

        public int Index
        {
            get { return index; }
            internal set { index = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public FieldType Type
        {
            get { return type; }
        }

        public LocalText Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

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

        public string AutoTextKey
        {
            get
            {
                if (autoTextKey == null)
                    autoTextKey = "Db." + Fields.LocalTextPrefix + "." + (propertyName ?? name);

                return autoTextKey;
            }
        }

        public int Size
        {
            get { return size; }
        }

        public int Scale { get; set; }

        public FieldFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        internal object[] customAttributes;

        public object[] CustomAttributes
        {
            get { return customAttributes; }
            set
            {
                if (customAttributes != value)
                {
                    customAttributes = value;
                    if (fields != null)
                        fields.byAttribute = null;
                }
            }
        }

        protected Exception JsonUnexpectedToken(JsonReader reader)
        {
            throw new JsonSerializationException("Unexpected token when deserializing row: " + reader.TokenType);
        }

        public void CopyNoAssignment(IRow source, IRow target)
        {
            Copy(source, target);
            target.ClearAssignment(this);
        }

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

        public string JoinAlias
        {
            get { return joinAlias; }
        }

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

        public string Origin
        {
            get { return origin; }
        }

        public string ForeignTable
        {
            get { return foreignTable; }
            set { foreignTable = value.TrimToNull(); }
        }

        public string ForeignField
        {
            get { return foreignField; }
            set { foreignField = value.TrimToNull(); }
        }

        public Join ForeignJoinAlias
        {
            get; set;
        }

        public string InsertPermission
        {
            get { return insertPermission; }
            set { insertPermission = value; }
        }

        public SelectLevel MinSelectLevel
        {
            get { return minSelectLevel; }
            set { minSelectLevel = value; }
        }

        public int NaturalOrder
        {
            get { return naturalOrder; }
            set { naturalOrder = value; }
        }

        public string ReadPermission
        {
            get { return readPermission; }
            set { readPermission = value; }
        }

        public string TextualField
        {
            get { return textualField; }
            set { textualField = value; }
        }

        public string UpdatePermission
        {
            get { return updatePermission; }
            set { updatePermission = value; }
        }

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

            var join = new LeftJoin(Fields.Joins, ForeignTable, foreignJoin,
                new Criteria(foreignJoin, joinKeyField) == new Criteria(sourceAlias, sourceKeyField));

            ForeignJoinAlias = join;
            return join;
        }

        protected internal virtual void OnRowInitialization()
        {
        }

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

        public abstract void ValueToJson(JsonWriter writer, IRow row, JsonSerializer serializer);
        public abstract void ValueFromJson(JsonReader reader, IRow row, JsonSerializer serializer);
        public abstract void Copy(IRow source, IRow target);
        public abstract void GetFromReader(IDataReader reader, int index, IRow row);
        public abstract Type ValueType { get; }
        public abstract object ConvertValue(object source, IFormatProvider provider);
        public abstract int IndexCompare(IRow row1, IRow row2);
        public abstract object AsObject(IRow row);
        public abstract void AsObject(IRow row, object value);
        protected abstract bool GetIsNull(IRow row);

        public virtual object AsSqlValue(IRow row)
        {
            return AsObject(row);
        }

        public bool IsNull(IRow row)
        {
            CheckUnassignedRead(row);
            return GetIsNull(row);
        }

        public Criteria Criteria
        {
            get
            {
                if (criteria is object)
                    return criteria;

                criteria = new Criteria(this);
                return criteria;
            }
        }

        IDictionary<string, Join> IFieldWithJoinInfo.Joins
        {
            get { return Fields.Joins; }
        }

        public string ColumnAlias
        {
            get { return propertyName ?? name; }
        }

        public string GetTitle(ITextLocalizer localizer)
        {
            if (caption is null)
            {
                if (autoTextKey == null)
                    autoTextKey = "Db." + Fields.LocalTextPrefix + "." + (propertyName ?? name);

                return localizer?.TryGet(autoTextKey) ?? propertyName ?? name;
            }

            return caption.ToString(localizer);
        }

        public override string ToString()
        {
            return Expression;
        }
    }
}