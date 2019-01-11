// ----------------------------------------------------------------------
// <copyright file="ExpressionFxParserTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ExpressionTools.Parser.UTest.Utils;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionFxParserTest
    {
        [Theory(DisplayName = "It must parse a basic mathematical f(x) unary expression")]
        [InlineData("x => + x", 1d, 1d)]
        [InlineData("x => - x", 2d, -2d)]
        public void BasicUnaryFunctionXYParseTest(string expression, double x, double y)
        {
            AssertEval(expression, x, y);
        }

        [Theory(DisplayName = "It must parse a basic mathematical f(x) binary expression")]
        [InlineData("x => x + 1", 1d, 2d)]
        [InlineData("x => x - 1", 2d, 1d)]
        [InlineData("x => x * x", 2d, 4d)]
        [InlineData("x => x / x", 2d, 1d)]
        [InlineData("x => (x + 1) * 2", 1d, 4d)]
        [InlineData("x => 1 / x", 2d, 0.5d)]
        [InlineData("x => x % 2", 11d, 1d)]
        public void BasicBinaryFunctionXYParseTest(string expression, double x, double y)
        {
            AssertEval(expression, x, y);
        }

        [Theory(DisplayName = "It must parse a mathematical f(x) expression")]
        [InlineData("x => Math.Abs(x)", -2d, 2d)]
        [InlineData("x => Abs(x)", -2d, 2d)]
        [InlineData("x => Math.PI * x", 1d, Math.PI)]
        [InlineData("x => Math.Abs(Math.Min(x, -10d))", -20d, 20d)]
        [InlineData("x => Math.Min(x, 10)", 15d, 10d)]
        [InlineData("x => Math.Abs(x) + Math.Min(x, -10d)", -20d, 0d)]
        public void FunctionXYParseTest(string expression, double x, double y)
        {
            AssertEval(expression, x, y);
        }

        private static void AssertEval(string expression, double x, double y)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<double>(
                methodFunc: (name, args) =>
                {
                    return typeof(Math).GetMethod(name, args);
                },
                typeNameFunc: (typeName) =>
                {
                    if (typeName == nameof(Math))
                    {
                        return typeof(Math);
                    }

                    return null;
                });

            var lambda = expParser.Parse(expression);

            Assert.NotNull(lambda);

            var func = (Func<double, double>)lambda.Compile();

            var output = func(x);
            Assert.Equal(y, output);
        }
    }
}
