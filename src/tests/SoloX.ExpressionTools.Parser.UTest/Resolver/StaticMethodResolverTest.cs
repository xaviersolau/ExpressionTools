// ----------------------------------------------------------------------
// <copyright file="StaticMethodResolverTest.cs" company="SoloX Software">
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
    public class StaticMethodResolverTest
    {
        [Theory(DisplayName = "It must resolve a method name and a parameter type list")]
        [InlineData(typeof(Math), nameof(Math.Max), "Max", new[] { typeof(double), typeof(double) }, true, true)]
        [InlineData(typeof(Math), nameof(Math.Max), "max", new[] { typeof(double), typeof(double) }, true, true)]
        [InlineData(typeof(Math), nameof(Math.Max), "Max", new[] { typeof(double), typeof(double) }, false, true)]
        [InlineData(typeof(Math), nameof(Math.Max), "max", new[] { typeof(double), typeof(double) }, false, false)]
        public void ResolveMethodNameTest(
            Type type, string methodName, string lookupName, Type[] argumentsType, bool ignoreCase, bool expectedMatch)
        {
            var resolver = new StaticMethodResolver(ignoreCase, type);

            var methodInfo = resolver.ResolveMethod(lookupName, argumentsType);
            if (expectedMatch)
            {
                Assert.Same(type.GetMethod(methodName, argumentsType), methodInfo);
            }
            else
            {
                Assert.Null(methodInfo);
            }
        }
    }
}
