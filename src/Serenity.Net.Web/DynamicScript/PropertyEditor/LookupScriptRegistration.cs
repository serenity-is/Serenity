using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// Contains helper methods for lookup script registration
/// </summary>
public class LookupScriptRegistration
{
    /// <summary>
    /// Creates and registers dynamic <see cref="LookupScript"/> objects for
    /// classes with <see cref="LookupScriptAttribute"/>
    /// </summary>
    /// <param name="scriptManager">Dynamic script manager</param>
    /// <param name="typeSource">Type source</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">Script manager, type source or
    /// service provider is null</exception>
    /// <exception cref="InvalidOperationException">A custom type has LookupScriptAttribute
    /// but it does not derive from the <see cref="LookupScript"/> class.</exception>
    public static void RegisterLookupScripts(IDynamicScriptManager scriptManager, 
        ITypeSource typeSource, IServiceProvider serviceProvider)
    {
        if (scriptManager == null)
            throw new ArgumentNullException(nameof(scriptManager));

        if (typeSource == null)
            throw new ArgumentNullException(nameof(typeSource));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var registeredType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in typeSource.GetTypesWithAttribute(typeof(LookupScriptAttribute)))
        {
            if (type.IsAbstract)
                continue;

            var attr = type.GetCustomAttribute<LookupScriptAttribute>();
            LookupScript script;

            if (typeof(IRow).IsAssignableFrom(type))
            {
                if (attr.LookupType == null)
                    script = (LookupScript)ActivatorUtilities.CreateInstance(serviceProvider,
                        typeof(RowLookupScript<>).MakeGenericType(type));
                else if (attr.LookupType.IsGenericType)
                    script = (LookupScript)ActivatorUtilities.CreateInstance(serviceProvider,
                        attr.LookupType.MakeGenericType(type));
                else if (attr.LookupType.GetCustomAttribute<LookupScriptAttribute>() == null)
                    script = (LookupScript)ActivatorUtilities.CreateInstance(serviceProvider,
                        attr.LookupType);
                else
                {
                    // lookup script type already has a LookupScript attribute, 
                    // so it's dynamic script will be generated on itself
                    continue;
                }
            }
            else if (!typeof(LookupScript).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
                    "Type {0} can't be registered as a lookup script!", type.FullName));
            }
            else
            {
                script = (LookupScript)ActivatorUtilities.CreateInstance(serviceProvider, type);
            }

            script.LookupKey = attr.Key ??
                LookupScriptAttribute.AutoLookupKeyFor(type);

            if (registeredType.TryGetValue(script.LookupKey, out Type otherType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Types {0} and {1} has the same lookup key (\"{2}\"). " +
                    "\r\n\r\nPlease remove LookupScript attribute from one of them or change the lookup key!",
                    type.FullName, otherType.FullName, script.LookupKey));
            }

            registeredType[script.LookupKey] = type;

            if (attr.Permission != null)
                script.Permission = attr.Permission;

            if (attr.Expiration != 0)
                script.Expiration = TimeSpan.FromSeconds(attr.Expiration);

            scriptManager.Register(script.ScriptName, script);
        }
    }
}
