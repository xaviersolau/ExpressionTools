// ----------------------------------------------------------------------
// <copyright file="NameSpaceTypeNameResolver.cs" company="Xavier Solau">
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
    /// ITypeNameResolver implementation that resolve a type name given a nameSpace list.
    /// </summary>
    public class NameSpaceTypeNameResolver : ITypeNameResolver
    {
        private readonly IEnumerable<string> namespaces;

        /// <summary>
        /// Setup the NameSpace type resolver.
        /// </summary>
        /// <param name="namespaces"></param>
        public NameSpaceTypeNameResolver(IEnumerable<string> namespaces)
        {
            this.namespaces = namespaces;
        }

        /// <inheritdoc/>
        public Type ResolveTypeName(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            foreach (var nameSpace in this.namespaces)
            {
                type = Type.GetType($"{nameSpace}.{typeName}");
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
