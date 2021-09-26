// ----------------------------------------------------------------------
// <copyright file="PropertyNameResolverTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Transform.Impl.Resolver;
using System;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest.Resolver
{
    public class PropertyNameResolverTest
    {
        [Fact]
        public void ItShouldReturnTheGivenPropertyName()
        {
            var resolver = new PropertyNameResolver();

            var name = resolver.GetPropertyName<IData1, IData2>(x => x.Data2);

            name.Should()
                .NotBeNull()
                .And.Be(nameof(IData1.Data2));
        }

        [Fact]
        public void ItShouldReturnTheGivenPropertyNameRecusivly()
        {
            var resolver = new PropertyNameResolver();

            var name = resolver.GetPropertyName<IData1, IData3>(x => x.Data2.Data3);

            name.Should()
                .NotBeNull()
                .And.Be($"{nameof(IData1.Data2)}.{nameof(IData2.Data3)}");
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithMethodCall()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, string>(x => x.ToString()));
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithArrayAccess()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1[], IData2>(x => x[0].Data2));
        }
    }
}
