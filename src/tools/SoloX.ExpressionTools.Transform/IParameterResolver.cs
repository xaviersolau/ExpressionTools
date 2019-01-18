// ----------------------------------------------------------------------
// <copyright file="IParameterResolver.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Interface used by the IExpressionInliner in order to resolve expression to in-line for a given parameter.
    /// </summary>
    public interface IParameterResolver
    {
        /// <summary>
        /// Resolve the expression to in-line for the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter to resolve.</param>
        /// <returns>The expression to in-line or null if the parameter must not be replaced.</returns>
        LambdaExpression Resolve(ParameterExpression parameter);
    }
}
