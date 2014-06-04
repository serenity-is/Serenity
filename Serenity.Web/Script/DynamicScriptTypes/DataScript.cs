using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Serenity.Web
{
    public class DataScript<TData> : INamedDynamicScript, ITwoLevelCached
        where TData: class
    {
        private string _scriptName;
        private bool _authorize;
        private Func<TData> _getData;
        private string _right;
        private EventHandler _scriptChanged;
        private bool _nonCached; 
        private Dictionary<string, object> _lookupParams;

        public DataScript(string name, bool authorize = false, string right = null, bool nonCached = false,
            Func<TData> getData = null)
        {
            _lookupParams = new Dictionary<string, object>();
            
            if (name == null)
                throw new ArgumentNullException("name");

            _scriptName = name;
            _getData = getData;
            _authorize = authorize;
            _right = right;
            _nonCached = nonCached;

            LocalExpiration = CacheExpiration.Never;
            RemoteExpiration = CacheExpiration.OneDay;

            DynamicScriptManager.Register(this);
        }

        public bool NonCached { get { return _nonCached; } }

        public void Changed()
        {
            if (_scriptChanged != null)
                _scriptChanged(this, new EventArgs());
        }

        public string ScriptName { get { return _scriptName; } }

        public string GetScript()
        {
            TData data = _getData();
            return String.Format("Q$ScriptData.set({0}, {1});", _scriptName.ToSingleQuoted(), data.ToJson());
        }

        public void CheckRights()
        {
            if (_authorize && !SecurityHelper.IsLoggedIn)
                throw new AccessViolationException(String.Format("{0} script'ine yalnızca giriş yapmış kullanıcılar tarafından erişilebilir!"));

            if (_right != null)
                SecurityHelper.EnsurePermission(_right, RightErrorHandling.ThrowException);
        }

        public event System.EventHandler ScriptChanged
        {
            add { _scriptChanged += value; }
            remove { _scriptChanged -= value; }
        }

        public string GlobalGenerationKey { get; set; }
        public TimeSpan LocalExpiration { get; set; }
        public TimeSpan RemoteExpiration { get; set; }
    }
}