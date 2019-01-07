// ----------------------------------------------------------------------
// <copyright file="ExpressionVisitor.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Reflection;
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

        public Type ResolvedAsAType { get; private set; }

        public string ResolvedAsAnIdentifier { get; private set; }

        /// <inheritdoc />
        public override Expression VisitArgument(ArgumentSyntax node)
        {
            return this.Visit(node.Expression);
        }

        /// <inheritdoc />
        public override Expression VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return this.lambdaVisitor.VisitLiteralExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var member = node.Name.Identifier.Text;
            var exp = this.Visit(node.Expression);
            if (exp != null)
            {
                return Expression.MakeMemberAccess(exp, exp.Type.GetProperty(member));
            }

            if (this.ResolvedAsAType != null)
            {
                MemberInfo memberInfo = this.ResolvedAsAType.GetProperty(member);
                if (memberInfo == null)
                {
                    memberInfo = this.ResolvedAsAType.GetField(member);
                }

                this.ResolvedAsAType = null;
                return Expression.MakeMemberAccess(null, memberInfo);
            }

            if (this.ResolvedAsAnIdentifier == null)
            {
                throw new FormatException($"Unable to parse {node.ToString()}");
            }

            var identifier = $"{this.ResolvedAsAnIdentifier}.{member}";

            this.ResolverTypeIdentifier(identifier);

            return null;
        }

        /// <inheritdoc />
        public override Expression VisitIdentifierName(IdentifierNameSyntax node)
        {
            var identifier = node.Identifier.Text;
            var exp = this.lambdaVisitor.ResolveIdentifier(identifier);
            if (exp != null)
            {
                return exp;
            }

            this.ResolverTypeIdentifier(identifier);

            return null;
        }

        private void ResolverTypeIdentifier(string identifier)
        {
            var resolvedType = this.lambdaVisitor.TypeNameResolver.ResolveTypeName(identifier);
            if (resolvedType != null)
            {
                this.ResolvedAsAType = resolvedType;
                this.ResolvedAsAnIdentifier = null;
            }
            else
            {
                this.ResolvedAsAnIdentifier = identifier;
            }
        }
    }
}
