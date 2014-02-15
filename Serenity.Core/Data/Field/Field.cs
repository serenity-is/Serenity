using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Reflection;

namespace Serenity.Data
{
    public abstract class Field : IJoinField
    {
        private FieldType _type;
        internal int _index;
        internal RowFieldsBase _fields;
        internal string _name;
        internal int _size;
        internal FieldFlags _flags;
        internal string _expression;
        internal HashSet<string> _referencedJoins;
        internal string _joinAlias;
        internal Join _join;
        internal string _origin;
        internal string _foreignTable;
        internal string _foreignField;
        internal LocalText _caption;
        internal string _autoTextKey;
        internal string _propertyName;
        internal Type _rowType;
        internal object _defaultValue;
        internal SelectLevel _minSelectLevel;
        internal int _naturalOrder;
        internal string _textualField;
        
        protected Field(ICollection<Field> fields, FieldType type, string name, LocalText caption, int size, FieldFlags flags)
        {
            _name = name;
            _expression = "T0." + name;
            _size = size;
            _flags = flags;
            _type = type;
            _index = -1;
            _minSelectLevel = SelectLevel.Default;
            _naturalOrder = 0;
            _caption = caption;
            if (fields != null)
                fields.Add(this);
        }

        public RowFieldsBase Fields
        {
            get { return _fields; }
        }

        public int Index
        {
            get { return _index; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldType Type
        {
            get { return _type; }
        }

        public LocalText Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public HashSet<string> ReferencedJoins
        {
            get
            {
                return _referencedJoins;
            }
            set
            {
                _referencedJoins = value;
            }
        }

        public string Title
        {
            get
            {
                if (null == _caption)
                {
                    if (_autoTextKey == null)
                        _autoTextKey = "Db." + this.Fields.LocalTextPrefix + "." + (_propertyName ?? _name);

                    return LocalText.TryGet(_autoTextKey) ?? (_propertyName ?? _name);
                }
                else
                    return _caption.ToString();
            }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public int Scale { get; set; }

        public FieldFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
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
            get { return _expression; }
            set 
            {
                value = value.TrimToNull();
                if (_expression != value)
                {
                    _expression = value;
                    _referencedJoins = null;
                    _joinAlias = null;
                    _origin = null;
                    _join = null;

                    if (value != null)
                    {
                        if (String.Compare(_expression, "T0." + _name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _flags ^= FieldFlags.Calculated;
                            _flags ^= FieldFlags.Foreign;
                            _expression = "T0." + _name;
                            return;
                        }

                        var aliases = JoinAliasLocator.Locate(value);
                        if (aliases != null && aliases.Count > 0)
                        {
                            _referencedJoins = aliases;

                            if (aliases.Count == 1)
                            {
                                var enumerator = aliases.GetEnumerator();
                                enumerator.MoveNext();
                                var join = enumerator.Current;

                                if (join.ToLowerInvariant() == "t0")
                                    _flags = (FieldFlags)(_flags ^ FieldFlags.Foreign) | FieldFlags.Calculated;
                                else
                                {
                                    _flags = _flags | FieldFlags.Foreign;

                                    var split = _expression.Split('.');
                                    if (split.Length == 2 &&
                                        split[0] == join &&
                                        IsValidIdentifier(split[1]))
                                    {
                                        _joinAlias = join;
                                        _origin = split[1];
                                    }
                                    else
                                        _flags = _flags | FieldFlags.Calculated;
                                }
                            }
                            else
                                _flags = _flags | FieldFlags.Calculated | FieldFlags.Foreign;
                        }
                    }
                    else
                    {
                        _expression = "T0." + _name;
                        _flags ^= FieldFlags.Calculated;
                        _flags ^= FieldFlags.Foreign;
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
            get { return _joinAlias; }
        }

        public Join Join
        {
            get
            {
                if (_join == null &&
                    _joinAlias != null)
                {
                    Join join;
                    if (_fields.Joins.TryGetValue(_joinAlias, out join))
                        _join = join;
                }

                return _join;
            }
        }

        public string Origin
        {
            get { return _origin; }
        }

        public string ForeignTable
        {
            get { return _foreignTable; }
            set { _foreignTable = value.TrimToNull(); }
        }

        public string ForeignField
        {
            get { return _foreignField; }
            set { _foreignField = value.TrimToNull(); }
        }

        public SelectLevel MinSelectLevel
        {
            get { return _minSelectLevel; }
            set { _minSelectLevel = value; }
        }

        public int NaturalOrder
        {
            get { return _naturalOrder; }
            set { _naturalOrder = value; }
        }

        public string TextualField
        {
            get { return _textualField; }
            set { _textualField = value; }
        }

        public LeftJoin ForeignJoin(Int32? foreignIndex = null)
        {
            if (ForeignTable.IsEmptyOrNull())
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

        IDictionary<string, Join> IJoinField.Joins
        {
            get { return Fields.Joins; }
        }
    }
}