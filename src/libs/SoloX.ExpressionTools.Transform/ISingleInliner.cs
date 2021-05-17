// ----------------------------------------------------------------------
// <copyright file="ISingleInliner.cs" company="Xavier Solau">
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
    public interface ISingleInliner
    {
        /// <summary>
        /// In-line expression1 in expression2 parameter.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TInter">Intermediate parameter type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="expression1">Expression to in-line.</param>
        /// <param name="expression2">Expression that will be transformed.</param>
        /// <returns>Expression2 with its parameter replaced by Expression1.</returns>
        Expression<Func<TIn, TOut>> Inline<TIn, TInter, TOut>(
            Expression<Func<TIn, TInter>> expression1,
            Expression<Func<TInter, TOut>> expression2);
    }
}
