// ----------------------------------------------------------------------
// <copyright file="MultiParameterInlinerTest.cs" company="Xavier Solau">
// Copyright © 2019-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Shouldly;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Sample.Impl;
using SoloX.ExpressionTools.Transform.Impl;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest
{
    public class MultiParameterInlinerTest
    {
        [Fact(DisplayName = "It must in-line a basic constant expression")]
        public void BasicConstInlineTest()
        {
            var pi = new MultiParameterInliner();

            var parameterResolver = CreateParameterResolver<Func<double>>(() => 1);

            var resultingExp = pi.Amend<Func<double, double>, Func<double>>(parameterResolver, s => s + 1);
            resultingExp.ShouldNotBeNull();

            var func = resultingExp.Compile();
            func().ShouldBe(2);
        }

        [Fact(DisplayName = "It must in-line a basic one argument function expression")]
        public void BasicFunctionInlineTest()
        {
            var pi = new MultiParameterInliner();

            var parameterResolver = CreateParameterResolver<Func<double, double>>((a) => a * 3);

            var resultingExp = pi.Amend<Func<double, double>, Func<double, double>>(parameterResolver, s => s + 1);
            resultingExp.ShouldNotBeNull();

            var func = resultingExp.Compile();
            func(2).ShouldBe(7);
        }

        [Fact(DisplayName = "It must in-line multiple parameters expression")]
        public void MultiParametersInlineTest()
        {
            var pi = new MultiParameterInliner();

            var expMap = new Dictionary<string, Expression<Func<double, double>>>()
            {
                { "x", (a) => a * 3 },
                { "y", (a) => a * 5 },
            };

            var parameterResolver = CreateParameterResolver(expMap.ToDictionary(x => x.Key, x => (LambdaExpression)x.Value));

            var resultingExp = pi.Amend<Func<double, double, double>, Func<double, double, double>>(parameterResolver, (x, y) => x + y + 1);
            resultingExp.ShouldNotBeNull();

            var func = resultingExp.Compile();
            func(3, 2).ShouldBe(20);
        }

        [Fact(DisplayName = "It must in-line multiple parameters lambda expression")]
        public void MultiParametersInlineLambdaTest()
        {
            var pi = new MultiParameterInliner();

            var expMap = new Dictionary<string, Expression<Func<double, double>>>()
            {
                { "x", (a) => a * 3 },
                { "y", (a) => a * 5 },
            };

            var parameterResolver = CreateParameterResolver(expMap.ToDictionary(x => x.Key, x => (LambdaExpression)x.Value));

            Expression<Func<double, double, double>> expressionToAmend = (x, y) => x + y + 1;

            var resultingExp = pi.Amend(parameterResolver, expressionToAmend);
            resultingExp.ShouldNotBeNull();

            var func = (Func<double, double, double>)resultingExp.Compile();
            func(3, 2).ShouldBe(20);
        }

        [Fact(DisplayName = "It must in-line member access")]
        public void MemberAccessInlineTest()
        {
            var pi = new MultiParameterInliner();

            var parameterResolver = CreateParameterResolver<Func<IData1, IData2>>(s => s.Data2);

            var resultingExp = pi.Amend<Func<IData2, IData3>, Func<IData1, IData3>>(parameterResolver, s => s.Data3);
            resultingExp.ShouldNotBeNull();

            var input = new Data1()
            {
                Data2 = new Data2()
                {
                    Data3 = new Data3(),
                },
            };

            var func = resultingExp.Compile();
            func(input).ShouldBeSameAs(input.Data2.Data3);
        }

        [Fact(DisplayName = "It must in-line method argument")]
        public void MethodArgumentInlineTest()
        {
            var pi = new MultiParameterInliner();

            Expression<Func<int>> exp = () => 10;
            var expMap = new Dictionary<string, LambdaExpression>()
            {
                { "x", exp },
            };

            var parameterResolver = CreateParameterResolver(expMap);

            var resultingExp = pi.Amend<Func<IObjectWithMethod, int, int>, Func<IObjectWithMethod, int>>(parameterResolver, (o, x) => o.BasicMethod(x));
            resultingExp.ShouldNotBeNull();

            var func = resultingExp.Compile();

            var objWithMethod = new ObjectWithMethod();
            func(objWithMethod).ShouldBe(10);
        }

        [Fact(DisplayName = "It must not change expression if no argument to in-line")]
        public void NoArgumentToInlineTest()
        {
            var pi = new MultiParameterInliner();

            Expression<Func<int>> exp = () => 10;
            var expMap = new Dictionary<string, LambdaExpression>()
            {
            };

            var parameterResolver = CreateParameterResolver(expMap);

            var resultingExp = pi.Amend<Func<int, int>, Func<int, int>>(parameterResolver, (x) => x + 1);
            resultingExp.ShouldNotBeNull();

            var func = resultingExp.Compile();

            func(1).ShouldBe(2);
        }

        private static IParameterResolver CreateParameterResolver<TDelegate>(Expression<TDelegate> exp)
        {
            var parameterResolverMock = new Mock<IParameterResolver>();

            parameterResolverMock
                .Setup(r => r.Resolve(It.IsAny<ParameterExpression>()))
                .Returns(exp);

            return parameterResolverMock.Object;
        }

        private static IParameterResolver CreateParameterResolver(Dictionary<string, LambdaExpression> parameterMap)
        {
            var parameterResolverMock = new Mock<IParameterResolver>();

            parameterResolverMock
                .Setup(r => r.Resolve(It.IsAny<ParameterExpression>()))
                .Returns((ParameterExpression p) => parameterMap.TryGetValue(p.Name, out var exp) ? exp : null);

            return parameterResolverMock.Object;
        }
    }
}
