// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
    internal class UnNamedRegistrationKey : IRegistrationKey
    {
        internal Type InstanceType;

        public UnNamedRegistrationKey(Type type)
        {
            InstanceType = type;
        }

        public Type GetInstanceType() { return InstanceType; }

        // comparison methods
        public override bool Equals(object obj)
        {
            var r = obj as UnNamedRegistrationKey;
            return (r != null) && Object.ReferenceEquals(InstanceType, r.InstanceType);
        }

        public override int GetHashCode()
        {
            return InstanceType.GetHashCode();
        }
    }
}
