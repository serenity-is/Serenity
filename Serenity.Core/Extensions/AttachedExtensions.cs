using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Serenity.Data;

namespace Serenity.Data
{
    public static class Attached
    {
        public static void SetAttached(this ISupportAttached obj, AttachedProperty property, object value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            /*ISupportAttached att = obj as ISupportAttached;
            if (att == null)
                throw new ArgumentOutOfRangeException("obj", obj.GetType().Name + " doesn't support attached properties");*/

            Hashtable prop = obj.AttachedProperties;
            if (value == null)
            {
                if (prop == null)
                    return;

                prop.Remove(property);
            }
            else
            {
                if (prop == null)
                {
                    prop = new Hashtable();
                    obj.AttachedProperties = prop;
                }

                prop[property] = value;
            }
        }

        public static T GetAttached<T>(this ISupportAttached obj, AttachedProperty property)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            /*ISupportAttached att = obj as ISupportAttached;
            if (att == null)
                throw new ArgumentOutOfRangeException("obj", obj.GetType().Name + " doesn't support attached properties");*/

            Hashtable prop = obj.AttachedProperties;
            if (prop == null)
                return default(T);
            object value = prop[property];
            if (value == null)
                return default(T);
            return (T)value;
        }
    }
}