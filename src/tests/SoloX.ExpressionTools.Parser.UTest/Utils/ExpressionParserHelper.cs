// ----------------------------------------------------------------------
// <copyright file="ExpressionParserHelper.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;
using Moq;
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

            var methodResolverMock = new Mock<IMethodResolver>();
            if (methodFunc != null)
            {
                methodResolverMock
                    .Setup(r => r.ResolveMethod(It.IsAny<string>(), It.IsAny<Type[]>()))
                    .Returns(methodFunc);
            }

            var typeNameResolverMock = new Mock<ITypeNameResolver>();
            if (typeNameFunc != null)
            {
                typeNameResolverMock
                    .Setup(r => r.ResolveTypeName(It.IsAny<string>()))
                    .Returns(typeNameFunc);
            }

            return new ExpressionParser(typeResolver, methodResolverMock.Object, typeNameResolverMock.Object);
        }

        private static IParameterTypeResolver CreateParameterTypeResolver<T>()
        {
            var typeResolverMock = new Mock<IParameterTypeResolver>();

            typeResolverMock
                .Setup(r => r.ResolveType(It.IsAny<string>()))
                .Returns(typeof(T));

            return typeResolverMock.Object;
        }
    }
}
