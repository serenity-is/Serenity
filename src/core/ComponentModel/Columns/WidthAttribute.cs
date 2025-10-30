/// <summary>
/// Sets a fixed column width that cannot be resized (min = max = width).
/// </summary>
public class FixedWidthAttribute : WidthAttribute
{
    /// <summary>
    /// Creates a new fixed width attribute
    /// </summary>
    /// <param name="width"></param>
    public FixedWidthAttribute(int width) 
        : base(width)
    {
        Min = width;
        Max = width;
    }
}