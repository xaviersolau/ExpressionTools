// ----------------------------------------------------------------------
// <copyright file="IExpressionSerializer.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Expression serializer.
    /// </summary>
    public interface IExpressionSerializer
    {
        /// <summary>
        /// Serialize the expression.
        /// </summary>
        /// <typeparam name="TElement">Type of the root element.</typeparam>
        /// <typeparam name="TResult">Type of the property.</typeparam>
        /// <param name="expression">The lambda expression to get the property name from.</param>
        /// <returns>The name of the property.</returns>
        /// <remarks>It will throw an exception if the lambda is unexpected.</remarks>
        string Serialize<TElement, TResult>(Expression<Func<TElement, TResult>> expression);

        /// <summary>
        /// Return the name of the property used in the lambda expression.
        /// </summary>
        /// <param name="expression">The lambda expression to get the property name from.</param>
        /// <returns>The name of the property.</returns>
        /// <remarks>It will throw an exception if the lambda is unexpected.</remarks>
        string Serialize(LambdaExpression expression);
    }
}
