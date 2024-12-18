﻿// ----------------------------------------------------------------------
// <copyright file="BasicParserExample.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SoloX.ExpressionTools.Parser.Impl;
using SoloX.ExpressionTools.Parser.Impl.Resolver;

namespace SoloX.ExpressionTools.Examples
{
    /// <summary>
    /// This class shows how to use the ExpressionParser.
    /// </summary>
    internal static class BasicParserExample
    {
        /// <summary>
        /// Let's parse a self described lambda expression like "(int x) => x + 1".
        /// </summary>
        public static void ParseASimpleLambda()
        {
            // Set the expression to parse
            var expToParse = "(int x) => x + 1";

            // We need to create the parser.
            var expressionParser = new ExpressionParser();

            // We can just parse the expression.
            var lambdaExpression = expressionParser.Parse(expToParse);

            // Or we can parse the expression specifying the type of the lambda expression.
            var expression = expressionParser.Parse<Func<int, int>>(expToParse);
        }

        /// <summary>
        /// Let's parse a lambda expression like "x => x + 1" using a IParameterTypeResolver.
        /// </summary>
        public static void ParseASimpleLambdaWithAParameterTypeResolver()
        {
            // Set the expression to parse
            var expToParse = "x => x + 1";

            // We need to create the parser with a DictionaryParameterTypeResolver that will resolve the
            // parameter name using the given Dictionary.
            var expressionParser = new ExpressionParser(
                parameterTypeResolver: new DictionaryParameterTypeResolver(new Dictionary<string, Type>()
                {
                    { "x", typeof(int) },
                }));

            // We can just parse the expression.
            var expression = expressionParser.Parse<Func<int, int>>(expToParse);
        }

        /// <summary>
        /// Let's parse a lambda expression like "(double x, double y) => Max(x, y)" using a IMethodResolver.
        /// </summary>
        public static void ParseASimpleLambdaWithAMethodResolver()
        {
            // Set the expression to parse
            var expToParse = "(double x, double y) => Max(x, y)";

            // We need to create the parser with a StaticMethodResolver that will resolve methods with
            // the System.Math class.
            var expressionParser = new ExpressionParser(
                methodResolver: new StaticMethodResolver(typeof(Math)));

            // We can just parse the expression.
            var expression = expressionParser.Parse<Func<double, double, double>>(expToParse);
        }

        /// <summary>
        /// Let's parse a lambda expression like "(double x, double y) => Math.Max(x, y)" using a ITypeNameResolver.
        /// </summary>
        public static void ParseASimpleLambdaWithATypeNameResolver()
        {
            // Set the expression to parse
            var expToParse = "(double x, double y) => Math.Max(x, y)";

            // We need to create the parser with a TypeNameResolver that will resolve type name with
            // the given System.Math class.
            var expressionParser = new ExpressionParser(
                typeNameResolver: new BasicTypeNameResolver(typeof(Math)));

            // We can just parse the expression.
            var expression = expressionParser.Parse<Func<double, double, double>>(expToParse);
        }

        /// <summary>
        /// Let's parse a lambda expression with a custom type as parameter like "(IFoo x) => c.Value + 1".
        /// </summary>
        public static void ParseASimpleLambdaWithACustomTypeNameResolver1()
        {
            // Set the expression to parse
            var expToParse = "(IFoo x) => x.Value + 1";

            // We need to create the parser with a TypeNameResolver that will resolve the parameter type.
            var expressionParser = new ExpressionParser(
                typeNameResolver: new BasicTypeNameResolver(typeof(IFoo)));

            // We can just parse the expression.
            var expression = expressionParser.Parse<Func<IFoo, int>>(expToParse);
        }

        /// <summary>
        /// Let's parse a lambda expression with a custom type as parameter like "x => c.Value + 1" using a ITypeNameResolver.
        /// </summary>
        public static void ParseASimpleLambdaWithACustomTypeNameResolver2()
        {
            // Set the expression to parse
            var expToParse = "x => x.Value + 1";

            // We need to create the parser with a ParameterTypeResolver that will resolve the 'x' parameter as 'IFoo'.
            var expressionParser = new ExpressionParser(
                parameterTypeResolver: new SingleParameterTypeResolver(typeof(IFoo)));

            // We can just parse the expression.
            var expression = expressionParser.Parse<Func<IFoo, int>>(expToParse);
        }
    }

    /// <summary>
    /// IFoo interface to take as a custom type.
    /// </summary>
    internal interface IFoo
    {
        /// <summary>
        /// 
        /// </summary>
        int Value { get; set; }
    }
}
