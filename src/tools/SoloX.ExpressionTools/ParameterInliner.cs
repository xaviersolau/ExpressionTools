using SoloX.ExpressionTools.Visitor;
using System;
using System.Linq.Expressions;

namespace SoloX.ExpressionTools
{
    public class ParameterInliner
    {
        private InlinerVisitor _inlinerVisitor;

        public ParameterInliner(IParameterResolver parameterResolver)
        {
            _inlinerVisitor = new InlinerVisitor(parameterResolver);
        }

        public LambdaExpression Inline<TDelegate>(Expression<TDelegate> expression)
        {
            return (LambdaExpression)_inlinerVisitor.Visit(expression);
        }
    }
}
