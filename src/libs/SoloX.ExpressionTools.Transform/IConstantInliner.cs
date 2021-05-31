// ----------------------------------------------------------------------
// <copyright file="IConstantInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Constant expression in-liner to resolve external variable references as constant expressions.
    /// </summary>
    public interface IConstantInliner
    {
        /// <summary>
        /// Replace external variable use in the given expression by constant expression.
        /// </summary>
        /// <typeparam name="TDelegate">Delegate type of the lambda expression to convert.</typeparam>
        /// <param name="expression">The expression in witch variables must be replaced.</param>
        /// <returns>The resulting amended lambda expression.</returns>
        Expression<TDelegate> Amend<TDelegate>(Expression<TDelegate> expression);

        /// <summary>
        /// Replace external variable use in the given expression by constant expression.
        /// </summary>
        /// <param name="expression">The expression in witch parameters must be replaced.</param>
        /// <returns>The resulting amended abstract lambda expression.</returns>
        LambdaExpression Amend(LambdaExpression expression);
    }
}
