// <copyright file="TypeVisitor.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// TypeVisitor class that will actually identify Type.
    /// </summary>
    internal class TypeVisitor : CSharpSyntaxVisitor<Type>
    {
        private static readonly IReadOnlyDictionary<string, Type> PredefinedTypeMap = new Dictionary<string, Type>()
        {
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "short", typeof(short) },
            { "int", typeof(int) },
            { "long", typeof(long) },
            { "string", typeof(string) },
        };

        /// <inheritdoc />
        public override Type VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return PredefinedTypeMap[node.Keyword.Text];
        }
    }
}
