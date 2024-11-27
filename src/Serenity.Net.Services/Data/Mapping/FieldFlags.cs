namespace Serenity.Data;


/// <summary>Flags that determine basic properties of a field</summary>
[Flags]
public enum FieldFlags
{
    /// <summary>No flags set.</summary>
    None = 0,
    /// <summary>Internal fields are equal to no flags set.</summary>
    Internal = 0,
    /// <summary>Can a value be set on INSERT? Server side calculated 
    /// fields (like identity) shouldn't have this flag.</summary>
    Insertable = 1,
    /// <summary>Can it be set on UPDATE? Server side calculated 
    /// fields (like identity) shouldn't have this flag.</summary>
    Updatable = 2,
    /// <summary>Can it have a null or empty value?</summary>
    NotNull = 4,
    /// <summary>Field is a member of primary key.</summary>
    PrimaryKey = 8,
    /// <summary>Auto incrementing field.</summary>
    AutoIncrement = 16,
    /// <summary>It is a field originating from another table 
    /// through a join. e.g. view field.</summary> 
    Foreign = 32,
    /// <summary>Calculated field.</summary>
    Calculated = 64,
    /// <summary>Just reflects another field value (e.g. negative/absolute 
    /// version of it), so doesn't have client and server side storage of 
    /// its own, and setting it just sets another field.</summary>
    Reflective = 128,
    /// <summary>
    /// Field which is just a container to use in client side code (might 
    /// also be client side calculated / reflective).</summary>
    [Obsolete("Prefer NotMapped flag")]
    ClientSide = 256,
    /// <summary>
    /// Property which is not mapped to a SQL field / expression.</summary>
    NotMapped = 256,
    /// <summary>Should be trimmed (empty string as null) before 
    /// setting its value.</summary>
    Trim = 512,
    /// <summary>Should be trimmed to (null to empty string) before 
    /// setting its value.</summary>
    TrimToEmpty = 512 + 1024,
    /// <summary>Deny filtering on this field.</summary>
    DenyFiltering = 2048,
    /// <summary>Values should be unique.</summary>
    Unique = 4096,
    /// <summary>These are default flags unless specified otherwise. 
    /// Insertable, updatable and nullable and trimmed (to null).</summary>
    Default = Insertable | Updatable | Trim,
    /// <summary>Default flags with NotNull included.</summary>
    Required = Default | NotNull,
    /// <summary>An identity primary key field with auto incrementing value.</summary>
    Identity = PrimaryKey | AutoIncrement | NotNull
}