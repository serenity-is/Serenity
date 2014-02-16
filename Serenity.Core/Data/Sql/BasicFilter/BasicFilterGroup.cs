using System.Collections.Generic;

namespace Serenity.Data
{
    public class BasicFilterGroup : BasicFilterBase
    {
        public BasicFilterGroup(LogicalOp op, params BasicFilterBase[] nodes)
        {
            Operator = op;
            Nodes = new List<BasicFilterBase>();
            Nodes.AddRange(nodes);
        }

        public BasicFilterGroup(LogicalOp op)
        {
            Operator = op;
            Nodes = new List<BasicFilterBase>();
        }

        public LogicalOp Operator { get; set; }
        public List<BasicFilterBase> Nodes { get; private set; }
    }
}
