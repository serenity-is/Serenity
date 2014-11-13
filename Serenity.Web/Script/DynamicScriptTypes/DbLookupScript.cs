using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Web
{
    public class DbLookupScript<TItem> : INamedDynamicScript
        where TItem: class, new()
    {
        private string _scriptName;
        private string _schema;
        private string _lookup;
        private bool _authorize;
        private Func<IDbConnection, IEnumerable> _getItems;
        private string _right;
        private EventHandler _scriptChanged;
        private bool _nonCached; 
        private Dictionary<string, object> _lookupParams;

        public DbLookupScript(string name, string connectionKey = null, bool authorize = false, string right = null, bool nonCached = false,
            Func<IDbConnection, IEnumerable<TItem>> getItems = null)
        {
            _lookupParams = new Dictionary<string, object>();
            
            Row row = null;
            if (typeof(TItem).IsSubclassOf(typeof(Row)))
                row = (Row)(object)(new TItem());

            if (name == null)
            {
                if (row == null)
                    throw new ArgumentNullException("name");

                name = row.Table;
            }

            if (row != null)
            {
                Field field;

                var idRow = row as IIdRow;
                if (idRow != null)
                {
                    field = ((Field)idRow.IdField);
                    IdField = field.PropertyName ?? field.Name;
                }

                var nameRow = row as INameRow;
                if (nameRow != null)
                {
                    field = ((Field)nameRow.NameField);
                    TextField = field.PropertyName ?? field.Name;
                }

                var treeRow = row as IParentIdRow;
                if (treeRow != null)
                {
                    field = ((Field)treeRow.ParentIdField);
                    ParentIdField = field.PropertyName ?? field.Name;
                }
            }

            _scriptName = "Lookup." + name;

            if (connectionKey == null)
            {
                if (row != null)
                    connectionKey = RowRegistry.GetConnectionKey(row);
                else
                    connectionKey = RowRegistry.DefaultConnectionKey;
            }

            _schema = connectionKey;

            _lookup = name;
            _getItems = getItems;
            _authorize = authorize;
            _right = right;
            _nonCached = nonCached;

            if (row != null)
                GroupKey = row.GetFields().GenerationKey;

            Expiration = _nonCached ? TimeSpan.FromDays(-1) : TimeSpan.Zero;

            DynamicScriptManager.Register(this);
        }

        public bool NonCached { get { return _nonCached; } }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        protected virtual IDbConnection CreateConnection()
        {
            return SqlConnections.NewByKey(_schema);
        }

        public string GetScript()
        {
            IEnumerable items;
            using (var connection = CreateConnection())
            {
                items = _getItems(connection);
            }
            return String.Format("Q$ScriptData.set({0}, new Q$Lookup({1}, \n{2}\n));", 
                ("Lookup." + _lookup).ToSingleQuoted(), LookupParams.ToJson(), items.ToJson());
 	    }

        public Dictionary<string, object> LookupParams
        {
            get { return _lookupParams; }
        }

        public string IdField
        {
            get 
            { 
                object value;
                if (_lookupParams.TryGetValue("idField", out value) && value != null)
                    return value.ToString();
                return null;
            }
            set 
            {
                _lookupParams["idField"] = value;
            }
        }

        public string TextField
        {
            get 
            { 
                object value;
                if (_lookupParams.TryGetValue("textField", out value) && value != null)
                    return value.ToString();
                return null;
            }
            set 
            {
                _lookupParams["textField"] = value;
            }
        }

        public string ParentIdField
        {
            get
            { 
                object value;
                if (_lookupParams.TryGetValue("parentIdField", out value) && value != null)
                    return value.ToString();
                return null;
            }
            set 
            {
                _lookupParams["parentIdField"] = value;
            }
        }

        public void CheckRights()
        {
            if (_authorize && !Authorization.IsLoggedIn)
                throw new AccessViolationException(String.Format("{0} script'ine yalnızca giriş yapmış kullanıcılar tarafından erişilebilir!"));

            if (_right != null)
                Authorization.ValidatePermission(_right);
        }

        public event System.EventHandler ScriptChanged
        {
            add { _scriptChanged += value; }
            remove { _scriptChanged -= value; }
        }

        public string GroupKey { get; set; }
        public TimeSpan Expiration { get; set; }
    }
}