// ----------------------------------------------------------------------
// <copyright file="BasicTypeNameResolverTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ExpressionTools.Parser.Impl.Resolver;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest.Resolver
{
    public class BasicTypeNameResolverTest
    {
        [Theory(DisplayName = "It must resolve a type name")]
        [InlineData(typeof(Math), nameof(Math), true)]
        [InlineData(typeof(Math), "System.Math", true)]
        [InlineData(typeof(Math), "NoMath", false)]
        public void ResolveTypeNameTest(
            Type type, string lookupName, bool expectedMatch)
        {
            var resolver = new BasicTypeNameResolver(type);

            var resolvedType = resolver.ResolveTypeName(lookupName);
            if (expectedMatch)
            {
                Assert.Same(type, resolvedType);
            }
            else
            {
                Assert.Null(resolvedType);
            }
        }
    }
}
