using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Xunit;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Sample.Impl;
using System.Collections.Generic;
using System.Linq;
using SoloX.ExpressionTools.Parser.Impl;

namespace SoloX.ExpressionTools.Parser.UTest
{
    public class ExpressionErrorParserTest
    {
        [Fact(DisplayName = "It must parse a single member expression")]
        public void ProtectionMultiMembersParseTest()
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            var methodResolverMock = new Mock<IMethodResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(object));

            var expParser = new ExpressionParser(typeResolverMock.Object, methodResolverMock.Object);

            Assert.Throws<FormatException>(()=> expParser.Parse("s => s; var tmp = y => y"));
        }
    }
}
