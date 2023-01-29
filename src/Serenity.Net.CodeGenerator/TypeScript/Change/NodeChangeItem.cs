using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.Change;

public class NodeChangeItem
{
    public NodeChangeType ChangeType { get; set; }

    public INode Node { get; set; }
    //public int Pos { get; set; }

    //public int End { get; set; }
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