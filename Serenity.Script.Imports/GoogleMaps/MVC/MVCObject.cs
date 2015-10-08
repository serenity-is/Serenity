using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace google.maps
{
    [Imported]
    public partial class MVCObject
    {
        /// <summary>
        /// Base class implementing KVO.
        /// </summary>
        public MVCObject()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds a View to a Model.
        /// </summary>
        public void BindTo(string key, MVCObject target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds a View to a Model.
        /// </summary>
        public void BindTo(string key, MVCObject target, string targetKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds a View to a Model.
        /// </summary>
        public void BindTo(string key, MVCObject target, string targetKey, bool noNotify)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generic handler for state changes. Override this in derived classes to handle arbitrary state changes.
        /// </summary>
        public void Changed(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value.
        /// </summary>
        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notify all observers of a change on this property. This notifies both objects that are bound to the object's property as well as the object that it is bound to.
        /// </summary>
        public void Notify(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a value.
        /// </summary>
        public void Set(string key, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a collection of key-value pairs.
        /// </summary>
        public void SetValues(object values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a binding.  Unbinding will set the unbound property to the current value.  The object will not be notified, as the value has not changed.
        /// </summary>
        public void Unbind(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all bindings.
        /// </summary>
        public void UnbindAll()
        {
            throw new NotImplementedException();
        }
    }
}