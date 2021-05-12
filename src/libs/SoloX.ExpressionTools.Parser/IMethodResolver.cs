// ----------------------------------------------------------------------
// <copyright file="IMethodResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reflection;

namespace SoloX.ExpressionTools.Parser
{
    /// <summary>
    /// Interface used by the ExpressionParser in order to resolve method invocation expression.
    /// </summary>
    public interface IMethodResolver
    {
        /// <summary>
        /// Provide a MethodInfo matching the given name and the given parameter types.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <param name="argsType">The method argument types.</param>
        /// <returns>The MethodInfo.</returns>
        MethodInfo ResolveMethod(string methodName, Type[] argsType);
    }
}
