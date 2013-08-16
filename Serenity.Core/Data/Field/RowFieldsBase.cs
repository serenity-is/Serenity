using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;

namespace Serenity.Data
{
    public class RowFieldsBase : Collection<Field>
    {
        public readonly string TableName;
        internal Dictionary<string, Field> _byName;
        internal Dictionary<string, Field> _byPropertyName;
        internal Dictionary<string, LeftJoin> _leftJoins;
        internal FilterFields _filters;
        internal bool _isLocked;
        internal PropertyChangedEventArgs[] _propertyChangedEventArgs;
        internal PropertyDescriptorCollection _propertyDescriptors;
        internal string _fieldPrefix;
        internal string _localTextPrefix;
        internal string _schema;
        internal string _schemaDotTable;

        public RowFieldsBase(string tableName, string fieldPrefix)
        {
            TableName = tableName;
            FieldPrefix = fieldPrefix;
            _byName = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            _leftJoins = new Dictionary<string, LeftJoin>(StringComparer.OrdinalIgnoreCase);
            _filters = new FilterFields();
        }

        public string FieldPrefix
        {
            get { return _fieldPrefix ?? ""; }
            set { _fieldPrefix = value; }
        }

        public string LocalTextPrefix
        {
            get { return _localTextPrefix ?? TableName; }
            set { _localTextPrefix = value; }
        }

        public string Schema
        {
            get { return _schema; }
        }

        public string SchemaDotTable
        {
            get 
            {
                _schemaDotTable = _schemaDotTable ?? (_schema + "." + TableName);
                return _schemaDotTable;
            }
        }

        public string GenerationKey
        {
            get { return SchemaDotTable; }
        }

        protected override void InsertItem(int index, Field item)
        {
            if (_isLocked)
                throw new InvalidOperationException("field collection can't be modified!");

            if (item == null)
                throw new ArgumentNullException("item");

            if (_byName.ContainsKey(item.Name))
                throw new ArgumentOutOfRangeException("item",
                    String.Format("field list already contains a field with name '{0}'", item.Name));

            if (item._fields != null)
                item._fields.Remove(item);

            base.InsertItem(index, item);

            item._fields = this;
            item._index = index;

            _byName[item.Name] = item;
        }

        protected override void RemoveItem(int index)
        {
            if (_isLocked)
                throw new InvalidOperationException("field collection can't be modified!");

            var item = base[index];
            base.RemoveItem(index);
            item._index = -1;
            item._fields = null;
            _byName.Remove(item.Name);
            for (int i = index; i < Count; i++)
                this[i]._index = i;
        }

        protected override void SetItem(int index, Field item)
        {
            if (_isLocked)
                throw new InvalidOperationException("field collection can't be modified!");

            if (item == null)
                throw new ArgumentNullException("item");

            if (_byName.ContainsKey(item.Name))
                throw new ArgumentOutOfRangeException("item", 
                    String.Format("field list already contains a field with name '{0}'", item.Name));

            var old = base[index];

            base.SetItem(index, item);

            old._index = -1;
            old._fields = null;
            _byName.Remove(old.Name);

            item._index = index;
            item._fields = this;
            _byName[item.Name] = item;
        }

        public Field FindField(string fieldName)
        {
            Field field;
            if (_byName.TryGetValue(fieldName, out field))
                return field;
            else
                return null;
        }

        public Field FindFieldByPropertyName(string propertyName)
        {
            Field field;
            if (_byPropertyName.TryGetValue(propertyName, out field))
                return field;
            else
                return null;
        }

        public IDictionary<string, LeftJoin> LeftJoins
        {
            get { return _leftJoins; }
        }

        public FilterFields Filters
        {
            get { return _filters; }
        }
    }
}