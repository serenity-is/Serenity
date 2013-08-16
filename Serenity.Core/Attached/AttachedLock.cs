using System;
using Serenity.Data;

namespace Serenity.Data
{
    public class AttachedLock : IDisposable
    {
        private ISupportAttached _target;
        private AttachedProperty _property;

        public AttachedLock(ISupportAttached target, AttachedProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (target == null)
                throw new ArgumentNullException("target");

            this._target = target;
            this._property = property;

            this._target.SetAttached(this._property, this._target.GetAttached<int>(this._property) + 1);
        }

        public static bool IsLocked(ISupportAttached target, AttachedProperty property)
        {
            return target.GetAttached<int>(property) > 0;
        }

        public void Dispose()
        {
            this._target.SetAttached(this._property, this._target.GetAttached<int>(this._property) - 1);
        }
    }
}
