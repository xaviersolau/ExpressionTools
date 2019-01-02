using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ExpressionTools
{
    /// <summary>
    /// Interface used by the ParameterInliner in order to resolve expression to in-line for a given parameter.
    /// </summary>
    public interface IParameterResolver
    {
        /// <summary>
        /// Resolve the expression to in-line for the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter to resolve</param>
        /// <returns>The expression to in-line or null if the parameter must not be replaced</returns>
        LambdaExpression Resolve(ParameterExpression parameter);
    }
}
