// ----------------------------------------------------------------------
// <copyright file="StaticMethodResolver.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoloX.ExpressionTools.Parser.Impl.Resolver
{
    /// <summary>
    /// IMethodResolver implementation that match methods in a given type definition.
    /// </summary>
    public class StaticMethodResolver : IMethodResolver
    {
        private readonly Dictionary<MethodEntry, MethodInfo> methodMap = new Dictionary<MethodEntry, MethodInfo>();
        private readonly bool ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodResolver"/> class.
        /// </summary>
        /// <param name="ignoreCase">Tells if the resolver must ignore the method name case.</param>
        /// <param name="types">Type list used to search and resolve method name.</param>
        public StaticMethodResolver(bool ignoreCase, params Type[] types)
        {
            this.ignoreCase = ignoreCase;
            this.LoadMethodMap(types);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodResolver"/> class.
        /// </summary>
        /// <param name="types">Type list used to search and resolve method name.</param>
        public StaticMethodResolver(params Type[] types)
            : this(false, types)
        {
        }

        /// <inheritdoc />
        public MethodInfo ResolveMethod(string methodName, Type[] argsType)
        {
            var name = this.ignoreCase ? methodName.ToUpperInvariant() : methodName;
            var entry = new MethodEntry(name, argsType);
            return this.methodMap.TryGetValue(entry, out var methodInfo) ? methodInfo : null;
        }

        private void LoadMethodMap(Type[] types)
        {
            foreach (var methodInfo in types.SelectMany(t => t.GetMethods()).Where(m => m.IsStatic))
            {
                var name = this.ignoreCase ? methodInfo.Name.ToUpperInvariant() : methodInfo.Name;
                var argsType = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
                var entry = new MethodEntry(name, argsType);

                this.methodMap.Add(entry, methodInfo);
            }
        }

        private sealed class MethodEntry
        {
            public MethodEntry(string name, Type[] argumentsType)
            {
                this.Name = name;
                this.ArgumentsType = argumentsType;
            }

            public string Name { get; }

            public Type[] ArgumentsType { get; }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode() + this.ArgumentsType.Select(t => t.GetHashCode()).Sum();
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj is MethodEntry entry && entry.Name == this.Name && entry.ArgumentsType.Length == this.ArgumentsType.Length)
                {
                    for (int i = 0; i < this.ArgumentsType.Length; i++)
                    {
                        if (!object.ReferenceEquals(this.ArgumentsType[i], entry.ArgumentsType[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
