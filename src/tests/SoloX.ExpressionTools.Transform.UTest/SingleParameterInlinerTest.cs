// ----------------------------------------------------------------------
// <copyright file="SingleParameterInlinerTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl;
using System;
using System.Linq.Expressions;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest
{
    public class SingleParameterInlinerTest
    {
        [Fact(DisplayName = "It must in-line a single parameter expression")]
        public void SingleParameterInLinerTest()
        {
            var pi = new SingleParameterInliner();

            var resultingExp = pi.Amend<int, double, bool>((i) => i * 0.01d, (x) => x > 1d);

            Assert.NotNull(resultingExp);

            var func = resultingExp.Compile();

            Assert.True(func(1000));
            Assert.False(func(10));
        }

        [Fact(DisplayName = "It must in-line a single parameter lambda expression")]
        public void SingleParameterInLinerLambdaTest()
        {
            var pi = new SingleParameterInliner();

            Expression<Func<double, bool>> expToAmend = x => x > 1d;
            Expression<Func<int, double>> expToInline = i => i * 0.01d;

            var lambdaToAmend = (LambdaExpression)expToAmend;
            var lambdaToInline = (LambdaExpression)expToInline;

            var resultingExp = pi.Amend(lambdaToInline, lambdaToAmend);

            Assert.NotNull(resultingExp);

            var func = (Func<int, bool>)resultingExp.Compile();

            Assert.True(func(1000));
            Assert.False(func(10));
        }
    }
}
