using Serenity.Abstractions;
#if COREFX
using IDictionary = System.Collections.Generic.IDictionary<object, object>;
#else
using System.Collections;
using System.Collections.Generic;
using System;
#endif
#if ASPNETCORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace Serenity.Web
{
    public class RequestContext : IRequestContext
    {
        public IDictionary Items
        {
            get 
            {
#if ASPNETCORE
                var context = Dependency.Resolve<IHttpContextAccessor>().HttpContext;
#if COREFX
                if (context != null)
                    return context.Items;
#else
                if (context != null)
                    return new GenericDictionaryWrapper(context.Items);
#endif
#else
                if (HttpContext.Current != null)
                    return HttpContext.Current.Items;
#endif

                return null;
            }
        }

#if ASPNETCORE
#if !COREFX
        private class GenericDictionaryWrapper : IDictionary
        {
            private readonly IDictionary<object, object> dictionary;

            public GenericDictionaryWrapper(IDictionary<object, object> dictionary)
            {
                this.dictionary = dictionary;
            }

            public object this[object key] { get => dictionary[key]; set => dictionary[key] = value; }

            public ICollection Keys => throw new NotImplementedException();

            public ICollection Values => throw new NotImplementedException();

            public bool IsReadOnly => dictionary.IsReadOnly;

            public bool IsFixedSize => throw new NotImplementedException();

            public int Count => dictionary.Count;

            public object SyncRoot => throw new NotImplementedException();

            public bool IsSynchronized => throw new NotImplementedException();

            public void Add(object key, object value)
            {
                dictionary.Add(key, value);
            }

            public void Clear()
            {
                dictionary.Clear();
            }

            public bool Contains(object key)
            {
                return dictionary.ContainsKey(key);
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                dictionary.Remove(key);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return dictionary.GetEnumerator();
            }
        }
#endif
#endif
    }
}

