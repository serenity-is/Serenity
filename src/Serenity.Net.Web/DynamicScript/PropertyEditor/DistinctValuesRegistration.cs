using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Contains helper methods for distinct values scripts
/// </summary>
public class DistinctValuesRegistration
{
    /// <summary>
    /// Creates and registers dynamic scripts for row properties with 
    /// <see cref="DistinctValuesEditorAttribute"/>
    /// </summary>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="typeSource">Type source</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">Script manager, type source or
    /// service provider is null</exception>
    /// <exception cref="Exception">DistinctValuesAttribute is placed on a non-row type</exception>
    public static void RegisterDistinctValueScripts(IDynamicScriptManager scriptManager, 
        ITypeSource typeSource, IServiceProvider serviceProvider)
    {
        if (scriptManager == null)
            throw new ArgumentNullException(nameof(scriptManager));

        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var list = new List<DistinctValuesEditorAttribute>();
        foreach (var type in typeSource.GetTypes())
        {
            bool isRow = typeof(IRow).IsAssignableFrom(type) &&
                !type.IsInterface;

            if (!isRow &&
                type.GetCustomAttribute<FormScriptAttribute>() == null &&
                type.GetCustomAttribute<ColumnsScriptAttribute>() == null)
            {
                continue;
            }

            if (isRow &&
                type.IsAbstract)
                continue;

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = property.GetCustomAttribute<DistinctValuesEditorAttribute>();
                if (attr == null)
                    continue;

                if (attr.RowType != null)
                {
                    if (attr.RowType.IsInterface ||
                        attr.RowType.IsAbstract ||
                        !typeof(IRow).IsAssignableFrom(attr.RowType))
                    {
                        throw new Exception("DistinctValuesEditor can't be used with type: " +
                            attr.RowType.FullName + " as it is not a row type. This attribute is specified " +
                            "on " + property.Name + " property of " + type.FullName);
                    }

                    attr.PropertyName = attr.PropertyName.IsEmptyOrNull() ? property.Name :
                        attr.PropertyName;
                }
                else
                {
                    if (!isRow)
                    {
                        var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
                        if (basedOnRowAttr == null || basedOnRowAttr.RowType == null ||
                            basedOnRowAttr.RowType.IsAbstract ||
                            basedOnRowAttr.RowType.IsInterface ||
                            !typeof(IRow).IsAssignableFrom(basedOnRowAttr.RowType))
                        {
                            throw new Exception("Invalid usage of DistinctValuesEditor attribute on " +
                                "property " + property.Name + " of " + type.FullName + ". " +
                                "RowType has to be specified!");
                        }

                        attr.RowType = basedOnRowAttr.RowType;
                    }
                    else
                        attr.RowType = type;

                    attr.PropertyName = attr.PropertyName.IsEmptyOrNull() ? property.Name :
                        attr.PropertyName;
                }

                list.Add(attr);
            }
        }

        var byRowProperty = list.ToLookup(x => new Tuple<Type, string>(x.RowType, x.PropertyName));

        foreach (var key in byRowProperty)
        {
            var row = (IRow)Activator.CreateInstance(key.Key.Item1);

            var script = (LookupScript)ActivatorUtilities.CreateInstance(serviceProvider,
                typeof(DistinctValuesScript<>).MakeGenericType(key.Key.Item1), 
                new object[] { key.Key.Item2 });

            script.LookupKey = "Distinct." + row.GetFields().LocalTextPrefix + "." +
                key.Key.Item2;

            var withPermission = key.FirstOrDefault(x => !string.IsNullOrEmpty(x.Permission));
            if (withPermission != null)
                script.Permission = withPermission.Permission;

            var withExpiration = key.FirstOrDefault(x => x.Expiration != 0);
            if (withExpiration != null)
                script.Expiration = TimeSpan.FromSeconds(withExpiration.Expiration);

            scriptManager.Register(script.ScriptName, script);
        }
    }
}
