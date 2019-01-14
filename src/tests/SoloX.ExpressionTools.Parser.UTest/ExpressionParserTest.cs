// ----------------------------------------------------------------------
// <copyright file="ExpressionParserTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using SoloX.ExpressionTools.Parser.UTest.Utils;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Sample.Impl;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionParserTest
    {
        public static IData2 GetData2FromData1(IData1 d)
        {
            return d.Data2;
        }

        [Fact(DisplayName = "It must parse a simple identity expression")]
        public void BasicIdentityParseTest()
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<object>();

            var lambda = expParser.Parse<Func<object, object>>("s => s");

            Assert.NotNull(lambda);

            var func = lambda.Compile();

            var input = new object();
            var output = func(input);

            Assert.Same(input, output);
        }

        [Fact(DisplayName = "It must parse a member access expression")]
        public void MemberAccessParseTest()
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<IData1>();

            var lambda = expParser.Parse<Func<IData1, IData2>>("s => s.Data2");

            Assert.NotNull(lambda);

            AssertItReturnData2PropertyValue(lambda);
        }

        [Fact(DisplayName = "It must parse a static method call expression without class name prefix")]
        public void StaticMethodWithoutPrefixParseTest()
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<IData1>(
                (string name, Type[] argsType) =>
                {
                    return this.GetType().GetMethod(name, argsType);
                });

            var lambda = expParser.Parse<Func<IData1, IData2>>("s => GetData2FromData1(s)");

            Assert.NotNull(lambda);

            AssertItReturnData2PropertyValue(lambda);
        }

        [Theory(DisplayName = "It must parse a static method call expression with class name prefix")]
        [InlineData("s => ExpressionParserTest.GetData2FromData1(s)")]
        [InlineData("s => SoloX.ExpressionTools.Parser.UTest.ExpressionParserTest.GetData2FromData1(s)")]
        public void StaticMethodWithPrefixParseTest(string expression)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<IData1>(
                typeNameFunc: (string typeName) =>
                {
                    if (typeName == nameof(ExpressionParserTest) || typeName == typeof(ExpressionParserTest).FullName)
                    {
                        return typeof(ExpressionParserTest);
                    }

                    return null;
                });

            var lambda = expParser.Parse<Func<IData1, IData2>>(expression);

            Assert.NotNull(lambda);

            AssertItReturnData2PropertyValue(lambda);
        }

        [Theory(DisplayName = "It must parse a method call expression")]
        [InlineData("o => o.BasicMethod(10)", 10)]
        [InlineData("o => o.TestMethod1(1).TestMethod1(2).BasicMethod(10)", 10)]
        [InlineData("o => o.TestMethod2(1, 2).TestMethod1(2).TestMethod2(2, 1).BasicMethod(10)", 10)]
        [InlineData("o => o.TestMethod2(1, 2).TestMethod1(o.BasicMethod(10)).TestMethod2(2, 1).BasicMethod(10)", 10)]
        [InlineData("o => o.TestMethod2(1, 2).TestMethod1(o.Self.BasicMethod(10)).Self.TestMethod2(2, 1).BasicMethod(10)", 10)]
        public void MethodParseTest(string expression, int expectedRes)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<IObjectWithMethod>();

            var lambda = expParser.Parse<Func<IObjectWithMethod, int>>(expression);
            Assert.NotNull(lambda);

            var func = lambda.Compile();

            var input = new ObjectWithMethod();
            var output = func(input);
            Assert.Equal(expectedRes, output);
        }

        [Theory(DisplayName = "It must parse a conditional expression")]
        [InlineData("x => x >= 10 ? 1: 0", 10, 1)]
        [InlineData("x => x >= 10 ? 1: 0", 15, 1)]
        [InlineData("x => x >= 10 ? 1: 0", 5, 0)]
        public void ConditionalExpressionParseTest(string conditionalExpression, int input, int expected)
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<int>();

            var lambda = expParser.Parse<Func<int, int>>(conditionalExpression);

            Assert.NotNull(lambda);
            var func = lambda.Compile();

            Assert.Equal(expected, func(input));
        }

        private static void AssertItReturnData2PropertyValue(Expression<Func<IData1, IData2>> lambda)
        {
            var func = lambda.Compile();

            var input = new Data1()
            {
                Data2 = new Data2(),
            };

            var output = func(input);
            Assert.Same(input.Data2, output);
        }
    }
}
