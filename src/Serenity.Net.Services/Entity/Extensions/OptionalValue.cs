namespace Serenity;

/// <summary>
/// An optional value returned by some methods like interceptors.
/// It can be used to indicate that the result is optional and
/// if default is returned, the operation should continue normally.
/// </summary>
/// <typeparam name="T">Value type</typeparam>
/// <param name="value">Value</param>
public readonly struct OptionalValue<T>(T value)
{
    private readonly bool hasValue = true;

    /// <summary>
    /// True if the object has a meaningful value.
    /// </summary>
    public bool HasValue
    {
        get { return hasValue; }
    }

    /// <summary>
    /// The meaningful value of the object.
    /// </summary>
    public T Value
    {
        get { return value; }
    }

    /// <summary>
    /// Creates a new object initialized to a meaningful value. 
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator OptionalValue<T>(T value)
    {
        return new OptionalValue<T>(value);
    }
}