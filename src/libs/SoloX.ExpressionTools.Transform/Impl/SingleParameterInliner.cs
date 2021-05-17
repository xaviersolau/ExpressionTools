// ----------------------------------------------------------------------
// <copyright file="SingleParameterInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl.Resolver;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl
{
    /// <inheritdoc />
    public class SingleParameterInliner : ISingleParameterInliner
    {
        /// <inheritdoc />
        public Expression<Func<TIn, TOut>> Amend<TIn, TInter, TOut>(
            Expression<Func<TIn, TInter>> expressionToInline,
            Expression<Func<TInter, TOut>> expressionToAmend)
        {
            var parameterResolver = new SingleResolver<TIn, TInter>(expressionToInline);

            var pi = new MultiParameterInliner();

            return pi.Amend<Func<TInter, TOut>, Func<TIn, TOut>>(parameterResolver, expressionToAmend);
        }

        /// <inheritdoc />
        public LambdaExpression Amend(LambdaExpression expressionToInline, LambdaExpression expressionToAmend)
        {
            var parameterResolver = new SingleResolver(expressionToInline);

            var pi = new MultiParameterInliner();

            return pi.Amend(parameterResolver, expressionToAmend);
        }
    }
}
