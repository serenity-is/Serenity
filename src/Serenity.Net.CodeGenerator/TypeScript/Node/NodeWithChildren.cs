
namespace Serenity.TypeScript;

internal class NodeWithChildren : NodeBase, IHasChildren
{
    IEnumerable<INode> IHasChildren.Children { get; set; }
}
