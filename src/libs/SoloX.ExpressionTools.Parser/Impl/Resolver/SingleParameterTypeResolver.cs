// ----------------------------------------------------------------------
// <copyright file="SingleParameterTypeResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.ExpressionTools.Parser.Impl.Resolver
{
    /// <summary>
    /// IParameterTypeResolver implementation using a single type to resolve a parameter type
    /// given a parameter name.
    /// </summary>
    public class SingleParameterTypeResolver : IParameterTypeResolver
    {
        private readonly Type typeToResolve;
        private string parameterName;

        /// <summary>
        /// Setup the single parameter type resolver.
        /// </summary>
        /// <param name="typeToResolve">Type to resolve.</param>
        /// <param name="parameterName">Parameter name to match.</param>
        public SingleParameterTypeResolver(Type typeToResolve, string parameterName = null)
        {
            this.typeToResolve = typeToResolve;
            this.parameterName = parameterName;
        }

        ///<inheritdoc/>
        public Type ResolveType(string parameterName)
        {
            if (this.parameterName != null && !this.parameterName.Equals(parameterName, StringComparison.Ordinal))
            {
                throw new NotSupportedException($"Multiple parameter detected registered: {this.parameterName} new: {parameterName}");
            }

            this.parameterName = parameterName;
            return this.typeToResolve;
        }
    }
}
