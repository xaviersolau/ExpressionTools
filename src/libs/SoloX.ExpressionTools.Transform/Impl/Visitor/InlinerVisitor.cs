// ----------------------------------------------------------------------
// <copyright file="InlinerVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Transform.Impl.Visitor
{
    /// <summary>
    /// InlinerVisitor class that will actually in-line expression and replace parameter use.
    /// </summary>
    internal sealed class InlinerVisitor : ExpressionVisitor
    {
        private readonly IParameterResolver parameterResolver;
        private readonly IDictionary<ParameterExpression, LambdaExpression> parameterMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinerVisitor"/> class.
        /// </summary>
        /// <param name="parameterResolver">The resolver that will provide the expression to in-line depending on the parameters.</param>
        public InlinerVisitor(IParameterResolver parameterResolver)
        {
            this.parameterResolver = parameterResolver;
            this.parameterMap = new Dictionary<ParameterExpression, LambdaExpression>();
        }

        /// <inheritdoc />
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.parameterMap.Clear();

            var parameters = new List<ParameterExpression>();
            foreach (var parameter in node.Parameters)
            {
                var pexp = this.parameterResolver.Resolve(parameter);
                if (pexp != null)
                {
                    this.parameterMap.Add(parameter, pexp);
                    parameters.AddRange(pexp.Parameters);
                }
                else
                {
                    parameters.Add(parameter);
                }
            }

            return Expression.Lambda(this.Visit(node.Body), parameters);
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            return Expression.MakeMemberAccess(this.Visit(node.Expression), node.Member);
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (this.parameterMap.TryGetValue(node, out var exp))
            {
                return exp.Body;
            }
            else
            {
                return node;
            }
        }
    }
}
