// ----------------------------------------------------------------------
// <copyright file="ConstantInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl.Visitor;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl
{
    /// <inheritdoc/>
    public class ConstantInliner : IConstantInliner
    {
        /// <inheritdoc/>
        public Expression<TDelegate> Amend<TDelegate>(Expression<TDelegate> expression)
        {
            var inlinerVisitor = new ConstantVisitor();
            return (Expression<TDelegate>)inlinerVisitor.Visit(expression);
        }

        /// <inheritdoc/>
        public LambdaExpression Amend(LambdaExpression expression)
        {
            var inlinerVisitor = new ConstantVisitor();
            return (LambdaExpression)inlinerVisitor.Visit(expression);
        }
    }
}
