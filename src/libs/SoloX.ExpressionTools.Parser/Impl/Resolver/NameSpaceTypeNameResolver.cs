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
        private readonly IEnumerable<(string, string)> namespaceAssemblyItems;

        /// <summary>
        /// Setup the NameSpace type resolver.
        /// </summary>
        /// <param name="namespaceAssemblyItems"></param>
        public NameSpaceTypeNameResolver(IEnumerable<(string, string)> namespaceAssemblyItems)
        {
            this.namespaceAssemblyItems = namespaceAssemblyItems;
        }

        /// <inheritdoc/>
        public Type ResolveTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (TryResolveTypeNameWithAssembly(typeName, out var type))
            {
                return type;
            }

            foreach (var nameSpace in this.namespaceAssemblyItems)
            {
                if (TryResolveTypeNameWithAssembly($"{nameSpace.Item1}.{typeName}", out type))
                {
                    return type;
                }
            }

            // Try to remove partial namespace in typeName
            var idx = typeName.IndexOf('.');
            while (idx >= 0)
            {
                typeName = typeName.Substring(idx + 1);

                type = ResolveTypeName(typeName);

                if (type != null)
                {
                    return type;
                }

                idx = typeName.IndexOf('.');
            }

            return null;
        }

        private bool TryResolveTypeNameWithAssembly(string typeName, out Type type)
        {
            type = Type.GetType(typeName);
            if (type != null)
            {
                return true;
            }

            // Check if the type name start with a registered namespaceAssembly Items
            foreach (var nameSpace in this.namespaceAssemblyItems)
            {
                if (!string.IsNullOrEmpty(nameSpace.Item2) && typeName.StartsWith($"{nameSpace.Item1}.", StringComparison.Ordinal))
                {
                    type = Type.GetType($"{typeName}, {nameSpace.Item2}");
                    if (type != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
