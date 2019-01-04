// ----------------------------------------------------------------------
// <copyright file="ParameterInliner.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;
using SoloX.ExpressionTools.Impl.Visitor;

namespace SoloX.ExpressionTools.Impl
{
    /// <inheritdoc />
    public class ParameterInliner : IParameterInliner
    {
        private InlinerVisitor inlinerVisitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInliner"/> class with a given parameter resolver.
        /// </summary>
        /// <param name="parameterResolver">The parameter resolver that provides the expression to in-line replacing a given parameter.</param>
        public ParameterInliner(IParameterResolver parameterResolver)
        {
            this.inlinerVisitor = new InlinerVisitor(parameterResolver);
        }

        /// <inheritdoc />
        public Expression<TOutputDelegate> Inline<TInputDelegate, TOutputDelegate>(Expression<TInputDelegate> expression)
        {
            return (Expression<TOutputDelegate>)this.inlinerVisitor.Visit(expression);
        }

        /// <inheritdoc />
        public LambdaExpression Inline(LambdaExpression expression)
        {
            return (LambdaExpression)this.inlinerVisitor.Visit(expression);
        }
    }
}
