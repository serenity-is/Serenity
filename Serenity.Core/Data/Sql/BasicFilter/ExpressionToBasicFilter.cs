using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Serenity.Data
{
    public class ExpressionToBasicFilter
    {
        public ExpressionToBasicFilter()
        {
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            IList<ParameterExpression> _parameters;

            public IList<ParameterExpression> GetParameters(Expression expr)
            {
                _parameters = new List<ParameterExpression>();
                Visit(expr);
                return _parameters;
            }

            protected override Expression VisitParameter(System.Linq.Expressions.ParameterExpression p)
            {
                if (!_parameters.Contains(p))
                    _parameters.Add(p);

                return base.VisitParameter(p);
            }
        }

        public BasicFilterBase Convert(Expression exp)
        {
            var parameters = new ParameterVisitor().GetParameters(exp);
            if (parameters.Count != 1)
                throw new InvalidOperationException("İfade tek bir parametre içermeli!");

            return Visit(exp);
        }

        protected BasicFilterBase Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        /*
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }*/

        protected virtual BasicFilterBase VisitUnary(UnaryExpression u)
        {
            /*
            Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;*/
            throw new NotImplementedException();
        }

        private static MemberInfo GetMember(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression == null)
                return null;

            if (!(memberExpression.Expression is ParameterExpression))
                return null;

            if (memberExpression.Member == null)
                return null;

            return memberExpression.Member;
        }

        private static MemberInfo IsStaticStringCompare(MethodCallExpression expression)
        {
            if (expression.Object == null &&
                expression.Method.DeclaringType == typeof(string) &&
                expression.Method.Name == "Compare" &&
                expression.Arguments != null &&
                expression.Arguments.Count == 2)
            {
                var member = GetMember(expression.Arguments[0]);
                if (member == null)
                    return null;

                var constant = expression.Arguments[0] as ConstantExpression;
                if (constant != null &&
                    constant.Value is Int32 &&
                    (Int32)constant.Value == 0)
                {
                    return member;
                }
            }
            
            return null;
        }

        protected virtual BasicFilterBase VisitBinary(BinaryExpression b)
        {
            if (b.Left == null || b.Right == null)
                throw new NotSupportedException("Left or Right is null!");
            if (b.Conversion != null)
                throw new NotSupportedException("Expression has conversion!");

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    BasicFilterBase leftNode = this.Visit(b.Left);
                    BasicFilterBase rightNode = this.Visit(b.Right);
                    LogicalOp logical;
                    switch (b.NodeType)
                    {
                        case ExpressionType.And:
                        case ExpressionType.AndAlso: logical = LogicalOp.And; break;
                        case ExpressionType.Or:
                        case ExpressionType.OrElse: logical = LogicalOp.Or; break;
                        default: throw new NotSupportedException("Unexpected Node Type Error!");
                    }
                    return leftNode.Merge(logical, rightNode);

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    var left = b.Left as MemberExpression;
                    if (left == null)
                    {
                        //bool isValid = false;
                        var methodCall = b.Left as MethodCallExpression;
                        if (methodCall != null)
                        {
                            var member = IsStaticStringCompare(methodCall);
                            if (member != null)
                            {
                            }
                        }
                            
                        throw new NotSupportedException("Left must be a MemberExpression!");
                    }
                    var right = b.Right as ConstantExpression;
                    if (right == null)
                        throw new NotSupportedException("Right must be a ConstantExpression!");

                    var op = FilterOp.EQ;
                    switch (b.NodeType)
                    {
                        case ExpressionType.LessThan: op = FilterOp.LT; break;
                        case ExpressionType.LessThanOrEqual: op = FilterOp.LE; break;
                        case ExpressionType.GreaterThan: op = FilterOp.GT; break;
                        case ExpressionType.GreaterThanOrEqual: op = FilterOp.GE; break;
                        case ExpressionType.Equal: op = FilterOp.EQ; break;
                        case ExpressionType.NotEqual: op = FilterOp.NE; break;
                        default: throw new NotSupportedException("Unexpected Node Type Error!");
                    }

                    return new BasicFilter()
                    {
                        Field = "Dummy",
                        Operator = op,
                        Value = "Dummy"
                    };

                default:
                    throw new NotSupportedException("Unsupported expression type: " + b.NodeType.ToString());
            }
        }

        protected virtual BasicFilterBase VisitTypeIs(TypeBinaryExpression b)
        {
            /*
            Expression expr = this.Visit(b.Expression);
            if (expr != b.Expression)
            {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitConstant(ConstantExpression c)
        {
            /*
            return c;
                */
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitConditional(ConditionalExpression c)
        {
            /*
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitParameter(ParameterExpression p)
        {
            /*
            return p;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitMemberAccess(MemberExpression m)
        {
            /*
            Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
                */
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitMethodCall(MethodCallExpression m)
        {
            /*
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            return m;*/
            throw new NotImplementedException();
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            /*List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;*/
            throw new NotImplementedException();
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            /*
            Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;*/
            throw new NotImplementedException();
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            /*IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;*/
            throw new NotImplementedException();
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            /*
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;*/
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            /*
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;*/
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            /*List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitLambda(LambdaExpression lambda)
        {
            /*Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitNew(NewExpression nex)
        {
            /*IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }
            return nex;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitMemberInit(MemberInitExpression init)
        {
            /*NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return init;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitListInit(ListInitExpression init)
        {
            /*NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return init;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitNewArray(NewArrayExpression na)
        {
            /*IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;*/
            throw new NotImplementedException();
        }

        protected virtual BasicFilterBase VisitInvocation(InvocationExpression iv)
        {
            /*IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return iv;*/
            throw new NotImplementedException();
        }
    }
}