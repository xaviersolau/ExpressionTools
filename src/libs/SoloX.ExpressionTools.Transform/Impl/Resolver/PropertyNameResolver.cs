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
            var visitor = new PropertyOrMethodNameResolverVisitor(false);
            visitor.Visit(expression);

            var name = visitor.PropertyOrMethodName;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"unable to get the property name from the given expression.");
            }

            return visitor.PropertyOrMethodName;
        }

        /// <inheritdoc/>
        public string GetPropertyName(LambdaExpression expression)
        {
            var visitor = new PropertyOrMethodNameResolverVisitor(false);
            visitor.Visit(expression);

            return visitor.PropertyOrMethodName;
        }

        /// <inheritdoc/>
        public string GetMethodName<TElement, TDelegate>(Expression<Func<TElement, TDelegate>> expression)
        {
            var visitor = new PropertyOrMethodNameResolverVisitor(true);
            visitor.Visit(expression);

            var name = visitor.PropertyOrMethodName;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"unable to get the method name from the given expression.");
            }

            return visitor.PropertyOrMethodName;
        }

        /// <inheritdoc/>
        public string GetMethodName<TElement>(Expression<Func<TElement, Delegate>> expression)
        {
            return GetMethodName<TElement, Delegate>(expression);
        }

        /// <inheritdoc/>
        public string GetMethodName(LambdaExpression expression)
        {
            var visitor = new PropertyOrMethodNameResolverVisitor(true);
            visitor.Visit(expression);

            var name = visitor.PropertyOrMethodName;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"unable to get the method name from the given expression.");
            }

            return visitor.PropertyOrMethodName;
        }
    }
}
