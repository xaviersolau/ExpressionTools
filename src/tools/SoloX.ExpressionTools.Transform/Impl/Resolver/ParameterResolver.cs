// ----------------------------------------------------------------------
// <copyright file="ParameterResolver.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl.Resolver
{
    /// <summary>
    /// IParameterResolver implementation that resolve a parameter expression with an expression to in-line.
    /// </summary>
    public class ParameterResolver : IParameterResolver
    {
        private readonly Dictionary<string, LambdaExpression> expressionMap = new Dictionary<string, LambdaExpression>();

        /// <summary>
        /// Register an expression to be in-line and substitute a parameter matching the given name.
        /// </summary>
        /// <typeparam name="TDelegate">Delegate type of the lambda expression.</typeparam>
        /// <param name="parameterName">The parameter name that will be replaced by the given lambda expression.</param>
        /// <param name="expression">The expression to in-line.</param>
        /// <returns>The current resolver.</returns>
        public ParameterResolver Register<TDelegate>(string parameterName, Expression<TDelegate> expression)
        {
            this.expressionMap.Add(parameterName, expression);
            return this;
        }

        /// <inheritdoc />
        public LambdaExpression Resolve(ParameterExpression parameter)
        {
            var name = parameter.Name;
            return this.expressionMap.TryGetValue(name, out var exp) ? exp : null;
        }
    }
}
