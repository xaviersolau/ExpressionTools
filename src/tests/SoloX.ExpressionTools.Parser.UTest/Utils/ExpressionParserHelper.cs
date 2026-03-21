// ----------------------------------------------------------------------
// <copyright file="ExpressionParserHelper.cs" company="Xavier Solau">
// Copyright © 2019-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;
using NSubstitute;
using SoloX.ExpressionTools.Parser.Impl;

namespace SoloX.ExpressionTools.Parser.UTest.Utils
{
    public static class ExpressionParserHelper
    {
        public static IExpressionParser CreateExpressionParser<T>(
            Func<string, Type[], MethodInfo> methodFunc = null,
            Func<string, Type> typeNameFunc = null)
        {
            var typeResolver = CreateParameterTypeResolver<T>();

            var methodResolverMock = Substitute.For<IMethodResolver>();
            if (methodFunc != null)
            {
                methodResolverMock
                    .ResolveMethod(Arg.Any<string>(), Arg.Any<Type[]>())
                    .Returns(ci => methodFunc(ci.Arg<string>(), ci.Arg<Type[]>()));
            }

            var typeNameResolverMock = Substitute.For<ITypeNameResolver>();
            if (typeNameFunc != null)
            {
                typeNameResolverMock
                    .ResolveTypeName(Arg.Any<string>())
                    .Returns(ci => typeNameFunc(ci.Arg<string>()));
            }

            return new ExpressionParser(typeResolver, methodResolverMock, typeNameResolverMock);
        }

        private static IParameterTypeResolver CreateParameterTypeResolver<T>()
        {
            var typeResolverMock = Substitute.For<IParameterTypeResolver>();

            typeResolverMock
                .ResolveType(Arg.Any<string>())
                .Returns(typeof(T));

            return typeResolverMock;
        }
    }
}
