// ----------------------------------------------------------------------
// <copyright file="DictionaryParameterTypeResolverTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SoloX.ExpressionTools.Parser.Impl.Resolver;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest.Resolver
{
    public class DictionaryParameterTypeResolverTest
    {
        [Fact(DisplayName = "It must resolve a parameter type depending of the dictionary given at setup")]
        public void ResolveParameterTypeTest()
        {
            var resolver = new DictionaryParameterTypeResolver(new Dictionary<string, Type>()
            {
                { "x", typeof(int) },
                { "y", typeof(double) },
            });

            Assert.Same(typeof(int), resolver.ResolveType("x"));
            Assert.Same(typeof(double), resolver.ResolveType("y"));
            Assert.Null(resolver.ResolveType("z"));
        }
    }
}
