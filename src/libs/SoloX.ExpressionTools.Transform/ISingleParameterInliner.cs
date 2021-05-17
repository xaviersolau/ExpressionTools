// ----------------------------------------------------------------------
// <copyright file="ISingleParameterInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Single parameter expression in-liner.
    /// </summary>
    public interface ISingleParameterInliner
    {
        /// <summary>
        /// In-line expressionToInline in expressionToAmend parameter.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TInter">Intermediate parameter type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="expressionToInline">Expression to in-line.</param>
        /// <param name="expressionToAmend">Expression that will be transformed.</param>
        /// <returns>Expression2 with its parameter replaced by Expression1.</returns>
        Expression<Func<TIn, TOut>> Amend<TIn, TInter, TOut>(
            Expression<Func<TIn, TInter>> expressionToInline,
            Expression<Func<TInter, TOut>> expressionToAmend);

        /// <summary>
        /// In-line expressionToInline in expressionToAmend parameter.
        /// </summary>
        /// <param name="expressionToInline">Expression to in-line.</param>
        /// <param name="expressionToAmend">Expression that will be transformed.</param>
        /// <returns>Expression2 with its parameter replaced by Expression1.</returns>
        LambdaExpression Amend(LambdaExpression expressionToInline, LambdaExpression expressionToAmend);
    }
}
