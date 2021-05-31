// ----------------------------------------------------------------------
// <copyright file="ConstantInlinerTest.cs" company="Xavier Solau">
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
    public class ConstantInlinerTest
    {
        [Fact]
        public void IsShouldConvertExternalVariableAsConst()
        {
            var inliner = new ConstantInliner();

            var externalValue = 0.01d;

            Expression<Func<double, double>> expToInline = i => i * externalValue;

            var exp = inliner.Amend(expToInline);

            externalValue = 0.1d;

            var func = exp.Compile();

            Assert.Equal(100d, func(10000));

            Expression<Func<double, double>> expectedExp = i => i * 0.01d;
            Assert.Equal(expectedExp.ToString(), exp.ToString());
        }
    }
}
