using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ExpressionTools.Visitor
{
    public class InlinerVisitor : ExpressionVisitor
    {
        private IParameterResolver _parameterResolver;
        private IDictionary<ParameterExpression, LambdaExpression> _parameterMap;

        public InlinerVisitor(IParameterResolver parameterResolver)
        {
            this._parameterResolver = parameterResolver;
            _parameterMap = new Dictionary<ParameterExpression, LambdaExpression>();
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _parameterMap.Clear();

            var parameters = new List<ParameterExpression>();
            foreach (var parameter in node.Parameters)
            {
                var pexp = _parameterResolver.Resolve(parameter);
                if (pexp != null)
                {
                    _parameterMap.Add(parameter, pexp);
                    parameters.AddRange(pexp.Parameters);
                }
            }

            return Expression.Lambda(Visit(node.Body), parameters);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return Expression.MakeMemberAccess(Visit(node.Expression), node.Member);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_parameterMap.TryGetValue(node, out var exp))
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
