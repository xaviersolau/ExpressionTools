// ----------------------------------------------------------------------
// <copyright file="BasicParserExample.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public static class BasicParserExample
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
    }
}
