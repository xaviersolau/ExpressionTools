// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ExpressionTools.Examples
{
    /// <summary>
    /// Example program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main()
        {
            BasicParserExample.ParseASimpleLambda();
            BasicParserExample.ParseASimpleLambdaWithAParameterTypeResolver();
            BasicParserExample.ParseASimpleLambdaWithAMethodResolver();
            BasicParserExample.ParseASimpleLambdaWithATypeNameResolver();
            BasicParserExample.ParseASimpleLambdaWithACustomTypeNameResolver1();
            BasicParserExample.ParseASimpleLambdaWithACustomTypeNameResolver2();

            BasicInlinerExample.InlineALambdaExpression();
        }
    }
}
