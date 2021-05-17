// ----------------------------------------------------------------------
// <copyright file="BasicInlinerExample.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using SoloX.ExpressionTools.Transform.Impl;
using SoloX.ExpressionTools.Transform.Impl.Resolver;

namespace SoloX.ExpressionTools.Examples
{
    /// <summary>
    /// This class shows how to use the ParameterInliner.
    /// </summary>
    public static class BasicInlinerExample
    {
        /// <summary>
        /// Let's in-line a lambda expression into one-another: a => a + 1 into b => b * 2 giving us a => (a + 1) * 2.
        /// </summary>
        public static void InlineALambdaExpression()
        {
            // Setup the expressions to use as input
            Expression<Func<int, int>> expressionToInline = a => a + 1;
            Expression<Func<int, int>> expressionToAmend = b => b * 2;

            // create the expression in-liner.
            var inliner = new MultiParameterInliner();

            // Setup the resolver telling that 'b' must be replaced by in-lined 'a => a + 1' lambda.
            var resolver = new ParameterResolver()
                .Register("b", expressionToInline);

            // Amend the given expression replacing parameter 'b' resulting in the lambda 'a => (a + 1) * 2'.
            var inlinedExpression = inliner.Amend<Func<int, int>, Func<int, int>>(resolver, expressionToAmend);
        }
    }
}
