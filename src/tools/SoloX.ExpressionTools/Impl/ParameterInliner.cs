using SoloX.ExpressionTools.Impl.Visitor;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools.Impl
{
    /// <inheritdoc />
    public class ParameterInliner : IParameterInliner
    {
        private InlinerVisitor _inlinerVisitor;

        /// <summary>
        /// Setup ParameterInliner with a given parameter resolver.
        /// </summary>
        /// <param name="parameterResolver">The parameter resolver that provides the expression to in-line
        /// replacing a given parameter</param>
        public ParameterInliner(IParameterResolver parameterResolver)
        {
            _inlinerVisitor = new InlinerVisitor(parameterResolver);
        }

        /// <inheritdoc />
        public Expression<TOutputDelegate> Inline<TInputDelegate, TOutputDelegate>(Expression<TInputDelegate> expression)
        {
            return (Expression<TOutputDelegate>)_inlinerVisitor.Visit(expression);
        }

        /// <inheritdoc />
        public LambdaExpression Inline(LambdaExpression expression)
        {
            return (LambdaExpression)_inlinerVisitor.Visit(expression);
        }
    }
}
