// ----------------------------------------------------------------------
// <copyright file="ExpressionErrorParserTest.cs" company="SoloX Software">
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

            Assert.Throws<FormatException>(() => expParser.Parse("s => s; var tmp = y => y"));
        }
    }
}
