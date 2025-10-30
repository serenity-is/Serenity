
namespace Serenity.ComponentModel;

/// <summary>
/// Pins column to the start (left for LTR, right for RTL) or end (right for LTR, left for RTL) side of the grid.
/// Note that pinning a column requires FrozenLayout or EnhancedLayout.
/// Only EnhancedLayout supports pinning to end side.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PinColumnAttribute : Attribute
{
    /// <summary>
    /// Creates a new PinColumnAttribute for specified side: "start" (or "left") or "end" (or "right").
    /// </summary>
    public PinColumnAttribute(string side)
    {
        Value = side == "start" || side == "left" ? "start" : 
            side == "end" || side == "right" ? "end" :
            throw new ArgumentOutOfRangeException("side", "PinColumnAttribute side must be either 'start' (left) or 'end' (right).");
    }

    /// <summary>
    /// Creates a new PinColumnAttribute. If pin is true, pins to start side, otherwise unpins.
    /// Use PinColumnAttribute("end") to pin to end side.
    /// </summary>
    public PinColumnAttribute(bool pin = true)
    {
        Value = pin ? "start" : null;
    }

    /// <summary>
    /// The pin value: "start" (left), "end" (right) or null (not pinned).
    /// </summary>
    public string? Value { get; private set; }
}