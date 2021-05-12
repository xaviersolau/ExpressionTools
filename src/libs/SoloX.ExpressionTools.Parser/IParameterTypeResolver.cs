// ----------------------------------------------------------------------
// <copyright file="IParameterTypeResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.ExpressionTools.Parser
{
    /// <summary>
    /// interface used by the expression parser to resolve parameter type.
    /// </summary>
    public interface IParameterTypeResolver
    {
        /// <summary>
        /// Provide the Type associated to the parameter matching the given name.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>The Type of the parameter.</returns>
        Type ResolveType(string parameterName);
    }
}
