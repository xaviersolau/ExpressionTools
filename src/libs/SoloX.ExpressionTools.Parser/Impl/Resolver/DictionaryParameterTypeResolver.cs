﻿// ----------------------------------------------------------------------
// <copyright file="DictionaryParameterTypeResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SoloX.ExpressionTools.Parser.Impl.Resolver
{
    /// <summary>
    /// IParameterTypeResolver implementation using a Dictionary as input to resolve a parameter type
    /// given a parameter name.
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
