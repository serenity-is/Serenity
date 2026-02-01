namespace Serenity.Web;

/// <summary>
/// Contains file metadata key constants
/// </summary>
public static class FileMetadataKeys
{
    /// <summary>
    /// A custom description of the file
    /// </summary>
    public const string Description = "Description";

    /// <summary>
    /// ID/Key of the associated entity
    /// </summary>
    public const string EntityId = "EntityId";

    /// <summary>
    /// Fieldname of the associated entity
    /// </summary>
    public const string EntityField = "EntityField";

    /// <summary>
    /// The property name of the associated entity
    /// </summary>
    public const string EntityProperty = "EntityProperty";

    /// <summary>
    /// Tablename of the associated entity
    /// </summary>
    public const string EntityTable = "EntityTable";

    /// <summary>
    /// Type of the associated entity
    /// </summary>
    public const string EntityType = "EntityType";

    /// <summary>
    /// Image size like 300x200
    /// </summary>
    public const string ImageSize = "ImageSize";

    /// <summary>
    /// Represents the key used to indicate whether an item is a thumbnail image.
    /// </summary>
    public const string IsThumbnail = "IsThumbnail";

    /// <summary>
    /// Original name of the file
    /// </summary>
    public const string OriginalName = "OriginalName";

    /// <summary>
    /// Generated default thumbnail size like 96x96.
    /// </summary>
    public const string ThumbSize = "ThumbSize";

    /// <summary>
    /// Represents the primary file's extension when the file is a derivative version,
    /// like a thumbnail or a converted format.
    /// </summary>
    public const string PrimaryFileExtension = "PrimaryFileExtension";
}