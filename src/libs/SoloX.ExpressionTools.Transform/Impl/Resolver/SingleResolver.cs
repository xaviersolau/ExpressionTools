// ----------------------------------------------------------------------
// <copyright file="SingleResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl.Resolver
{
    /// <summary>
    /// Single expression resolver.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class SingleResolver<TIn, TOut> : IParameterResolver
    {
        /// <summary>
        /// Get the expression to in-line.
        /// </summary>
        public Expression<Func<TIn, TOut>> Expression { get; }

        /// <summary>
        /// Register an expression to be in-lined.
        /// </summary>
        /// <param name="expression">The expression to in-line.</param>
        public SingleResolver(Expression<Func<TIn, TOut>> expression)
        {
            this.Expression = expression;
        }

        /// <inheritdoc />
        public LambdaExpression Resolve(ParameterExpression parameter)
        {
            if (parameter != null && parameter.Type == typeof(TOut))
            {
                return Expression;
            }

            return null;
        }
    }

    /// <summary>
    /// Single expression resolver.
    /// </summary>
    public class SingleResolver : IParameterResolver
    {
        private readonly Type outType;

        /// <summary>
        /// Get the expression to in-line.
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// Register an expression to be in-lined.
        /// </summary>
        /// <param name="expression">The expression to in-line.</param>
        public SingleResolver(LambdaExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.Expression = expression;
            this.outType = expression.ReturnType;
        }

        /// <inheritdoc />
        public LambdaExpression Resolve(ParameterExpression parameter)
        {
            if (parameter != null && parameter.Type == this.outType)
            {
                return Expression;
            }

            return null;
        }
    }
}
