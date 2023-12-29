using Serenity.TypeScript;

namespace Serenity.TypeScript.Change;

internal class NodeChangeItem
{
    public NodeChangeType ChangeType { get; set; }

    public INode Node { get; set; }
    public string NewValue { get; set; }

    private string NewValueSmall => NewValue == null
        ? ""
        : NewValue.Length > 20
            ? NewValue[..18] + $"..({NewValue.Length})"
            : NewValue;

    public override string ToString()
    {
        if (ChangeType == NodeChangeType.Delete) return $"{ChangeType} {Node}.";
        return $"{ChangeType} {Node}. NewValue = \"{NewValueSmall}\"";
    }
}