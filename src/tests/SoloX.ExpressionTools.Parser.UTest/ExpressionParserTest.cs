// ----------------------------------------------------------------------
// <copyright file="ExpressionParserTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Moq;
using SoloX.ExpressionTools.Parser.Impl;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Sample.Impl;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionParserTest
    {
        public static IData2 M(IData1 d)
        {
            return d.Data2;
        }

        [Fact(DisplayName = "It must parse a simple identity expression")]
        public void BasicIdentityParseTest()
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            var methodResolverMock = new Mock<IMethodResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(object));

            var expParser = new ExpressionParser(typeResolverMock.Object, methodResolverMock.Object);

            var lambda = expParser.Parse("s => s");

            Assert.NotNull(lambda);

            var func = (Func<object, object>)lambda.Compile();

            var input = new object();
            var output = func(input);

            Assert.Same(input, output);
        }

        [Fact(DisplayName = "It must parse a member access expression")]
        public void MemberAccessParseTest()
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            var methodResolverMock = new Mock<IMethodResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(IData1));

            var expParser = new ExpressionParser(typeResolverMock.Object, methodResolverMock.Object);

            var lambda = expParser.Parse("s => s.Data2");

            Assert.NotNull(lambda);

            var func = (Func<IData1, IData2>)lambda.Compile();

            var input = new Data1()
            {
                Data2 = new Data2(),
            };

            var output = func(input);
            Assert.Same(input.Data2, output);
        }

        [Fact(DisplayName = "It must parse a method call expression")]
        public void MethodParseTest()
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(IData1));

            var methodResolverMock = new Mock<IMethodResolver>();

            methodResolverMock
                .Setup(r => r.ResolveMethod(It.IsAny<string>(), It.IsAny<Type[]>()))
                .Returns((string name, Type[] argsType) =>
                {
                    return this.GetType().GetMethod(name, argsType);
                });

            var expParser = new ExpressionParser(typeResolverMock.Object, methodResolverMock.Object);

            var lambda = expParser.Parse("s => M(s)");

            Assert.NotNull(lambda);

            var func = (Func<IData1, IData2>)lambda.Compile();

            var input = new Data1()
            {
                Data2 = new Data2(),
            };

            var output = func(input);
            Assert.Same(input.Data2, output);
        }
    }
}
