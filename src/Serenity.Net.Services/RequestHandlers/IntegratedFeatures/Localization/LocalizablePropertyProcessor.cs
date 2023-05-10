namespace Serenity.PropertyGrid;

/// <summary>
/// Property processor for handling localizable properties
/// </summary>
public partial class LocalizablePropertyProcessor : PropertyProcessor
{
    private LocalizationRowAttribute localAttr;
    private ILocalizationRow localRowInstance;
    private int localRowPrefixLength;
    private Field mappedIdField;
    private int rowPrefixLength;

    /// <inheritdoc/>
    public override void Initialize()
    {
        localAttr = null;

        if (BasedOnRow == null)
            return;

        localAttr = BasedOnRow.GetType().GetCustomAttribute<LocalizationRowAttribute>(false);
        if (localAttr == null)
            return;

        if (!typeof(ILocalizationRow).IsAssignableFrom(localAttr.LocalizationRow))
            throw new InvalidOperationException(string.Format("Localization table {0} doesn't implement ILocalizationRow interface!",
                localAttr.LocalizationRow.FullName));
    }

    /// <inheritdoc/>
    public override void Process(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<LocalizableAttribute>();
        if (attr != null)
        {
            if (attr.IsLocalizable)
                item.Localizable = true;

            return;
        }

        if (source.BasedOnField is not null &&
            IsLocalized(source.BasedOnField))
        {
            item.Localizable = true;
        }
    }

    /// <inheritdoc/>
    public override int Priority => 15;

    private bool IsEnabled()
    {
        if (localAttr == null)
            return false;

        if (localRowInstance != null)
            return true;

        localRowInstance = (ILocalizationRow)Activator.CreateInstance(localAttr.LocalizationRow);
        rowPrefixLength = PrefixHelper.DeterminePrefixLength(BasedOnRow.EnumerateTableFields(),
            x => x.Name);
        localRowPrefixLength = PrefixHelper.DeterminePrefixLength(localRowInstance.EnumerateTableFields(),
            x => x.Name);
        mappedIdField = localRowInstance.FindField(localAttr.MappedIdField ?? BasedOnRow.IdField.Name);
        if (mappedIdField is null)
            throw new InvalidOperationException(string.Format("Can't locate localization table mapped ID field for {0}!",
                localRowInstance.Table));

        return true;
    }

    /// <summary>
    /// Returns if the field is localizable
    /// </summary>
    /// <param name="field">Field object</param>
    public bool IsLocalized(Field field)
    {
        return IsEnabled() && LocalizationBehavior.GetLocalizationMatch(field,
            localRowInstance, localRowPrefixLength, rowPrefixLength) is not null;
    }
}
