// ----------------------------------------------------------------------
// <copyright file="ExpressionRelationalParserTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ExpressionTools.Parser.UTest.Utils;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionRelationalParserTest
    {
        [Theory(DisplayName = "It must parse a basic relational operator expression")]
        [InlineData("(a, b) => a < b", 1, 2, true)]
        [InlineData("(a, b) => a > b", 1, 2, false)]
        [InlineData("(a, b) => a <= b", 1, 2, true)]
        [InlineData("(a, b) => a <= b", 2, 2, true)]
        [InlineData("(a, b) => a <= b", 3, 2, false)]
        [InlineData("(a, b) => a >= b", 1, 2, false)]
        [InlineData("(a, b) => a >= b", 2, 2, true)]
        [InlineData("(a, b) => a >= b", 3, 2, true)]
        [InlineData("(a, b) => a == b", 1, 2, false)]
        [InlineData("(a, b) => a == b", 2, 2, true)]
        [InlineData("(a, b) => a != b", 1, 2, true)]
        [InlineData("(a, b) => a != b", 2, 2, false)]
        public void BasicRelationalOperationParseTest(string expression, int operandA, int operandB, bool expectedResult)
        {
            AssertEval(expression, operandA, operandB, expectedResult);
        }

        private static void AssertEval(string expression, int operandA, int operandB, bool expectedResult)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<int>();

            var lambda = expParser.Parse<Func<int, int, bool>>(expression);

            Assert.NotNull(lambda);

            var func = lambda.Compile();

            var output = func(operandA, operandB);
            Assert.Equal(expectedResult, output);
        }
    }
}
