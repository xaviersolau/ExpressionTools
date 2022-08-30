// ----------------------------------------------------------------------
// <copyright file="NameSpaceTypeNameResolverTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Parser.Impl.Resolver;
using System;
using System.Linq;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest.Resolver
{
    public class NameSpaceTypeNameResolverTest
    {
        [Theory(DisplayName = "It must resolve a type name")]
        [InlineData(typeof(DateTime), "System", null, nameof(DateTime), true)]
        [InlineData(typeof(DateTime), "System", null, "System.DateTime", true)]
        [InlineData(typeof(DateTime), "System", null, "NoMath", false)]
        [InlineData(typeof(Enumerable), "System.Linq", "System.Linq", "Enumerable", true)]
        [InlineData(typeof(Enumerable), "System.Linq", "System.Linq", "System.Linq.Enumerable", true)]
        [InlineData(typeof(Enumerable), "System", null, "Linq.Enumerable", false)]
        [InlineData(typeof(Enumerable), "System.Linq", "System.Linq", "Linq.Enumerable", true)]
        public void ResolveTypeNameTest(
            Type type, string nameSpace, string assembly, string lookupName, bool expectedMatch)
        {
            var resolver = new NameSpaceTypeNameResolver(new (string, string)[] { (nameSpace, assembly) });

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
