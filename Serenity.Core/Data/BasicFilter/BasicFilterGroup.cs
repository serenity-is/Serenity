using System.Collections.Generic;

namespace Serenity.Data
{
    public class BasicFilterGroup : BasicFilter
    {
        public BasicFilterGroup(LogicalOp op, params BasicFilter[] nodes)
        {
            Operator = op;
            Nodes = new List<BasicFilter>();
            Nodes.AddRange(nodes);
        }

        public BasicFilterGroup(LogicalOp op)
        {
            Operator = op;
            Nodes = new List<BasicFilter>();
        }

        public LogicalOp Operator { get; set; }
        public List<BasicFilter> Nodes { get; private set; }
    }
}
