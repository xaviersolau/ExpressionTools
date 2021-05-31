// ----------------------------------------------------------------------
// <copyright file="ConstantVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SoloX.ExpressionTools.Transform.Impl.Visitor
{
    /// <summary>
    /// Evaluate all constant node.
    /// </summary>
    internal class ConstantVisitor : ExpressionVisitor
    {
        /// <inheritdoc/>
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        /// <inheritdoc/>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Expression is ConstantExpression constNode && node.Member is FieldInfo fieldInfo)
            {
                var constNodeValue = constNode.Value;

                var value = fieldInfo.GetValue(constNodeValue);

                return Expression.Constant(value);
            }

            return base.VisitMember(node);
        }
    }
}
