using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ExpressionTools
{
    public interface IParameterResolver
    {
        LambdaExpression Resolve(ParameterExpression parameter);
    }
}
