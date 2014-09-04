using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Linq;

namespace Serenity.Reflection
{
    /// <summary>
    /// This class defines all the extension methods provided by the Copyable framework 
    /// on the <see cref="System.Object"/> type.
    /// </summary>
    public static class CloneHelper
    {
        /// <summary>
        /// Creates a copy of the object.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object Copy(this object instance)
        {
            if (instance == null)
                return null;

            var copy = InstanceCreator.GetInstance(instance.GetType());
            Copy(instance, copy);
            //var ser1 = Serialize(instance);
            //var ser2 = Serialize(copy);
            //if (!CompareMemoryStreams(ser1, ser2))
            //{
            //    var co = new CompareObjects();
            //    co.CompareChildren = true;
            //    co.ComparePrivateFields = true;
            //    co.ComparePrivateProperties = true;
            //    co.Compare(instance, copy);
            //    throw new InvalidOperationException(co.DifferencesString);
            //}
            return copy;
        }

        //public static MemoryStream Serialize(object s)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    bf.Serialize(ms, s);
        //    ms.Position = 0;
        //    return ms;
        //}

        //private static bool CompareMemoryStreams(MemoryStream ms1, MemoryStream ms2)
        //{
        //    if (ms1.Length != ms2.Length)
        //        return false;
        //    ms1.Position = 0;
        //    ms2.Position = 0;

        //    var msArray1 = ms1.ToArray();
        //    var msArray2 = ms2.ToArray();
        //    return msArray1.SequenceEqual(msArray2);
        //}

        /// <summary>
        /// Creates a deep copy of the object using the supplied object as a target for the copy operation.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="copy">The object to copy values to. All fields of this object will be overwritten.</param>
        /// <returns>A deep copy of the object.</returns>
        public static object Copy(this object instance, object copy)
        {
            if (instance == null)
                return null;
            if (copy == null)
                throw new ArgumentNullException("The copy instance cannot be null");
            return Clone(instance, new VisitedGraph(), copy);
        }

        /// <summary>
        /// Creates a deep copy of an object using the supplied dictionary of visited objects as 
        /// a source of objects already encountered in the copy traversal. The dictionary of visited 
        /// objects is used for holding objects that have already been copied, to avoid erroneous 
        /// duplication of parts of the object graph.
        /// </summary>
        /// <param name="instance">The object to be copied.</param>
        /// <param name="visited">The graph of objects visited so far.</param>
        /// <returns></returns>
        private static object Clone(this object instance, VisitedGraph visited)
        {
            if (instance == null)
                return null;

            Type instanceType = instance.GetType();

            if (instanceType.IsValueType || instanceType == typeof(string))
                return instance; // Value types and strings are immutable
            else if (instanceType.IsArray)
            {
                int length = ((Array)instance).Length;
                Array copied = (Array)Activator.CreateInstance(instanceType, length);
                visited.Add(instance, copied);
                for (int i = 0; i < length; ++i)
                    copied.SetValue(((Array)instance).GetValue(i).Clone(visited), i);
                return copied;
            }
            else
                return Clone(instance, visited, InstanceCreator.GetInstance(instanceType));
        }

        private static readonly Hashtable typeFieldCache = new Hashtable();

        private struct FieldAccessor
        {
            public Func<Object, Object> Getter;
            public Action<Object, Object> Setter;
        }

        private static FieldAccessor[] GetTypeCopyableFieldList(Type type)
        {

            if (type == null)
                throw new ArgumentNullException("type");

            FieldAccessor[] obj = (FieldAccessor[])typeFieldCache[type];
            if (obj != null) 
                return obj;

            lock (typeFieldCache)
            {
                obj = (FieldAccessor[])typeFieldCache[type];
                if (obj != null)
                    return obj;

                var lst = new List<FieldAccessor>();

                var current = type;
                while (current != null)
                {
                    foreach (FieldInfo field in current.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        if (!field.IsInitOnly)
                        {
                            var accessor = new FieldAccessor();
                            accessor.Getter = ILGeneration.GenerateGetter(field);
                            accessor.Setter = ILGeneration.GenerateSetter(field);
                            lst.Add(accessor);
                        }

                    current = current.BaseType;
                }

                obj = lst.ToArray();

                typeFieldCache[type] = obj;
                return obj;
            }
        }

        private static object Clone(this object instance, VisitedGraph visited, object copy)
        {
            visited.Add(instance, copy);

            var type = instance.GetType();

            foreach (var field in GetTypeCopyableFieldList(type))
            {
                object value = field.Getter(instance);
                object cloned;
                if (value == null)
                    cloned = null;
                else if (!visited.TryGetValue(value, out cloned))
                    cloned = value.Clone(visited);
                field.Setter(copy, cloned);
            }
            return copy;
        }
    }
}
