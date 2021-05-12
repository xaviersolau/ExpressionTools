// ----------------------------------------------------------------------
// <copyright file="ExpressionErrorParserTest.cs" company="Xavier Solau">
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
    public class ExpressionErrorParserTest
    {
        [Fact(DisplayName = "It must parse a single member expression")]
        public void ProtectionMultiMembersParseTest()
        {
            var expParser = ExpressionParserHelper.CreateExpressionParser<object>();

            Assert.Throws<FormatException>(() => expParser.Parse("s => s; var tmp = y => y"));
        }
    }
}
