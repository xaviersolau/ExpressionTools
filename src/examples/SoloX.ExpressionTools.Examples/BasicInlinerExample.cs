// ----------------------------------------------------------------------
// <copyright file="BasicInlinerExample.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
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
            Expression<Func<int, int>> expression = b => b * 2;

            // Setup the resolver telling that 'b' must be replaced by in-lined 'a => a + 1' lambda.
            var resolver = new ParameterResolver()
                .Register("b", expressionToInline);

            // create the expression in-liner.
            var inliner = new ExpressionInliner(resolver);

            // Amend the given expression replacing parameter 'b' resulting in the lambda 'a => (a + 1) * 2'.
            var inlinedExpression = inliner.Amend<Func<int, int>, Func<int, int>>(expression);
        }
    }
}
