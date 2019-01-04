// ----------------------------------------------------------------------
// <copyright file="ExpressionVisitor.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    internal class ExpressionVisitor : CSharpSyntaxVisitor<Expression>
    {
        private LambdaVisitor lambdaVisitor;

        public ExpressionVisitor(LambdaVisitor lambdaVisitor)
        {
            this.lambdaVisitor = lambdaVisitor;
        }

        /// <inheritdoc />
        public override Expression VisitArgument(ArgumentSyntax node)
        {
            return this.Visit(node.Expression);
        }

        /// <inheritdoc />
        public override Expression VisitIdentifierName(IdentifierNameSyntax node)
        {
            return this.lambdaVisitor.ResolveIdentifier(node.Identifier.Text);
        }
    }
}
