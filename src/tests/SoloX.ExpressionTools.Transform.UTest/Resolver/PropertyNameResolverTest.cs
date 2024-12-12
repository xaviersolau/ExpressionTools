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
using System.Linq.Expressions;
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
        public void ItShouldReturnTheGivenMethodNameWithFunc()
        {
            var resolver = new PropertyNameResolver();

            var name = resolver.GetMethodName<IObjectWithMethod, Func<int, IObjectWithMethod>>(x => x.TestMethod1);

            name.Should()
                .NotBeNull()
                .And.Be(nameof(IObjectWithMethod.TestMethod1));
        }

        [Fact]
        public void ItShouldReturnTheGivenMethodNameWithDelegate()
        {
            var resolver = new PropertyNameResolver();

            var name = resolver.GetMethodName<IObjectWithMethod, Delegate>(x => x.TestMethod1);

            name.Should()
                .NotBeNull()
                .And.Be(nameof(IObjectWithMethod.TestMethod1));
        }

        [Fact]
        public void ItShouldReturnTheGivenMethodNameWithLambda()
        {
            var resolver = new PropertyNameResolver();

            Expression<Func<IObjectWithMethod, Delegate>> lambdaExpression = x => x.TestMethod1;

            var name = resolver.GetMethodName((LambdaExpression)lambdaExpression);

            name.Should()
                .NotBeNull()
                .And.Be(nameof(IObjectWithMethod.TestMethod1));
        }

        [Fact]
        public void ItShouldReturnTheGivenMethodName()
        {
            var resolver = new PropertyNameResolver();

            var name = resolver.GetMethodName<IObjectWithMethod>(x => x.TestMethod1);

            name.Should()
                .NotBeNull()
                .And.Be(nameof(IObjectWithMethod.TestMethod1));
        }

        [Fact]
        public void ItShouldReturnTheGivenPropertyNameFromLambda()
        {
            var resolver = new PropertyNameResolver();

            Expression<Func<IData1, IData2>> lambdaExpression = (IData1 x) => x.Data2;

            var name = resolver.GetPropertyName((LambdaExpression)lambdaExpression);

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


        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithIdentity()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, IData1>(x => x));
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithConstant()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, int>(x => 10));
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithNew()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, object>(x => new object()));
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithNewArray()
        {
            var resolver = new PropertyNameResolver();

#pragma warning disable CA1861 // Avoid constant arrays as arguments
            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, int[]>(x => new int[] { 0 }));
#pragma warning restore CA1861 // Avoid constant arrays as arguments
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithBinary()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, int>(x => 10 + 2));
        }

        [Fact]
        public void ItShouldThrowAnArgumentExceptionWithUnary()
        {
            var resolver = new PropertyNameResolver();

            Assert.Throws<ArgumentException>(() => resolver.GetPropertyName<IData1, int>(x => -10));
        }
    }
}
