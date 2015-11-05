using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    /// <summary>
    /// Stores a static, cached list of behaviors that are implicitly or explicitly activated
    /// for a row type (TRow).
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public class RowSaveBehaviors<TRow>
        where TRow: Row, new()
    {
        static IEnumerable<ISaveBehavior> behaviors;

        /// <summary>
        /// Cached list of behaviors for TRow type.
        /// </summary>
        public static IEnumerable<ISaveBehavior> Default
        {
            get
            {
                if (behaviors != null)
                    return behaviors;

                var list = new List<ISaveBehavior>();

                TRow row = new TRow();

                var registry = Dependency.TryResolve<IImplicitBehaviorRegistry>();
                if (registry != null)
                {
                    foreach (var behaviorType in registry.GetTypes())
                    {
                        var saveBehavior = Activator.CreateInstance(behaviorType) as ISaveBehavior;
                        if (saveBehavior == null)
                            continue;

                        var implicitBehavior = saveBehavior as IImplicitBehavior;
                        if (implicitBehavior == null)
                            continue;

                        var fieldBehavior = saveBehavior as IFieldBehavior;
                        if (fieldBehavior == null)
                        {
                            if (implicitBehavior.ActivateFor(row))
                                list.Add(saveBehavior);

                            continue;
                        }

                        foreach (var field in row.GetFields())
                        {
                            fieldBehavior.Target = field;
                            if (implicitBehavior.ActivateFor(row))
                            {
                                list.Add(saveBehavior);

                                saveBehavior = Activator.CreateInstance(behaviorType) as ISaveBehavior;
                                implicitBehavior = saveBehavior as IImplicitBehavior;
                                fieldBehavior = saveBehavior as IFieldBehavior;
                            }
                        }
                    }
                }

                foreach (var attr in row.GetType().GetCustomAttributes())
                {
                    var saveBehavior = attr as ISaveBehavior;
                    if (saveBehavior == null)
                        continue;

                    list.Add(saveBehavior);
                }

                foreach (var field in row.GetFields())
                {
                    if (field.CustomAttributes == null)
                        continue;

                    foreach (var attr in field.CustomAttributes)
                    {
                        var saveBehavior = attr as ISaveBehavior;
                        if (saveBehavior == null)
                            continue;

                        var fieldBehavior = saveBehavior as IFieldBehavior;
                        if (fieldBehavior == null)
                            continue;

                        fieldBehavior.Target = field;
                        list.Add(saveBehavior);
                    }
                }

                behaviors = list;
                return list;
            }
        }
    }
}