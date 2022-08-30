// ----------------------------------------------------------------------
// <copyright file="ExpressionSerializer.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl.Visitor;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl
{
    /// <summary>
    /// Expression serializer.
    /// </summary>
    public class ExpressionSerializer : IExpressionSerializer
    {
        /// <inheritdoc/>
        public string Serialize<TElement, TResult>(Expression<Func<TElement, TResult>> expression)
        {
            var serializerVisitor = new SerializerVisitor();
            serializerVisitor.Visit(expression);
            return serializerVisitor.GetString();
        }

        /// <inheritdoc/>
        public string Serialize(LambdaExpression expression)
        {
            var serializerVisitor = new SerializerVisitor();
            serializerVisitor.Visit(expression);
            return serializerVisitor.GetString();
        }
    }
}
