using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Serenity.Data
{
    public abstract class Field : IFieldWithJoinInfo
    {
        private string autoTextKey;
        internal LocalText caption;
        private string expression;
        private RowFieldsBase fields;
        internal FieldFlags flags;
        private string foreignTable;
        private string foreignField;
        internal int index;
        internal Join join;
        internal string joinAlias;
        private readonly string name;
        internal string origin;
        internal string propertyName;
        internal HashSet<string> referencedAliases;
        private readonly int size;
        private readonly FieldType type;
        internal object defaultValue;
        internal SelectLevel minSelectLevel;
        internal int naturalOrder;
        internal string textualField;

        protected Field(ICollection<Field> fields, FieldType type, string name, LocalText caption, int size, FieldFlags flags)
        {
            this.name = name;
            expression = "T0." + name;
            this.size = size;
            this.flags = flags;
            this.type = type;
            index = -1;
            minSelectLevel = SelectLevel.Default;
            naturalOrder = 0;
            this.caption = caption;
            if (fields != null)
                fields.Add(this);
        }

        public RowFieldsBase Fields
        {
            get { return fields; }
            internal set { fields = value; }
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

        public string Title
        {
            get
            {
                if (ReferenceEquals(null, caption))
                {
                    if (autoTextKey == null)
                        autoTextKey = "Db." + this.Fields.LocalTextPrefix + "." + (propertyName ?? name);

                    return LocalText.TryGet(autoTextKey) ?? (propertyName ?? name);
                }
                else
                    return caption.ToString();
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

        protected Exception JsonUnexpectedToken(JsonReader reader)
        {
            throw new JsonSerializationException("Unexpected token when deserializing row: " + reader.TokenType);
        }

        public void CopyNoAssignment(Row source, Row target)
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
                        if (String.Compare(expression, "T0." + name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            flags ^= FieldFlags.Calculated;
                            flags ^= FieldFlags.Foreign;
                            expression = "T0." + name;
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

                                if (theJoin.ToLowerInvariant() == "t0")
                                    flags = (FieldFlags)(flags ^ FieldFlags.Foreign) | FieldFlags.Calculated;
                                else
                                {
                                    flags = flags | FieldFlags.Foreign;

                                    var split = expression.Split('.');
                                    if (split.Length == 2 &&
                                        split[0] == theJoin &&
                                        IsValidIdentifier(split[1]))
                                    {
                                        joinAlias = theJoin;
                                        origin = split[1];
                                    }
                                    else
                                        flags = flags | FieldFlags.Calculated;
                                }
                            }
                            else
                                flags = flags | FieldFlags.Calculated | FieldFlags.Foreign;
                        }
                    }
                    else
                    {
                        expression = "T0." + name;
                        flags ^= FieldFlags.Calculated;
                        flags ^= FieldFlags.Foreign;
                    }
                }
            }
        }

        public static bool IsValidIdentifier(string s)
        {
            if (s == null || s.Length == 0)
                return false;

            var c = Char.ToUpperInvariant(s[0]);
            if (c != '_' && (c < 'A' || c > 'Z'))
                return false;

            for (var i = 1; i < s.Length; i++)
            {
                c = Char.ToUpperInvariant(s[i]);
                if (c != '_' &&
                    !((c >= '0' && c <= '9') ||
                      (c >= 'A' && c <= 'Z')))
                    return false;
            }

            return true;
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
                    Join theJoin;
                    if (fields.Joins.TryGetValue(joinAlias, out theJoin))
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

        public string TextualField
        {
            get { return textualField; }
            set { textualField = value; }
        }

        public LeftJoin ForeignJoin(Int32? foreignIndex = null)
        {
            if (ForeignTable.IsNullOrEmpty())
                throw new ArgumentNullException("ForeignTable");
            
            string foreignJoin;
            if (foreignIndex == null)
            {
                foreignJoin = Name;
                if (foreignJoin.EndsWith("Id", StringComparison.Ordinal))
                    foreignJoin = foreignJoin.Substring(0, foreignJoin.Length - 2);
                else if (foreignJoin.EndsWith("_ID", StringComparison.OrdinalIgnoreCase))
                    foreignJoin = foreignJoin.Substring(0, foreignJoin.Length - 3);

                foreignJoin = "j" + foreignJoin;
            }
            else
            {
                foreignJoin = foreignIndex.Value.TableAlias();
            }

            var joinKeyField = ForeignField ?? Name;
            var sourceAlias = "T0";
            var sourceKeyField = Name;
            return new LeftJoin(this.Fields.Joins, ForeignTable, foreignJoin,
                new Criteria(foreignJoin, joinKeyField) == new Criteria(sourceAlias, sourceKeyField));
        }

        protected internal virtual void OnRowInitialization()
        {
        }

        protected void CheckUnassignedRead(Row row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (!row.tracking)
                return;

            if (!row.unassignedReadErrors)
                return;

            if (row.IsAssigned(this))
                return;

            throw new InvalidOperationException(String.Format(
                "{0} field on {1} is read before assigned a value!", this.Name, row.GetType().Name));
        }

        public abstract void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer);
        public abstract void ValueFromJson(JsonReader reader, Row row, JsonSerializer serializer);
        public abstract void Copy(Row source, Row target);
        public abstract void GetFromReader(IDataReader reader, int index, Row row);
        public abstract Type ValueType { get; }
        public abstract object ConvertValue(object source, IFormatProvider provider);
        public abstract int IndexCompare(Row row1, Row row2);
        public abstract object AsObject(Row row);
        public abstract void AsObject(Row row, object value);
        public abstract bool IsNull(Row row);

        //public string EditorType { get; set; }
        //public Dictionary<string, object> EditorOptions { get; set; }
        //public string Hint { get; set; }
        //public LocalText Category { get; set; }
        //public int? MaxLength { get; set; }
        //public bool? HideOnInsert { get; set; }
        //public bool? HideOnUpdate { get; set; }
        //public bool? Localizable { get; set; }
        //public int? DisplayOrder { get; set; }
        //public string DisplayFormat { get; set; }

        IDictionary<string, Join> IFieldWithJoinInfo.Joins
        {
            get { return Fields.Joins; }
        }
    }
}