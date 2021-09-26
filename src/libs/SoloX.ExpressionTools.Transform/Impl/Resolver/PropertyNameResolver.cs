// ----------------------------------------------------------------------
// <copyright file="PropertyNameResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl.Visitor;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl.Resolver
{
    /// <inheritdoc/>
    public class PropertyNameResolver : IPropertyNameResolver
    {
        /// <inheritdoc/>
        public string GetPropertyName<TElement, TResult>(Expression<Func<TElement, TResult>> expression)
        {
            var visitor = new PropertyNameResolverVisitor();
            visitor.Visit(expression);

            return visitor.PropertyName;
        }

        /// <inheritdoc/>
        public string GetPropertyName(LambdaExpression expression)
        {
            var visitor = new PropertyNameResolverVisitor();
            visitor.Visit(expression);

            return visitor.PropertyName;
        }
    }
}
