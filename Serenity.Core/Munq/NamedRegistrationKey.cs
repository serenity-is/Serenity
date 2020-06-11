#if NET45
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
    internal class NamedRegistrationKey : IRegistrationKey
    {
        internal Type InstanceType;
        internal string Name;

        public NamedRegistrationKey(string name, Type type)
        {
            Name = name ?? String.Empty;
            InstanceType = type;
        }

        public Type GetInstanceType() { return InstanceType; }

        // comparison methods
        public override bool Equals(object obj)
        {
            var r = obj as NamedRegistrationKey;
            return (r != null) &&
                Object.ReferenceEquals(InstanceType, r.InstanceType) &&
                String.Compare(Name, r.Name, true) == 0; // ignore case
        }

        public override int GetHashCode()
        {
            return InstanceType.GetHashCode();
        }
    }
}
#endif