// ----------------------------------------------------------------------
// <copyright file="SerializerVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ExpressionTools.Transform.Impl.Visitor
{
    internal class SerializerVisitor : ExpressionVisitor
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private readonly HashSet<string> defaultNamespaces = new HashSet<string>(new[] { "System", "System.Linq", "System.Globalization" });

        public string GetString()
        {
            var txt = this.stringBuilder.ToString();
            this.stringBuilder.Clear();
            return txt;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node) { return node; }
        protected override ElementInit VisitElementInit(ElementInit node) { return node; }
        protected override LabelTarget VisitLabelTarget(LabelTarget node) { return node; }
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node) { return node; }
        protected override MemberBinding VisitMemberBinding(MemberBinding node) { return node; }
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node) { return node; }
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node) { return node; }
        protected override SwitchCase VisitSwitchCase(SwitchCase node) { return node; }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                Visit(node.Left);
                this.stringBuilder.Append('[');
                Visit(node.Right);
                this.stringBuilder.Append(']');
            }
            else
            {
                this.stringBuilder.Append('(');
                Visit(node.Left);
                this.stringBuilder.Append(' ');
                AppendBinaryOperator(node.NodeType);
                this.stringBuilder.Append(' ');
                Visit(node.Right);
                this.stringBuilder.Append(')');
            }
            return node;
        }

        private void AppendBinaryOperator(ExpressionType nodeType)
        {
#pragma warning disable IDE0072 // Add missing cases
            this.stringBuilder.Append(nodeType switch
            {
                ExpressionType.Equal => "==",
                ExpressionType.NotEqual => "!=",
                ExpressionType.LessThan => "<",
                ExpressionType.GreaterThan => ">",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.MemberAccess => ".",
                ExpressionType.Add => "+",
                ExpressionType.Subtract => "-",
                ExpressionType.Divide => "/",
                ExpressionType.Multiply => "*",
                ExpressionType.Modulo => "%",
                ExpressionType.Or => "||",
                ExpressionType.OrElse => "||",
                ExpressionType.And => "&&",
                ExpressionType.AndAlso => "&&",
                _ => throw new NotSupportedException(),
            });
#pragma warning restore IDE0072 // Add missing cases
        }

        protected override Expression VisitBlock(BlockExpression node) { return node; }
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            this.stringBuilder.Append('(');
            Visit(node.Test);
            this.stringBuilder.Append(" ? ");
            Visit(node.IfTrue);
            this.stringBuilder.Append(" : ");
            Visit(node.IfFalse);
            this.stringBuilder.Append(')');

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                this.stringBuilder.Append("null");
            }
            else if (node.Type == typeof(string))
            {
                this.stringBuilder.Append('"');
                this.stringBuilder.Append(node.Value);
                this.stringBuilder.Append('"');
            }
            else if (node.Type == typeof(bool))
            {
                var value = (bool)node.Value;
                this.stringBuilder.Append(value ? "true" : "false");
            }
            else
            {
                this.stringBuilder.Append(node.Value);
            }
            return node;
        }
        protected override Expression VisitDebugInfo(DebugInfoExpression node) { return node; }
        protected override Expression VisitDefault(DefaultExpression node) { return node; }
        protected override Expression VisitDynamic(DynamicExpression node) { return node; }
        protected override Expression VisitExtension(Expression node) { return node; }
        protected override Expression VisitGoto(GotoExpression node) { return node; }
        protected override Expression VisitIndex(IndexExpression node) { return node; }
        protected override Expression VisitInvocation(InvocationExpression node) { return node; }
        protected override Expression VisitLabel(LabelExpression node) { return node; }
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Parameters.Count != 1)
            {
                this.stringBuilder.Append('(');
                var first = true;
                foreach (var parameter in node.Parameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.stringBuilder.Append(", ");
                    }

                    Visit(parameter);
                }
                this.stringBuilder.Append(')');
            }
            else
            {
                Visit(node.Parameters.First());
            }
            this.stringBuilder.Append(" => ");
            Visit(node.Body);
            return node;
        }
        protected override Expression VisitListInit(ListInitExpression node) { return node; }
        protected override Expression VisitLoop(LoopExpression node) { return node; }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null)
            {
                base.Visit(node.Expression);
            }
            else
            {
                this.stringBuilder.Append(SerializeTypeName(node.Member.DeclaringType));
            }

            this.stringBuilder.Append('.');
            this.stringBuilder.Append(node.Member.Name);

            return node;
        }
        protected override Expression VisitMemberInit(MemberInitExpression node) { return node; }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsStatic && node.Method.DeclaringType == typeof(Enumerable) && node.Arguments.Count > 0)
            {
                var thisArg = node.Arguments.First();
                base.Visit(thisArg);

                this.stringBuilder.Append('.');
                this.stringBuilder.Append(node.Method.Name);

                var genArgs = node.Method.GetGenericArguments();
                this.stringBuilder.Append('<');

                var firstArg = true;
                foreach (var arg in genArgs)
                {
                    if (firstArg)
                    {
                        firstArg = false;
                    }
                    else
                    {
                        this.stringBuilder.Append(", ");
                    }

                    this.stringBuilder.Append(SerializeTypeName(arg));
                }

                this.stringBuilder.Append('>');

                this.stringBuilder.Append('(');

                var first = true;
                foreach (var argument in node.Arguments.Skip(1))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.stringBuilder.Append(", ");
                    }

                    base.Visit(argument);
                }

                this.stringBuilder.Append(')');
            }
            else
            {
                if (node.Method.IsStatic)
                {
                    this.stringBuilder.Append(SerializeTypeName(node.Method.DeclaringType));
                    this.stringBuilder.Append('.');
                    this.stringBuilder.Append(node.Method.Name);
                }
                else
                {
                    base.Visit(node.Object);
                    this.stringBuilder.Append('.');
                    this.stringBuilder.Append(node.Method.Name);
                }

                if (node.Method.IsGenericMethod)
                {
                    var genArgs = node.Method.GetGenericArguments();
                    this.stringBuilder.Append('<');

                    var firstArg = true;
                    foreach (var arg in genArgs)
                    {
                        if (firstArg)
                        {
                            firstArg = false;
                        }
                        else
                        {
                            this.stringBuilder.Append(", ");
                        }

                        this.stringBuilder.Append(SerializeTypeName(arg));
                    }

                    this.stringBuilder.Append('>');
                }

                this.stringBuilder.Append('(');

                var first = true;
                foreach (var argument in node.Arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        this.stringBuilder.Append(", ");
                    }

                    base.Visit(argument);
                }

                this.stringBuilder.Append(')');
            }

            return node;
        }
        protected override Expression VisitNew(NewExpression node)
        {
            this.stringBuilder.Append("new ");

            this.stringBuilder.Append(SerializeTypeName(node.Type));

            this.stringBuilder.Append('(');

            var first = true;
            foreach (var argument in node.Arguments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    this.stringBuilder.Append(", ");
                }

                base.Visit(argument);
            }
            this.stringBuilder.Append(')');

            return node;
        }
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            this.stringBuilder.Append("new ");

            this.stringBuilder.Append(SerializeTypeName(node.Type));

            if (node.NodeType == ExpressionType.NewArrayBounds)
            {
                throw new NotSupportedException();
            }
            else if (node.NodeType == ExpressionType.NewArrayInit)
            {
                this.stringBuilder.Append(" {");

                var first = true;

                foreach (var exp in node.Expressions)
                {
                    if (first)
                    {
                        this.stringBuilder.Append(' ');
                        first = false;
                    }
                    else
                    {
                        this.stringBuilder.Append(", ");
                    }

                    this.Visit(exp);
                }

                this.stringBuilder.Append(" }");
            }
            else
            {
                throw new NotSupportedException();
            }

            return node;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            this.stringBuilder.Append(node.Name);
            return node;
        }
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node) { return node; }
        protected override Expression VisitSwitch(SwitchExpression node) { return node; }
        protected override Expression VisitTry(TryExpression node) { return node; }
        protected override Expression VisitTypeBinary(TypeBinaryExpression node) { return node; }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            this.stringBuilder.Append('(');

            AppendPreUnaryOperator(node);

            this.stringBuilder.Append('(');
            base.Visit(node.Operand);
            this.stringBuilder.Append(')');

            AppendPostUnaryOperator(node);

            this.stringBuilder.Append(')');

            return node;
        }

        private void AppendPreUnaryOperator(UnaryExpression node)
        {
#pragma warning disable IDE0072 // Add missing cases
            this.stringBuilder.Append(node.NodeType switch
            {
                ExpressionType.Negate => "-",
                ExpressionType.UnaryPlus => "+",
                ExpressionType.Not => "!",
                ExpressionType.Convert => $"({SerializeTypeName(node.Type)})",

                ExpressionType.PostDecrementAssign => string.Empty,
                ExpressionType.PostIncrementAssign => string.Empty,
                ExpressionType.ArrayLength => string.Empty,

                _ => throw new NotSupportedException(),
            });
#pragma warning restore IDE0072 // Add missing cases
        }

        private void AppendPostUnaryOperator(UnaryExpression node)
        {
#pragma warning disable IDE0072 // Add missing cases
            this.stringBuilder.Append(node.NodeType switch
            {
                ExpressionType.PostDecrementAssign => "--",
                ExpressionType.PostIncrementAssign => "++",
                ExpressionType.ArrayLength => ".Length",

                ExpressionType.Negate => string.Empty,
                ExpressionType.UnaryPlus => string.Empty,
                ExpressionType.Not => string.Empty,
                ExpressionType.Convert => string.Empty,

                _ => throw new NotSupportedException(),
            });
#pragma warning restore IDE0072 // Add missing cases
        }

        private string SerializeTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var txt = IsDefaultNamespace(type.Namespace) ? string.Empty : type.Namespace + '.';
                var genSufixIndex = type.Name.IndexOf('`');
                txt += genSufixIndex >= 0 ? type.Name.Substring(0, genSufixIndex) : type.Name;
                txt += '<';
                foreach (var argument in type.GetGenericArguments())
                {
                    txt += SerializeTypeName(argument);
                }
                txt += '>';

                return txt;
            }
            else
            {
                return IsDefaultNamespace(type.Namespace) ? type.Name : type.ToString();
            }
        }

        private bool IsDefaultNamespace(string ns)
        {
            return this.defaultNamespaces.Contains(ns);
        }
    }
}
