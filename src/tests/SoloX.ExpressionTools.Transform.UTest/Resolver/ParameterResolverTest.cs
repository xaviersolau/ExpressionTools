// ----------------------------------------------------------------------
// <copyright file="ParameterResolverTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SoloX.ExpressionTools.Transform.Impl.Resolver;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest.Resolver
{
    public class ParameterResolverTest
    {
        [Fact(DisplayName = "It must resolve a given parameter to a lambda")]
        public void ResolverTest()
        {
            var resolver = new ParameterResolver();
            resolver
                .Register<Func<int, int>>("a", x => x + 1)
                .Register<Func<int, int>>("b", x => x * x);

            var parameter = Expression.Parameter(typeof(int), "a");

            var exp = resolver.Resolve(parameter);
            Assert.NotNull(exp);
            Assert.Equal(2, ((Func<int, int>)exp.Compile())(1));
        }
    }
}
