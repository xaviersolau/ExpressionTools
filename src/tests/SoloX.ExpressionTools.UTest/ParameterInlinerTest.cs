using Moq;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Sample.Impl;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace SoloX.ExpressionTools.UTest
{
    public class ParameterInlinerTest
    {
        [Fact(DisplayName = "It must in-line a basic constant expression")]
        public void BasicConstInlineTest()
        {
            var pi = CreateParameterInliner<Func<double>>(() => 1);

            var resultingExp = pi.Inline<Func<double, double>>(s => s + 1);

            Assert.NotNull(resultingExp);

            var func = (Func<double>)resultingExp.Compile();
            Assert.Equal(2, func());
        }

        [Fact(DisplayName = "It must in-line a basic one argument function expression")]
        public void BasicFunctionInlineTest()
        {
            var pi = CreateParameterInliner<Func<double, double>>((a) => a * 3);

            var resultingExp = pi.Inline<Func<double, double>>(s => s + 1);

            Assert.NotNull(resultingExp);

            var func = (Func<double, double>)resultingExp.Compile();
            Assert.Equal(7, func(2));
        }

        [Fact(DisplayName = "It must in-line multiple parameters")]
        public void MultiParametersInlineTest()
        {
            var expMap = new Dictionary<string, Expression<Func<double, double>>>()
            {
                { "x", (a) => a * 3 },
                { "y", (a) => a * 5 }
            };

            var parameterResolverMock = new Mock<IParameterResolver>();

            parameterResolverMock
                .Setup(r => r.Resolve(It.IsAny<ParameterExpression>()))
                .Returns((ParameterExpression p) => expMap[p.Name]);

            var pi = new ParameterInliner(parameterResolverMock.Object);

            var resultingExp = pi.Inline<Func<double, double, double>>((x, y) => x + y + 1);

            Assert.NotNull(resultingExp);

            var func = (Func<double, double, double>)resultingExp.Compile();
            Assert.Equal(20, func(3, 2));
        }

        [Fact(DisplayName = "It must in-line member access")]
        public void MemberAccessInlineTest()
        {
            Expression<Func<IData1, IData2>> exp = s => s.Data2;

            var parameterResolverMock = new Mock<IParameterResolver>();

            parameterResolverMock
                .Setup(r => r.Resolve(It.IsAny<ParameterExpression>()))
                .Returns(exp);

            var pi = new ParameterInliner(parameterResolverMock.Object);

            var resultingExp = pi.Inline<Func<IData2, IData3>>(s => s.Data3);

            var input = new Data1()
            {
                Data2 = new Data2()
                {
                    Data3 = new Data3()
                }
            };

            Assert.NotNull(resultingExp);

            var func = (Func<IData1, IData3>)resultingExp.Compile();
            Assert.Same(input.Data2.Data3, func(input));
        }

        private ParameterInliner CreateParameterInliner<TDelegate>(Expression<TDelegate> exp)
        {
            var parameterResolver = CreateParameterResolver<TDelegate>(exp);

            return new ParameterInliner(parameterResolver);
        }

        private IParameterResolver CreateParameterResolver<TDelegate>(Expression<TDelegate> exp)
        {
            var parameterResolverMock = new Mock<IParameterResolver>();

            parameterResolverMock
                .Setup(r => r.Resolve(It.IsAny<ParameterExpression>()))
                .Returns(exp);

            return parameterResolverMock.Object;
        }
    }
}
