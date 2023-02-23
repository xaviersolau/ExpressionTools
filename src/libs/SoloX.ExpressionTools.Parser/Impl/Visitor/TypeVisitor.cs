// ----------------------------------------------------------------------
// <copyright file="TypeVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// TypeVisitor class that will actually identify Type.
    /// </summary>
    internal sealed class TypeVisitor : CSharpSyntaxVisitor<Type>
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

        /// <inheritdoc />
        public override Type VisitArrayType(ArrayTypeSyntax node)
        {
            var type = this.Visit(node.ElementType);

            var rank = node.RankSpecifiers.Count;

            return rank == 1
                ? type.MakeArrayType()
                : type.MakeArrayType(node.RankSpecifiers.Count);
        }
    }
}
