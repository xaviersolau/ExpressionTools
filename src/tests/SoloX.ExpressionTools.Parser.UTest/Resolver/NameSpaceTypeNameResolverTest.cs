// ----------------------------------------------------------------------
// <copyright file="NameSpaceTypeNameResolverTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Parser.Impl.Resolver;
using System;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest.Resolver
{
    public class NameSpaceTypeNameResolverTest
    {
        [Theory(DisplayName = "It must resolve a type name")]
        [InlineData(typeof(DateTime), "System", nameof(DateTime), true)]
        [InlineData(typeof(DateTime), "System", "System.DateTime", true)]
        [InlineData(typeof(DateTime), "System", "NoMath", false)]
        public void ResolveTypeNameTest(
            Type type, string nameSpace, string lookupName, bool expectedMatch)
        {
            var resolver = new NameSpaceTypeNameResolver(new[] { nameSpace });

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
