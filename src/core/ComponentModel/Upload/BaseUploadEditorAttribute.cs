using Serenity.Web;

namespace Serenity.ComponentModel;

/// <summary>
/// Sets editor type as "ImageUpload", which only allows image files.
/// Make sure you use this attribute in Row.cs, not Form.cs as
/// the image upload behavior only works if it is in the row, otherwise
/// your files will stay in the temporary directory.
/// </summary>
/// <seealso cref="CustomEditorAttribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="BaseUploadEditorAttribute"/> class.
/// </remarks>
/// <param name="editorType">Type of the editor.</param>
public abstract class BaseUploadEditorAttribute(string editorType) : CustomEditorAttribute(editorType),
    IUploadEditor, IUploadFileConstraints, IUploadFileOptions, IUploadImageConstraints, IUploadImageOptions
{

    /// <inheritdoc/>
    public bool DisableDefaultBehavior { get; set; }

    /// <inheritdoc />
    public abstract bool IsMultiple { get; }

    /// <inheritdoc/>
    public string? UploadIntent
    {
        get { return GetOption<string>("uploadIntent"); }
        set { SetOption("uploadIntent", value); }
    }

    /// <inheritdoc/>
    public bool AllowNonImage
    {
        get { return GetOption<bool>("allowNonImage"); }
        set { SetOption("allowNonImage", value); }
    }

    /// <inheritdoc/>
    public int MaxSize
    {
        get { return GetOption<int>("maxSize"); }
        set { SetOption("maxSize", value); }
    }

    /// <inheritdoc/>
    public int MinSize
    {
        get { return GetOption<int>("minSize"); }
        set { SetOption("minSize", value); }
    }

    /// <inheritdoc/>
    public int MaxHeight
    {
        get { return GetOption<int>("maxHeight"); }
        set { SetOption("maxHeight", value); }
    }

    /// <inheritdoc/>
    public int MaxWidth
    {
        get { return GetOption<int>("maxWidth"); }
        set { SetOption("maxWidth", value); }
    }

    /// <inheritdoc/>
    public int MinHeight
    {
        get { return GetOption<int>("minHeight"); }
        set { SetOption("minHeight", value); }
    }

    /// <inheritdoc/>
    public int MinWidth
    {
        get { return GetOption<int>("minWidth"); }
        set { SetOption("minWidth", value); }
    }

    /// <inheritdoc/>
    public int ScaleQuality { get; set; } = 80;

    /// <inheritdoc/>
    public int ScaleWidth { get; set; }

    /// <inheritdoc/>
    public int ScaleHeight { get; set; }

    /// <inheritdoc/>
    public bool ScaleSmaller { get; set; }

    /// <inheritdoc/>
    public ImageScaleMode ScaleMode { get; set; } = UploadOptions.DefaultScaleMode;

    /// <inheritdoc/>
    public string? ScaleBackColor { get; set; }

    /// <inheritdoc/>
    public int ThumbHeight { get; set; }

    /// <inheritdoc/>
    public int ThumbWidth { get; set; }

    /// <inheritdoc/>
    public string? ThumbSizes { get; set; }

    /// <inheritdoc/>
    public ImageScaleMode ThumbMode { get; set; } = UploadOptions.DefaultThumbMode;

    /// <inheritdoc/>
    public int ThumbQuality { get; set; } = UploadOptions.DefaultThumbQuality;

    /// <inheritdoc/>
    public string? ThumbBackColor { get; set; }

    /// <inheritdoc/>
    public bool JsonEncodeValue
    {
        get { return GetOption<bool>("jsonEncodeValue"); }
        set { SetOption("jsonEncodeValue", value); }
    }

    /// <inheritdoc/>
    public string? OriginalNameProperty
    {
        get { return GetOption<string>("originalNameProperty"); }
        set { SetOption("originalNameProperty", value); }
    }

    /// <inheritdoc/>
    public bool DisplayFileName
    {
        get { return GetOption<bool>("displayFileName"); }
        set { SetOption("displayFileName", value); }
    }

    /// <inheritdoc/>
    public bool CopyToHistory { get; set; }

    /// <inheritdoc/>
    public string? FilenameFormat { get; set; }

    /// <inheritdoc/>
    public string? AllowedExtensions { get; set; }

    /// <inheritdoc/>
    public string? ImageExtensions { get; set; } = UploadOptions.DefaultImageExtensions;

    /// <inheritdoc/>
    public bool IgnoreExtensionMismatch { get; set; } = true;

    /// <inheritdoc/>
    public bool IgnoreEmptyImage { get; set; }

    /// <inheritdoc/>
    public bool IgnoreInvalidImage { get; set; }

    /// <summary>
    /// Whether the editor shows its toolbar with the select file button, as it did before, instead of a drop box while
    /// it has no file and buttons by its files. When not set, the editor shows it only when its toolbar has buttons of
    /// its own, see FileUploadEditor.defaults and MultipleFileUploadEditor.defaults on the client.
    /// </summary>
    public bool Toolbar
    {
        get { return GetOption<bool>("toolbar"); }
        set { SetOption("toolbar", value); }
    }

    /// <summary>
    /// The text of the drop box of the editor while it has no file, in place of "Drop a file here or click to choose",
    /// as a text or a local text key.
    /// </summary>
    public string? DropHint
    {
        get { return GetOption<string>("dropHint"); }
        set { SetOption("dropHint", value); }
    }

    /// <summary>
    /// A line under the text of the drop box, e.g. the kind or the size of the files, as a text or a local text key.
    /// </summary>
    public string? DropHintDetail
    {
        get { return GetOption<string>("dropHintDetail"); }
        set { SetOption("dropHintDetail", value); }
    }
}