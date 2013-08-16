using System;

namespace Serenity.Data
{
    public static class BasicFilterExtensions
    {
        public static BasicFilter Merge(this BasicFilter left, LogicalOp op, BasicFilter right)
        {
            var leftCriteria = left as BasicCriteria;
            var rightCriteria = right as BasicCriteria;
            var leftGroup = left as BasicFilterGroup;
            var rightGroup = right as BasicFilterGroup;

            if (left == null ||
                right == null)
            {
                return left ?? right;
            }
            else if (leftCriteria != null &&
                rightCriteria != null)
            {
                return new BasicFilterGroup(op, leftCriteria, rightCriteria);
            }
            else if (leftCriteria != null)
            {
                if (op == rightGroup.Operator)
                {
                    rightGroup.Nodes.Insert(0, leftCriteria);
                    return rightGroup;
                }
                else
                    return new BasicFilterGroup(op, leftCriteria, rightGroup);
            }
            else if (rightCriteria != null)
            {
                if (leftGroup.Operator == op)
                {
                    leftGroup.Nodes.Add(rightCriteria);
                    return leftGroup;
                }
                else
                    return new BasicFilterGroup(op, leftGroup, rightCriteria);
            }
            else
            {
                if (leftGroup.Operator == rightGroup.Operator &&
                    op == leftGroup.Operator)
                {
                    leftGroup.Nodes.AddRange(rightGroup.Nodes);
                    return leftGroup;
                }
                else if (leftGroup.Operator == op)
                {
                    leftGroup.Nodes.Add(rightGroup);
                    return leftGroup;
                }
                else if (rightGroup.Operator == op)
                {
                    rightGroup.Nodes.Insert(0, leftGroup);
                    return rightGroup;
                }
                else
                    return new BasicFilterGroup(op, leftGroup, rightGroup);
            }
        }

        public static BasicFilter Clone(this BasicFilter filter)
        {
            if (filter == null)
                return null;

            var criteria = filter as BasicCriteria;
            if (criteria != null)
                return Clone(criteria);

            return Clone((BasicFilterGroup)filter);
        }

        private static BasicFilter Clone(this BasicFilterGroup group)
        {
            var result = new BasicFilterGroup(group.Operator);
            foreach (var node in group.Nodes)
                result.Nodes.Add(Clone(node));
            return result;
        }

        private static BasicFilter Clone(this BasicCriteria criteria)
        {
            return new BasicCriteria
            {
                Field = criteria.Field,
                Operator = criteria.Operator,
                Value = criteria.Value,
                Value2 = criteria.Value2,
                Values = criteria.Values
            };
        }

        public static void ForEachCriteria(this BasicFilter filter,
            Action<BasicCriteria> handler)
        {
            if (filter == null)
                return;

            var c = filter as BasicCriteria;
            if (c != null)
                handler(c);
            else foreach (var g in ((BasicFilterGroup)filter).Nodes)
                ForEachCriteria(g, handler);
        }

        public static void ForEach(this BasicFilter filter,
            Action<BasicFilter> handler)
        {
            if (filter == null)
                return;

            var c = filter as BasicFilterGroup;
            if (c != null)
                foreach (var g in c.Nodes)
                    ForEach(g, handler);
            else
                handler(filter);
        }

        public static bool IsSame(this BasicFilter filter, BasicFilter other)
        {
            if (filter == null)
                return other == null;

            var g = filter as BasicFilterGroup;
            if (g != null)
            {
                var go = other as BasicFilterGroup;
                if (go == null)
                    return false;

                if (g.Nodes.Count != go.Nodes.Count)
                    return false;

                for (var i = 0; i < g.Nodes.Count; i++)
                    if (!IsSame(g.Nodes[i], go.Nodes[i]))
                        return false;
            }
            else
            {
                BasicCriteria c = (BasicCriteria)filter;
                BasicCriteria co = other as BasicCriteria;
                if (co == null)
                    return false;

                if (c.Field != co.Field)
                    return false;

                if (c.Operator != co.Operator)
                    return false;

                if (c.Value != co.Value)
                    return false;

                if (c.Value2 != co.Value2)
                    return false;

                if ((c.Values == null) != (co.Values == null))
                    return false;

                if (c.Values != null)
                {
                    if (c.Values.Length != co.Values.Length)
                        return false;

                    for (var i = 0; i < c.Values.Length; i++)
                    {
                        if (c.Values[i] != co.Values[i])
                            return false;
                    }
                }
            }

            return true;
        }
    }
}