// ----------------------------------------------------------------------
// <copyright file="PropertyNameResolverVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ExpressionTools.Transform.Impl.Visitor
{
    internal class PropertyNameResolverVisitor : ExpressionVisitor
    {
        private readonly StringBuilder name = new StringBuilder();
        private bool isSet;

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var exp = base.VisitMember(node);

            if (this.isSet)
            {
                this.name.Append('.');
            }
            else
            {
                this.isSet = true;
            }

            this.name.Append(node.Member.Name);

            return exp;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            throw new ArgumentException($"Unexpected use of the method {node.Method.Name} in the given expression.");
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            throw new ArgumentException($"Unexpected use of switch case in the given expression.");
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            throw new ArgumentException($"Unexpected use of index in the given expression.");
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            throw new ArgumentException($"Unexpected use of binary in the given expression.");
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            throw new ArgumentException($"Unexpected use of block in the given expression.");
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            throw new ArgumentException($"Unexpected use of conditional in the given expression.");
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            throw new ArgumentException($"Unexpected use of constant in the given expression.");
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            throw new ArgumentException($"Unexpected use of debug info in the given expression.");
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            throw new ArgumentException($"Unexpected use of default in the given expression.");
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new ArgumentException($"Unexpected use of dynamic in the given expression.");
        }

        protected override Expression VisitExtension(Expression node)
        {
            throw new ArgumentException($"Unexpected use of extension in the given expression.");
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            throw new ArgumentException($"Unexpected use of goto in the given expression.");
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            throw new ArgumentException($"Unexpected use of invocation in the given expression.");
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            throw new ArgumentException($"Unexpected use of label in the given expression.");
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            throw new ArgumentException($"Unexpected use of list init in the given expression.");
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            throw new ArgumentException($"Unexpected use of loop in the given expression.");
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            throw new ArgumentException($"Unexpected use of member init in the given expression.");
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new ArgumentException($"Unexpected use of new in the given expression.");
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            throw new ArgumentException($"Unexpected use of new array in the given expression.");
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            throw new ArgumentException($"Unexpected use of variable in the given expression.");
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            throw new ArgumentException($"Unexpected use of switch in the given expression.");
        }

        protected override Expression VisitTry(TryExpression node)
        {
            throw new ArgumentException($"Unexpected use of try in the given expression.");
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            throw new ArgumentException($"Unexpected use of type binary in the given expression.");
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            throw new ArgumentException($"Unexpected use of unary in the given expression.");
        }

        public string PropertyName => this.name.ToString();
    }
}
