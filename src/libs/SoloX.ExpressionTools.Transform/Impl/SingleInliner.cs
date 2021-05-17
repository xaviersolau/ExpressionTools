// ----------------------------------------------------------------------
// <copyright file="SingleInliner.cs" company="Xavier Solau">
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
    public class SingleInliner : ISingleInliner
    {
        /// <inheritdoc />
        public Expression<Func<TIn, TOut>> Inline<TIn, TInter, TOut>(
            Expression<Func<TIn, TInter>> expression1,
            Expression<Func<TInter, TOut>> expression2)
        {
            var parameterResolver = new SingleResolver<TIn, TInter>(expression1);

            var pi = new ExpressionInliner(parameterResolver);

            return pi.Amend<Func<TInter, TOut>, Func<TIn, TOut>>(expression2);
        }
    }
}
