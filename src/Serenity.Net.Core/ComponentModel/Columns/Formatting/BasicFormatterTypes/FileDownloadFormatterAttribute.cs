namespace Serenity.ComponentModel;

/// <summary>
/// Sets formatting type to "FileDownload".
/// </summary>
/// <seealso cref="CustomFormatterAttribute" />
public class FileDownloadFormatterAttribute : CustomFormatterAttribute
{
    /// <summary>
    /// Formatter type key
    /// </summary>
    public const string Key = "FileDownload";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDownloadFormatterAttribute"/> class.
    /// </summary>
    public FileDownloadFormatterAttribute()
        : base(Key)
    {
    }

    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    /// <value>
    /// The display format.
    /// </value>
    public string? DisplayFormat
    {
        get { return GetOption<string>("displayFormat"); }
        set { SetOption("displayFormat", value); }
    }

    /// <summary>
    /// Gets or sets the icon class.
    /// </summary>
    /// <value>
    /// The icon class.
    /// </value>
    public string? IconClass
    {
        get { return GetOption<string>("iconClass"); }
        set { SetOption("iconClass", value); }
    }
    
    /// <summary>
    /// Gets or sets the original name property which indicates
    /// the property to read original file name from (if any).
    /// </summary>
    /// <value>
    /// The original name property.
    /// </value>
    public string? OriginalNameProperty
    {
        get { return GetOption<string>("originalNameProperty"); }
        set { SetOption("originalNameProperty", value); }
    }
}
