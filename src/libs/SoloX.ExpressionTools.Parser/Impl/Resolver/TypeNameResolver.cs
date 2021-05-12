// ----------------------------------------------------------------------
// <copyright file="TypeNameResolver.cs" company="Xavier Solau">
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
    /// ITypeNameResolver implementation that resolve a type name given a type list.
    /// </summary>
    public class TypeNameResolver : ITypeNameResolver
    {
        private readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNameResolver"/> class.
        /// </summary>
        /// <param name="types">The type list to match in the type name resolution.</param>
        public TypeNameResolver(params Type[] types)
        {
            foreach (var type in types)
            {
                this.typeMap.Add(type.Name, type);
                this.typeMap.Add(type.FullName, type);
            }
        }

        /// <inheritdoc />
        public Type ResolveTypeName(string typeName)
        {
            return this.typeMap.TryGetValue(typeName, out var type) ? type : null;
        }
    }
}
