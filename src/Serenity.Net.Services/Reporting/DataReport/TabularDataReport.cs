using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;
using System.Collections;

namespace Serenity.Reporting;

/// <summary>
/// An implementation of a data only report which is used mainly for Excel export.
/// </summary>
public class TabularDataReport : IDataOnlyReport
{
    /// <summary>
    /// The report data
    /// </summary>
    protected IEnumerable Data { get; set; }

    /// <summary>
    /// Columns type to use for export. This is used to determine
    /// the list of columns and their formats, decorator types etc.
    /// </summary>
    protected Type ColumnsType { get; set; }

    /// <summary>
    /// List of columns to export. If <see cref="ColumnsType"/> or 
    /// <see cref="ColumnList"/> is passed this is used to 
    /// subset the columns. Otherwise it contains the list of 
    /// columns to export.
    /// </summary>
    protected IEnumerable<string> ExportColumns { get; set; }

    /// <summary>
    /// Service provider.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// The column list
    /// </summary>
    protected List<ReportColumn> ColumnList { get; set; }

    /// <summary>
    /// A group key for caching
    /// </summary>
    public const string CacheGroupKey = nameof(TabularDataReport);

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    protected TabularDataReport()
    {
    }

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="data">The data objects</param>
    /// <param name="columns">List of report columns</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TabularDataReport(IEnumerable data, IEnumerable<ReportColumn> columns)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ColumnList = (columns ?? throw new ArgumentNullException(nameof(columns))).ToList();
    }

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="data">The data objects</param>
    /// <param name="columnsType">The columns type</param>
    /// <param name="serviceProvider">Service provider that will be used to
    /// extract PropertyItems from the columns type</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TabularDataReport(IEnumerable data, Type columnsType,
        IServiceProvider serviceProvider)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ColumnsType = columnsType ?? throw new ArgumentNullException(nameof(columnsType));
        ServiceProvider = serviceProvider ??
            throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Creates an instance of the class.
    /// </summary>
    /// <param name="data">The data objects</param>
    /// <param name="columnsType">The columns type</param>
    /// <param name="exportColumns">The list of column property names to export</param>
    /// <param name="serviceProvider">Service provider that will be used to
    /// extract PropertyItems from the columns type</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TabularDataReport(IEnumerable data, Type columnsType, IEnumerable<string> exportColumns,
        IServiceProvider serviceProvider)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ColumnsType = columnsType ?? throw new ArgumentNullException(nameof(columnsType));
        ExportColumns = exportColumns ?? throw new ArgumentNullException(nameof(exportColumns));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public virtual object GetData()
    {
        return Data;
    }

    /// <inheritdoc/>
    public virtual List<ReportColumn> GetColumnList()
    {
        if (ColumnList != null)
            return ColumnList;

        if (ColumnsType != null)
            return GetColumnListFor(ColumnsType, ExportColumns, ServiceProvider);

        return ExportColumns.Select(x => new ReportColumn
        {
            Name = x
        }).ToList();
    }

    /// <summary>
    /// Extracts report columns from a columns type
    /// </summary>
    /// <param name="columnsType">The columns type</param>
    /// <param name="exportColumns">The list of column property names to export</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">columnsType or serviceProvider is null</exception>
    public static List<ReportColumn> GetColumnListFor(Type columnsType,
        IEnumerable<string> exportColumns, IServiceProvider serviceProvider)
    {
        if (columnsType == null)
            throw new ArgumentNullException(nameof(columnsType));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var list = new List<ReportColumn>();
        if (exportColumns != null && !exportColumns.Any())
            return list;

        List<PropertyItem> propertyItems = null;
        IDictionary<string, PropertyItem> propertyItemByName = null;
        IDictionary<string, PropertyInfo> propertyInfos = null;
        IRow basedOnRow = null;
        if (columnsType != null)
        {
            var cache = serviceProvider.GetRequiredService<ITwoLevelCache>();
            propertyItems = cache.GetLocalStoreOnly(CacheGroupKey + ":Columns:" + columnsType.FullName,
                TimeSpan.Zero, CacheGroupKey, () =>
                {
                    var propertyItemProvider = serviceProvider.GetRequiredService<IPropertyItemProvider>();
                    var items = propertyItemProvider.GetPropertyItemsFor(columnsType).ToList();

                    if (typeof(ICustomizePropertyItems).IsAssignableFrom(columnsType))
                    {
                        var instance = ActivatorUtilities.CreateInstance(serviceProvider, 
                            columnsType) as ICustomizePropertyItems;
                        instance.Customize(items);
                    }

                    return items;
                });

            propertyItemByName = propertyItems.ToDictionary(x => x.Name);
            propertyInfos = columnsType.GetProperties().ToDictionary(x => x.Name);

            var basedOnAttr = columnsType.GetCustomAttribute<BasedOnRowAttribute>();
            if (basedOnAttr != null &&
                basedOnAttr.RowType != null &&
                !basedOnAttr.RowType.IsInterface &&
                !basedOnAttr.RowType.IsAbstract &&
                typeof(IRow).IsAssignableFrom(basedOnAttr.RowType))
            {
                basedOnRow = (IRow)Activator.CreateInstance(basedOnAttr.RowType);
            }
        }

        if (exportColumns == null)
            exportColumns = propertyItems.Select(x => x.Name).ToList();

        foreach (var columnName in exportColumns)
        {
            if (!propertyItemByName.TryGetValue(columnName, out PropertyItem item))
                continue;

            var basedOnField = basedOnRow == null ? null :
                (basedOnRow.FindField(columnName) ?? basedOnRow.FindFieldByPropertyName(columnName));

            if (propertyInfos == null || !propertyInfos.TryGetValue(columnName, out PropertyInfo p))
                p = null;

            list.Add(FromPropertyItem(item, basedOnField, p, serviceProvider, 
                serviceProvider.GetRequiredService<ITextLocalizer>()));
        }

        return list;
    }

    /// <summary>
    /// Creates a <see cref="ReportColumn"/> from a <see cref="PropertyItem"/>
    /// </summary>
    /// <param name="item">Property item</param>
    /// <param name="field">Optional field object</param>
    /// <param name="property">PropertyInfo if any</param>
    /// <param name="provider">Service provider</param>
    /// <param name="localizer">Text localizer</param>
    /// <exception cref="ArgumentNullException">item or localizer is null</exception>
    public static ReportColumn FromPropertyItem(PropertyItem item, Field field, 
        PropertyInfo property, IServiceProvider provider, ITextLocalizer localizer)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (localizer is null)
            throw new ArgumentNullException(nameof(localizer));

        var result = new ReportColumn
        {
            Name = item.Name,
            Title = item.Title ?? item.Name
        };

        if (result.Title != null)
            result.Title = localizer.TryGet(result.Title) ?? result.Title;

        if (item.Width != null)
            result.Width = item.Width;

        if (!string.IsNullOrWhiteSpace(item.DisplayFormat))
        {
            if (item.FormatterType == "Date" || item.FormatterType == "DateTime")
            {
                result.Format = item.DisplayFormat switch
                {
                    "d" => DateHelper.CurrentDateFormat,
                    "g" => DateHelper.CurrentDateTimeFormat.Replace(":ss", "", StringComparison.Ordinal),
                    "G" => DateHelper.CurrentDateTimeFormat,
                    "s" => "yyyy-MM-ddTHH:mm:ss",
                    "u" => "yyyy-MM-ddTHH:mm:ss.fffZ",
                    _ => item.DisplayFormat,
                };
            }
            else
                result.Format = item.DisplayFormat;
        }
        else
        {
            var dtf = field as DateTimeField;
            if (dtf is not null &&
                dtf.DateTimeKind != DateTimeKind.Unspecified)
            {
                result.Format = DateHelper.CurrentDateTimeFormat;
            }
            else if (dtf is not null)
            {
                result.Format = DateHelper.CurrentDateFormat;
            }
        }

        if (field is IEnumTypeField enumField && enumField.EnumType != null)
        {
            result.Decorator = new EnumDecorator(enumField.EnumType, localizer);
        }

        if (property != null)
        {
            var decorator = property.GetCustomAttribute<CellDecoratorAttribute>();
            if (decorator != null && decorator.DecoratorType != null)
                result.Decorator = (ICellDecorator)ActivatorUtilities.CreateInstance(provider, 
                    decorator.DecoratorType);
        }

        if (field is not null)
        {
            result.Title ??= field.GetTitle(localizer);

            if (result.Width == null && field is StringField && field.Size != 0)
                result.Width = field.Size;
        }

        result.DataType = field?.ValueType;

        return result;
    }
}