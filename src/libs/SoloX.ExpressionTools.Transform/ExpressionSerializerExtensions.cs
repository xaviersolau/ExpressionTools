// ----------------------------------------------------------------------
// <copyright file="ExpressionSerializerExtensions.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform
{
    /// <summary>
    /// Expression Serialization extensions.
    /// </summary>
    public static class ExpressionSerializerExtensions
    {
        /// <summary>
        /// Serialize a lambda expression as a string.
        /// </summary>
        /// <param name="expression">The expression to serialize.</param>
        /// <returns>The expression as a string.</returns>
        public static string Serialize<TElement, TResult>(this Expression<Func<TElement, TResult>> expression)
        {
            var serializer = new ExpressionSerializer();
            return serializer.Serialize(expression);
        }

        /// <summary>
        /// Serialize a lambda expression as a string.
        /// </summary>
        /// <param name="expression">The expression to serialize.</param>
        /// <returns>The expression as a string.</returns>
        public static string Serialize(this LambdaExpression expression)
        {
            var serializer = new ExpressionSerializer();
            return serializer.Serialize(expression);
        }
    }
}
