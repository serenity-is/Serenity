using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    /// <summary>
    /// Base class that stores a static, cached list of TBehavior behaviors that are 
    /// implicitly or explicitly activated for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public abstract class RowBehaviors<TRow, TBehavior>
        where TBehavior: class
        where TRow: Row, new()
    {
        static IEnumerable<TBehavior> behaviors;

        /// <summary>
        /// Cached list of behaviors for TRow type.
        /// </summary>
        public static IEnumerable<TBehavior> Default
        {
            get
            {
                if (behaviors != null)
                    return behaviors;

                var list = new List<TBehavior>();

                TRow row = new TRow();

                var registry = Dependency.TryResolve<IImplicitBehaviorRegistry>() ??
                    DefaultImplicitBehaviorRegistry.Instance;

                foreach (var behaviorType in registry.GetTypes())
                {
                    var behavior = Activator.CreateInstance(behaviorType) as TBehavior;
                    if (behavior == null)
                        continue;

                    var implicitBehavior = behavior as IImplicitBehavior;
                    if (implicitBehavior == null)
                        continue;

                    var fieldBehavior = behavior as IFieldBehavior;
                    if (fieldBehavior == null)
                    {
                        if (implicitBehavior.ActivateFor(row))
                            list.Add(behavior);

                        continue;
                    }

                    foreach (var field in row.GetFields())
                    {
                        fieldBehavior.Target = field;
                        if (implicitBehavior.ActivateFor(row))
                        {
                            list.Add(behavior);

                            behavior = Activator.CreateInstance(behaviorType) as TBehavior;
                            implicitBehavior = behavior as IImplicitBehavior;
                            fieldBehavior = behavior as IFieldBehavior;
                        }
                    }
                }

                foreach (var attr in row.GetType().GetCustomAttributes())
                {
                    var behavior = attr as TBehavior;
                    if (behavior == null)
                        continue;

                    list.Add(behavior);
                }

                foreach (var field in row.GetFields())
                {
                    if (field.CustomAttributes == null)
                        continue;

                    foreach (var attr in field.CustomAttributes)
                    {
                        var behavior = attr as TBehavior;
                        if (behavior == null)
                            continue;

                        var fieldBehavior = behavior as IFieldBehavior;
                        if (fieldBehavior == null)
                            continue;

                        fieldBehavior.Target = field;
                        list.Add(behavior);
                    }
                }

                behaviors = list;
                return list;
            }
        }
    }
}