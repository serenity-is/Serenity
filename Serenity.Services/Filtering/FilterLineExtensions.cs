using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public static class FilterLineExtensions
    {
        public static BasicFilterBase ToBasicFilter(this IList<FilterLine> lines)
        {
            if (lines == null)
                throw new ArgumentNullException("lines");

            if (lines.Count == 0)
                return null;

            bool inParens = false;

            // http://en.wikipedia.org/wiki/Shunting_yard_algorithm
            // http://en.wikipedia.org/wiki/Reverse_Polish_Notation

            List<BasicFilter> filters = new List<BasicFilter>(lines.Count);
            List<int> rpnOutput = new List<int>(lines.Count * 2); // will contain negative or indexes of items
            Stack<int> rpnStack = new Stack<int>(lines.Count * 2);

            int intLeftParen = -1;
            int intAnd = -2;
            int intOr = -3;
            int index = 0;

            foreach (FilterLine line in lines)
            {
                if (inParens &&
                    (line.RightParen || line.LeftParen))
                {
                    while (true)
                    {
                        var token = rpnStack.Pop();
                        if (token == intLeftParen)
                            break;
                        rpnOutput.Add(token);
                    }
                    inParens = false;
                }

                if (index > 0)
                {
                    var token = line.OR ? intOr : intAnd;
                    while (true)
                    {
                        if (rpnStack.Count == 0)
                            break;
                        var topToken = rpnStack.Peek();
                        if (topToken < intLeftParen &&
                            token <= topToken)
                            rpnOutput.Add(rpnStack.Pop());
                        else
                            break;
                    }

                    rpnStack.Push(token);
                }

                if (line.LeftParen)
                    rpnStack.Push(intLeftParen);

                rpnOutput.Add(index);

                index++;
            }

            if (inParens)
            {
                while (true)
                {
                    var token = rpnStack.Pop();
                    if (token == intLeftParen)
                        break;
                    rpnOutput.Add(token);
                }
            }

            while (rpnStack.Count > 0)
            {
                var token = rpnStack.Pop();
                if (token == intLeftParen)
                    throw new InvalidOperationException("mismatched leftParen in RPN stack!");
                rpnOutput.Add(token);
            }

            var evaluationStack = new Stack<BasicFilterBase>();

            foreach (var input in rpnOutput)
            {
                if (input >= 0)
                {
                    BasicFilter item = new BasicFilter();
                    var line = lines[input];
                    item.Field = line.Field;
                    item.Operator = line.Op;
                    item.Value = line.Value;
                    item.Values = line.Values;

                    evaluationStack.Push(item);
                }
                else
                {
                    if (evaluationStack.Count < 2)
                        throw new InvalidOperationException("RPN evaluation stack has less than two items!");

                    LogicalOp op = input == intAnd ? LogicalOp.And : LogicalOp.Or;

                    var right = evaluationStack.Pop();
                    var left = evaluationStack.Pop();
                    BasicFilterBase result = left.Merge(op, right);
                    evaluationStack.Push(result);
                }
            }

            if (evaluationStack.Count > 1)
                throw new InvalidOperationException("RPN evaluation stack has more than one item!");

            return evaluationStack.Pop();
        }
    }
}
