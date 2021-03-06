﻿// ----------------------------------------------------------------------
// <copyright file="ITypeNameResolver.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.ExpressionTools.Parser
{
    /// <summary>
    /// Interface used by the ExpressionParser in order to resolve type name.
    /// </summary>
    public interface ITypeNameResolver
    {
        /// <summary>
        /// Provide a Type from the given type name.
        /// </summary>
        /// <param name="typeName">The type name to match.</param>
        /// <returns>The resolved type matching the given name or null otherwise.</returns>
        Type ResolveTypeName(string typeName);
    }
}
