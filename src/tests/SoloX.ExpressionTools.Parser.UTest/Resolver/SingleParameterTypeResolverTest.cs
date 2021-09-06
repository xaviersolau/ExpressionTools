// ----------------------------------------------------------------------
// <copyright file="SingleParameterTypeResolverTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using SoloX.ExpressionTools.Parser.Impl.Resolver;
using System;
using Xunit;

namespace SoloX.ExpressionTools.Parser.UTest.Resolver
{
    public class SingleParameterTypeResolverTest
    {
        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(DateTime))]
        public void ItShouldResolveTheParameterName(Type typeToResolve)
        {
            var resolver = new SingleParameterTypeResolver(typeToResolve);

            var resolvedType = resolver.ResolveType("any");

            resolvedType.Should().Be(typeToResolve);
        }

        [Theory]
        [InlineData("a", "a", true)]
        [InlineData("a", "b", false)]
        public void ItShouldResolveTheGivenParameterName(string name, string nameToResolve, bool shouldResolve)
        {
            var resolver = new SingleParameterTypeResolver(typeof(string), name);


            if (shouldResolve)
            {
                var resolvedType = resolver.ResolveType(nameToResolve);
                resolvedType.Should().Be(typeof(string));
            }
            else
            {
                Assert.Throws<NotSupportedException>(() =>
                {
                    resolver.ResolveType(nameToResolve);
                });
            }
        }
    }
}
