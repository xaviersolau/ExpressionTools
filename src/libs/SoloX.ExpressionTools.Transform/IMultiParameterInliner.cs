// ----------------------------------------------------------------------
// <copyright file="IMultiParameterInliner.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Interface of the multi-parameter in-liner. Used to replace expression parameters by other associated expressions.
    /// </summary>
    public interface IMultiParameterInliner
    {
        /// <summary>
        /// Replace parameters in the given expression in-lining associated expressions.
        /// </summary>
        /// <typeparam name="TInputDelegate">Delegate type of the lambda expression used as input.</typeparam>
        /// <typeparam name="TOutputDelegate">Delegate type of the resulting lambda expression.</typeparam>
        /// <param name="parameterResolver">Parameter resolver to use to get the expression to in-line for a given parameter.</param>
        /// <param name="expression">The expression in witch parameters must be replaced.</param>
        /// <returns>The resulting amended lambda expression.</returns>
        Expression<TOutputDelegate> Amend<TInputDelegate, TOutputDelegate>(IParameterResolver parameterResolver, Expression<TInputDelegate> expression);

        /// <summary>
        /// Replace parameters in the given expression in-lining associated expressions.
        /// </summary>
        /// <param name="parameterResolver">Parameter resolver to use to get the expression to in-line for a given parameter.</param>
        /// <param name="expression">The expression in witch parameters must be replaced.</param>
        /// <returns>The resulting amended abstract lambda expression.</returns>
        LambdaExpression Amend(IParameterResolver parameterResolver, LambdaExpression expression);
    }
}
