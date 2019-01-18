// ----------------------------------------------------------------------
// <copyright file="ExpressionInliner.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;
using SoloX.ExpressionTools.Transform.Impl.Visitor;

namespace SoloX.ExpressionTools.Transform.Impl
{
    /// <inheritdoc />
    public class ExpressionInliner : IExpressionInliner
    {
        private readonly InlinerVisitor inlinerVisitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionInliner"/> class with a given parameter resolver.
        /// </summary>
        /// <param name="parameterResolver">The parameter resolver that provides the expression to in-line replacing a given parameter.</param>
        public ExpressionInliner(IParameterResolver parameterResolver)
        {
            this.inlinerVisitor = new InlinerVisitor(parameterResolver);
        }

        /// <inheritdoc />
        public Expression<TOutputDelegate> Amend<TInputDelegate, TOutputDelegate>(Expression<TInputDelegate> expression)
        {
            return (Expression<TOutputDelegate>)this.inlinerVisitor.Visit(expression);
        }

        /// <inheritdoc />
        public LambdaExpression Amend(LambdaExpression expression)
        {
            return (LambdaExpression)this.inlinerVisitor.Visit(expression);
        }
    }
}
