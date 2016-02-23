namespace Serenity
{
    using System;

    /// <summary>
    /// Makes dialog use Serenity flexify plugin. Flexify automatically
    /// resizes elements when dialog resize. The amount they grow or shrink
    /// are controlled by x/y flex factors.
    /// </summary>
    public class FlexifyAttribute : Attribute
    {
    }
}