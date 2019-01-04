// ----------------------------------------------------------------------
// <copyright file="IParameterInliner.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;

namespace SoloX.ExpressionTools
{
    /// <summary>
    /// Interface of the parameter in-liner. Used to replace expression parameter by other expression.
    /// </summary>
    public interface IParameterInliner
    {
        /// <summary>
        /// Replace parameters in the given expression in-lining parameter associated expressions.
        /// </summary>
        /// <typeparam name="TInputDelegate">Delegate type of the lambda expression used as input.</typeparam>
        /// <typeparam name="TOutputDelegate">Delegate type of the resulting lambda expression.</typeparam>
        /// <param name="expression">The expression in witch parameters must be in-lined.</param>
        /// <returns>The resulting in-lined typed lambda expression.</returns>
        Expression<TOutputDelegate> Inline<TInputDelegate, TOutputDelegate>(Expression<TInputDelegate> expression);

        /// <summary>
        /// Replace parameters in the given expression in-lining parameter associated expressions.
        /// </summary>
        /// <param name="expression">The expression in witch parameters must be in-lined.</param>
        /// <returns>The resulting in-lined abstract lambda expression.</returns>
        LambdaExpression Inline(LambdaExpression expression);
    }
}
