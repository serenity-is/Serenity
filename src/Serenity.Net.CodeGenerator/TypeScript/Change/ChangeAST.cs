using Serenity.TypeScript.TsTypes;

namespace Serenity.TypeScript.Change;

public class ChangeAST
{
    private readonly ICollection<NodeChangeItem> _nodeChangeItems;

    public ChangeAST(ICollection<NodeChangeItem> changeItems = null)
    {
        _nodeChangeItems = changeItems ?? new List<NodeChangeItem>();
    }

    public static string Change(string source, IEnumerable<NodeChangeItem> changeItems)
    {
        var changes = changeItems.OrderBy(v => v.Node.Pos).ThenBy(v2 => v2.ChangeType);
        var sb = new StringBuilder();
        var pos = 0;
        foreach (var ch in changes)
        {
            if (ch.Node.Pos == null) throw new NullReferenceException("Node.Pos");
            switch (ch.ChangeType)
            {
                case NodeChangeType.InsertBefore:
                    if (ch.Node.Pos > pos) sb.Append(source[pos..(int)ch.Node.Pos]);
                    sb.Append(ch.NewValue);
                    pos = (int) ch.Node.Pos;
                    break;
                case NodeChangeType.Change:
                    if (ch.Node.Pos > pos) sb.Append(source[pos..(int)ch.Node.Pos]);
                    sb.Append(ch.NewValue);
                    if (ch.Node.End != null) pos = (int) ch.Node.End;
                    else throw new NullReferenceException("Node.End");
                    break;
                case NodeChangeType.Delete:
                    if (ch.Node.Pos > pos) sb.Append(source[pos..(int)ch.Node.Pos]);
                    if (ch.Node.End != null) pos = (int) ch.Node.End + 1;
                    break;
                case NodeChangeType.InsertAfter:
                    if (ch.Node.End > pos) sb.Append(source[pos..(int)ch.Node.End]);
                    sb.Append(ch.NewValue);
                    if (ch.Node.End != null) pos = (int) ch.Node.End;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if (pos < source.Length) sb.Append(source[pos..]);
        var newSource = sb.ToString();

        return newSource;
    }

    public string GetChangedSource(string baseSource)
    {
        return Change(baseSource, _nodeChangeItems);
    }

    public void ChangeNode(INode node, string newValue)
    {
        if (_nodeChangeItems.Any(v => v.Node == node &&
                                      (v.ChangeType == NodeChangeType.Change ||
                                       v.ChangeType == NodeChangeType.Delete)))
            throw new Exception("ChangeItems already have this node. Delete first");
        if (_nodeChangeItems.Any(v => v.Node.Pos < node.Pos && v.Node.End > node.Pos))
            throw new Exception("ChangeItems already have node that contains this node. Delete first");

        if (newValue != node.GetTextWithComments())
        {
            var nodeCh = new NodeChangeItem {ChangeType = NodeChangeType.Change, Node = node, NewValue = newValue};
            _nodeChangeItems.Add(nodeCh);
        }
        else
        {
            throw new Exception("Same value");
        }
    }

    public void InsertBefore(INode node, string newValue)
    {
        if (node != null)
        {
            //if (_nodeChangeItems.Any(v => v.Node.Pos < node.Pos && v.Node.End > node.Pos))
            //    throw new Exception("ChangeItems already have node that contains this node. Delete first");

            var nodeCh = new NodeChangeItem
            {
                ChangeType = NodeChangeType.InsertBefore,
                Node = node,
                NewValue = newValue
            };
            _nodeChangeItems.Add(nodeCh);
        }
    }

    public void InsertAfter(INode node, string newValue)
    {
        if (node != null)
        {
            //if (_nodeChangeItems.Any(v => v.Node.Pos < node.Pos && v.Node.End > node.Pos))
            //    throw new Exception("ChangeItems already have node that contains this node. Delete first");

            var nodeCh = new NodeChangeItem
            {
                ChangeType = NodeChangeType.InsertAfter,
                Node = node,
                NewValue = newValue
            };
            _nodeChangeItems.Add(nodeCh);
        }
    }

    public void Delete(INode node)
    {
        if (node != null)
        {
            if (_nodeChangeItems.Any(v => v.Node.Pos < node.Pos && v.Node.End > node.Pos))
                throw new Exception("ChangeItems already have node that contains this node. Delete first");

            var nodeCh = new NodeChangeItem {ChangeType = NodeChangeType.Delete, Node = node};
            _nodeChangeItems.Add(nodeCh);
        }
    }
}