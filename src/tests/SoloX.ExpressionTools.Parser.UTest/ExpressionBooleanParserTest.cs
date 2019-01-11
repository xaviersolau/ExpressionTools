// ----------------------------------------------------------------------
// <copyright file="ExpressionBooleanParserTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ExpressionTools.Parser.UTest.Utils;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionBooleanParserTest
    {
        [Theory(DisplayName = "It must parse a basic binary operator boolean expression")]
        [InlineData("(a, b) => a || b", true, true, true)]
        [InlineData("(a, b) => a || b", true, false, true)]
        [InlineData("(a, b) => a || b", false, true, true)]
        [InlineData("(a, b) => a || b", false, false, false)]
        [InlineData("(a, b) => a && b", true, true, true)]
        [InlineData("(a, b) => a && b", true, false, false)]
        [InlineData("(a, b) => a && b", false, true, false)]
        [InlineData("(a, b) => a && b", false, false, false)]
        [InlineData("(a, b) => a ^ b", true, true, false)]
        [InlineData("(a, b) => a ^ b", true, false, true)]
        [InlineData("(a, b) => a ^ b", false, true, true)]
        [InlineData("(a, b) => a ^ b", false, false, false)]
        public void BasicBooleanBinaryOperationParseTest(string expression, bool operandA, bool operandB, bool expectedResult)
        {
            AssertEval(expression, operandA, operandB, expectedResult);
        }

        [Theory(DisplayName = "It must parse a basic unary operator boolean expression")]
        [InlineData("(a) => !a", true, false)]
        [InlineData("(a) => !a", false, true)]
        [InlineData("(a) => !!a", false, false)]
        public void BasicBooleanUnaryOperationParseTest(string expression, bool operandA, bool expectedResult)
        {
            AssertEval(expression, operandA, expectedResult);
        }

        private static void AssertEval(string expression, bool operandA, bool expectedResult)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<bool>();

            var lambda = expParser.Parse<Func<bool, bool>>(expression);

            Assert.NotNull(lambda);

            var func = lambda.Compile();

            var output = func(operandA);
            Assert.Equal(expectedResult, output);
        }

        private static void AssertEval(string expression, bool operandA, bool operandB, bool expectedResult)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<bool>();

            var lambda = expParser.Parse<Func<bool, bool, bool>>(expression);

            Assert.NotNull(lambda);

            var func = lambda.Compile();

            var output = func(operandA, operandB);
            Assert.Equal(expectedResult, output);
        }
    }
}
