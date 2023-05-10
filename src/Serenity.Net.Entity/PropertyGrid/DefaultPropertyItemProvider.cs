using Microsoft.Extensions.DependencyInjection;

namespace Serenity.PropertyGrid;

/// <summary>
/// Default property item provider
/// </summary>
/// <seealso cref="IPropertyItemProvider" />
public partial class DefaultPropertyItemProvider : IPropertyItemProvider
{
    private readonly IServiceProvider provider;
    private readonly IEnumerable<ObjectFactory> processorFactories;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPropertyItemProvider"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="typeSource">The type source.</param>
    /// <exception cref="ArgumentNullException">
    /// provider or typeSource is null
    /// </exception>
    public DefaultPropertyItemProvider(IServiceProvider provider, ITypeSource typeSource)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        processorFactories = (typeSource ?? throw new ArgumentNullException(nameof(typeSource)))
            .GetTypesWithInterface(typeof(IPropertyProcessor))
            .Where(x => !x.IsAbstract && !x.IsInterface)
            .Select(type => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="predicate"><inheritdoc/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">type is null</exception>
    /// <exception cref="InvalidProgramException">CheckNames is true and there is name mismatch</exception>
    public IEnumerable<PropertyItem> GetPropertyItemsFor(Type type, Func<PropertyInfo, bool> predicate)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        var list = new List<PropertyItem>();

        var basedOnRow = GetBasedOnRow(type, out bool checkNames);
        var processors = processorFactories.Select(x => (IPropertyProcessor)x(provider, Array.Empty<object>()))
            .OrderBy(x => x.Priority).ToList();

        foreach (var processor in processors)
        {
            processor.Items = list;
            processor.Type = type;
            processor.BasedOnRow = basedOnRow;
            processor.Initialize();
        }

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .OrderBy(x => x.MetadataToken))
        {
            if (property.GetCustomAttribute<IgnoreAttribute>(false) != null ||
                (predicate != null && predicate(property) == false))
                continue;

            var source = new PropertyInfoSource(property, basedOnRow);
            if (checkNames &&
                property.GetCustomAttribute<NotMappedAttribute>() == null &&
                property.GetCustomAttribute<IgnoreNameAttribute>() == null)
            {
                if (source.BasedOnField is null)
                {
                    throw new InvalidProgramException(string.Format(
                        "{0} has a [BasedOnRow(typeof({2}), CheckNames = true)] attribute but its '{1}' property " +
                        "doesn't have a matching field with same property / field name in the row.\n\n" +
                        "Please check if property is named correctly.\n\n" +
                        "To remove this validation you may set CheckNames to false on [BasedOnRow] attribute.\n\n" +
                        "To disable check for this specific property add a [IgnoreName] attribute to the property itself.",
                        type.FullName, property.Name, basedOnRow.GetType().FullName));
                }
                else if (
                    (!source.BasedOnField.PropertyName.IsEmptyOrNull() &&
                     source.BasedOnField.PropertyName != property.Name) ||
                    (source.BasedOnField.PropertyName.IsEmptyOrNull() &&
                     source.BasedOnField.Name != property.Name))
                {
                    throw new InvalidProgramException(string.Format(
                            "{0} has a [BasedOnRow(typeof({3}), CheckNames = true)] attribute but its '{1}' property " +
                            "doesn't match the property/field name '{2}' in the row.\n\n" +
                            "Property names must match case sensitively. Please change property name to '{2}'.\n\n" +
                            "To remove this validation you may set CheckNames to false on [BasedOnRow] attribute.\n\n" +
                            "To disable check for this specific property add a [IgnoreName] attribute to the property itself.",
                            type.FullName, property.Name, source.BasedOnField.PropertyName.TrimToNull() ??
                                source.BasedOnField.Name, basedOnRow.GetType().FullName));
                }
            }

            PropertyItem pi = new PropertyItem
            {
                Name = property.Name
            };

            foreach (var processor in processors)
                processor.Process(source, pi);

            list.Add(pi);
        }

        foreach (var processor in processors)
            processor.PostProcess();

        return list;
    }

    private static IRow GetBasedOnRow(Type type, out bool checkPropertyNames)
    {
        checkPropertyNames = false;
        var basedOnRowAttr = type.GetCustomAttribute<BasedOnRowAttribute>();
        if (basedOnRowAttr == null)
            return null;

        var basedOnRowType = basedOnRowAttr.RowType;
        if (!typeof(IRow).IsAssignableFrom(basedOnRowType) ||
            basedOnRowType.IsInterface ||
            basedOnRowType.IsAbstract)
            throw new InvalidOperationException(string.Format(
                "BasedOnRowAttribute value ({0}) must be set to a subclass of {1}!",
                    type.FullName, typeof(IRow).FullName));

        checkPropertyNames = basedOnRowAttr.CheckNames;
        return (IRow)Activator.CreateInstance(basedOnRowType);
    }
}