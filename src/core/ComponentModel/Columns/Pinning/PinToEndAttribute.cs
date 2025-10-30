
namespace Serenity.ComponentModel;

/// <summary>
/// Pins column to the "end" (right for LTR, left for RTL) side of the grid.
/// Note that pinning a column to end side requires EnhancedLayout.
/// </summary>
public class PinToEndAttribute() : PinColumnAttribute("end")
{
}