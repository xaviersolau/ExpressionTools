// ----------------------------------------------------------------------
// <copyright file="MultiParameterInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;
using SoloX.ExpressionTools.Transform.Impl.Visitor;

namespace SoloX.ExpressionTools.Transform.Impl
{
    /// <inheritdoc />
    public class MultiParameterInliner : IMultiParameterInliner
    {
        /// <inheritdoc />
        public Expression<TOutputDelegate> Amend<TInputDelegate, TOutputDelegate>(IParameterResolver parameterResolver, Expression<TInputDelegate> expression)
        {
            var inlinerVisitor = new InlinerVisitor(parameterResolver);
            return (Expression<TOutputDelegate>)inlinerVisitor.Visit(expression);
        }

        /// <inheritdoc />
        public LambdaExpression Amend(IParameterResolver parameterResolver, LambdaExpression expression)
        {
            var inlinerVisitor = new InlinerVisitor(parameterResolver);
            return (LambdaExpression)inlinerVisitor.Visit(expression);
        }
    }
}
