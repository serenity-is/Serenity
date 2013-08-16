using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Collections;

namespace Serenity.Data
{
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
    public sealed class WeakHashtable : Hashtable
    {
        private static IEqualityComparer _comparer = new WeakKeyComparer();
        private long _lastGlobalMem;
        private int _lastHashCount;

        internal WeakHashtable()
            : base(_comparer)
        {
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void Remove(object key)
        {
            base.Remove(key);
        }

        private void ScavengeKeys()
        {
            int count = this.Count;
            if (count != 0)
            {
                if (this._lastHashCount == 0)
                {
                    this._lastHashCount = count;
                }
                else
                {
                    long totalMemory = GC.GetTotalMemory(false);
                    if (this._lastGlobalMem == 0L)
                    {
                        this._lastGlobalMem = totalMemory;
                    }
                    else
                    {
                        float num3 = ((float)(totalMemory - this._lastGlobalMem)) / ((float)this._lastGlobalMem);
                        float num4 = ((float)(count - this._lastHashCount)) / ((float)this._lastHashCount);
                        if ((num3 < 0f) && (num4 >= 0f))
                        {
                            ArrayList list = null;
                            foreach (object obj2 in this.Keys)
                            {
                                WeakReference reference = obj2 as WeakReference;
                                if ((reference != null) && !reference.IsAlive)
                                {
                                    if (list == null)
                                    {
                                        list = new ArrayList();
                                    }
                                    list.Add(reference);
                                }
                            }
                            if (list != null)
                            {
                                foreach (object obj3 in list)
                                {
                                    this.Remove(obj3);
                                }
                            }
                        }
                        this._lastGlobalMem = totalMemory;
                        this._lastHashCount = count;
                    }
                }
            }
        }

        public void SetWeak(object key, object value)
        {
            this.ScavengeKeys();
            this[new EqualityWeakReference(key)] = value;
        }

        private sealed class EqualityWeakReference : WeakReference
        {
            private int _hashCode;

            internal EqualityWeakReference(object o)
                : base(o)
            {
                this._hashCode = o.GetHashCode();
            }

            public override bool Equals(object o)
            {
                if (o == null)
                {
                    return false;
                }
                if (o.GetHashCode() != this._hashCode)
                {
                    return false;
                }
                if ((o != this) && (!this.IsAlive || !object.ReferenceEquals(o, this.Target)))
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }
        }

        private class WeakKeyComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object x, object y)
            {
                if (x == null)
                {
                    return (y == null);
                }
                if ((y == null) || (x.GetHashCode() != y.GetHashCode()))
                {
                    return false;
                }
                WeakReference reference = x as WeakReference;
                WeakReference reference2 = y as WeakReference;
                if (reference != null)
                {
                    if (!reference.IsAlive)
                    {
                        return false;
                    }
                    x = reference.Target;
                }
                if (reference2 != null)
                {
                    if (!reference2.IsAlive)
                    {
                        return false;
                    }
                    y = reference2.Target;
                }
                return object.ReferenceEquals(x, y);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
