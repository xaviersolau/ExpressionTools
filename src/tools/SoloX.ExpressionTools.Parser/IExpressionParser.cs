// <copyright file="IExpressionParser.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Parser
{
    /// <summary>
    /// Interface of the expression parser. Used to parse a C# lambda expression and to generate
    /// a System.Linq.Expressions tree.
    /// </summary>
    public interface IExpressionParser
    {
        /// <summary>
        /// Parse a textual C# lambda expression.
        /// </summary>
        /// <typeparam name="TDelegate">The expected resulting delegate.</typeparam>
        /// <param name="lambdaExpressionText">The lambda expression text input.</param>
        /// <returns>A typed lambda expression.</returns>
        Expression<TDelegate> Parse<TDelegate>(string lambdaExpressionText);

        /// <summary>
        /// Parse a textual C# lambda expression.
        /// </summary>
        /// <param name="lambdaExpressionText">The lambda expression text input.</param>
        /// <returns>An abstract lambda expression.</returns>
        LambdaExpression Parse(string lambdaExpressionText);
    }
}
