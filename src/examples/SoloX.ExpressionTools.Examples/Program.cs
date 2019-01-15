// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ExpressionTools.Examples
{
    /// <summary>
    /// Example program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main()
        {
            BasicParserExample.ParseASimpleLambda();
            BasicParserExample.ParseASimpleLambdaWithAParameterTypeResolver();
        }
    }
}
