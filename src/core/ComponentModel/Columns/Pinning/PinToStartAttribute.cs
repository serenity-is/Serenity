
namespace Serenity.ComponentModel;

/// <summary>
/// Pins column to the "start" (left for LTR, right for RTL) side of the grid.
/// Note that pinning a column requires FrozenLayout or EnhancedLayout.
/// </summary>
public class PinToStartAttribute() : PinColumnAttribute("start")
{
}