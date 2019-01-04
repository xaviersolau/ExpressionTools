// ----------------------------------------------------------------------
// <copyright file="ExpressionFxParserTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Moq;
using SoloX.ExpressionTools.Parser.Impl;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionFxParserTest
    {
        [Theory(DisplayName = "It must parse a mathematical f(x) expression")]
        [InlineData("x => x + 1", 1d, 2d)]
        [InlineData("x => x - 1", 2d, 1d)]
        [InlineData("x => x * x", 2d, 4d)]
        [InlineData("x => x / x", 2d, 1d)]
        [InlineData("x => (x + 1) * 2", 1d, 4d)]
        [InlineData("x => x ^ 2", 3d, 9d)]
        [InlineData("x => 1 / x", 2d, 0.5d)]
        [InlineData("x => x % 2", 11d, 1d)]
        public void FunctionXYParseTest(string expression, double x, double y)
        {
            AssertEval(expression, x, y);
        }

        private static void AssertEval(string expression, double x, double y)
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(double));

            var methodResolverMock = new Mock<IMethodResolver>();

            var expParser = new ExpressionParser(typeResolverMock.Object, methodResolverMock.Object);

            var lambda = expParser.Parse(expression);

            Assert.NotNull(lambda);

            var func = (Func<double, double>)lambda.Compile();

            var output = func(x);
            Assert.Equal(y, output);
        }
    }
}
