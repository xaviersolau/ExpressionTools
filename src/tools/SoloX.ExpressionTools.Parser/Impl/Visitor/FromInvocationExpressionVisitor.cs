// ----------------------------------------------------------------------
// <copyright file="FromInvocationExpressionVisitor.cs" company="SoloX Software">
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
    /// <summary>
    /// Visitor used to visit an expression tree from an InvocationExpression node.
    /// </summary>
    internal class FromInvocationExpressionVisitor : CSharpSyntaxVisitor<MethodInfo>
    {
        private readonly LambdaVisitor lambdaVisitor;
        private readonly Type[] argsType;

        public FromInvocationExpressionVisitor(LambdaVisitor lambdaVisitor, Type[] argsType)
        {
            this.lambdaVisitor = lambdaVisitor;
            this.argsType = argsType;
        }

        /// <summary>
        /// Gets the instance expression on witch the method is called.
        /// </summary>
        public Expression InstanceExpression { get; private set; }

        /// <inheritdoc />
        public override MethodInfo VisitIdentifierName(IdentifierNameSyntax node)
        {
            return this.lambdaVisitor.MethodResolver.ResolveMethod(node.Identifier.Text, this.argsType);
        }
    }
}
