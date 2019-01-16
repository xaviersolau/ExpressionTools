// ----------------------------------------------------------------------
// <copyright file="DictionaryParameterTypeResolver.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ExpressionTools.Parser.Impl.Resolver
{
    /// <summary>
    /// IParameterTypeResolver implementation using a Dictionary as input.
    /// </summary>
    public class DictionaryParameterTypeResolver : IParameterTypeResolver
    {
        private readonly IReadOnlyDictionary<string, Type> typeMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryParameterTypeResolver"/> class.
        /// </summary>
        /// <param name="typeMap">The parameter type map.</param>
        public DictionaryParameterTypeResolver(IReadOnlyDictionary<string, Type> typeMap)
        {
            this.typeMap = typeMap;
        }

        /// <inheritdoc />
        public Type ResolveType(string parameterName)
        {
            return this.typeMap.TryGetValue(parameterName, out var type) ? type : null;
        }
    }
}
