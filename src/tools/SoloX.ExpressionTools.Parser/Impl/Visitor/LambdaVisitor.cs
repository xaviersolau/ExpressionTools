// ----------------------------------------------------------------------
// <copyright file="LambdaVisitor.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// Visitor to convert Lambda CSharp syntax tree to Linq Expression.
    /// </summary>
    internal class LambdaVisitor : CSharpSyntaxVisitor<Expression>
    {
        private readonly List<ParameterExpression> parameters = new List<ParameterExpression>();
        private readonly Dictionary<string, ParameterExpression> parameterMap = new Dictionary<string, ParameterExpression>();
        private readonly TypeVisitor typeVisitor = new TypeVisitor();

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaVisitor"/> class.
        /// </summary>
        /// <param name="parameterTypeResolver">Resolver that will identify the parameters type.</param>
        /// <param name="methodResolver">Resolver that will identify methods.</param>
        /// <param name="typeNameResolver">Resolver that will identify types.</param>
        public LambdaVisitor(IParameterTypeResolver parameterTypeResolver, IMethodResolver methodResolver, ITypeNameResolver typeNameResolver)
        {
            this.ParameterTypeResolver = parameterTypeResolver;
            this.MethodResolver = methodResolver;
            this.TypeNameResolver = typeNameResolver;
        }

        /// <summary>
        /// Gets the parameter type resolver.
        /// </summary>
        public IParameterTypeResolver ParameterTypeResolver { get; }

        /// <summary>
        /// Gets the method resolver.
        /// </summary>
        public IMethodResolver MethodResolver { get; }

        /// <summary>
        /// Gets the type name resolver.
        /// </summary>
        public ITypeNameResolver TypeNameResolver { get; }

        /// <summary>
        /// Resolve the given identifier as an Expression.
        /// </summary>
        /// <param name="identifier">The identifier to resolve.</param>
        /// <returns>The resulting Expression or null if it can not be resolved as an Expression.</returns>
        public Expression ResolveIdentifier(string identifier)
        {
            if (this.parameterMap.TryGetValue(identifier, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc />
        public override Expression Visit(SyntaxNode node)
        {
            return base.Visit(node);
        }

        /// <inheritdoc />
        public override Expression VisitParameter(ParameterSyntax node)
        {
            var name = node.Identifier.Text;
            var typeNode = node.Type;

            Type pType;

            if (typeNode == null)
            {
                if (this.ParameterTypeResolver != null)
                {
                    pType = this.ParameterTypeResolver.ResolveType(name);
                }
                else
                {
                    throw new FormatException($"Could not resolve the type of {name}");
                }
            }
            else
            {
                pType = this.typeVisitor.Visit(typeNode);
                if (pType == null)
                {
                    throw new FormatException($"Unknown type {typeNode.ToString()}");
                }
            }

            var parameterExp = Expression.Parameter(pType, name);

            this.parameterMap.Add(name, parameterExp);
            this.parameters.Add(parameterExp);

            return parameterExp;
        }

        /// <inheritdoc />
        public override Expression VisitParameterList(ParameterListSyntax node)
        {
            foreach (var parameter in node.Parameters)
            {
                this.VisitParameter(parameter);
            }

            return null;
        }

        /// <inheritdoc />
        public override Expression VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            this.parameterMap.Clear();
            this.parameters.Clear();

            this.Visit(node.Parameter);
            var body = this.Visit(node.Body);
            return Expression.Lambda(
                body,
                this.parameters);
        }

        /// <inheritdoc />
        public override Expression VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            this.parameterMap.Clear();
            this.parameters.Clear();

            this.Visit(node.ParameterList);
            var body = this.Visit(node.Body);
            return Expression.Lambda(
                body,
                this.parameters);
        }

        /// <inheritdoc />
        public override Expression VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var exp = this.Visit(node.Expression);
            return Expression.MakeMemberAccess(exp, exp.Type.GetProperty(node.Name.Identifier.Text));
        }

        /// <inheritdoc />
        public override Expression VisitIdentifierName(IdentifierNameSyntax node)
        {
            var text = node.Identifier.Text;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            else if (this.parameterMap.TryGetValue(text, out var parameterExpression))
            {
                return parameterExpression;
            }
            else
            {
                throw new FormatException($"Unknown identifier {node.Identifier.Text}");
            }
        }

        /// <inheritdoc />
        public override Expression VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var kind = node.Kind();
            switch (kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    return Expression.Constant(node.Token.Value);
                default:
                    throw new FormatException($"unsupported operator {node.Token.ValueText}");
            }
        }

        /// <inheritdoc />
        public override Expression VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return this.Visit(node.Expression);
        }

        /// <inheritdoc />
        public override Expression VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var le = this.Visit(node.Left);
            var re = this.Visit(node.Right);

            var kind = node.OperatorToken.Kind();
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return Expression.Add(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.MinusToken:
                    return Expression.Subtract(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.AsteriskToken:
                    return Expression.Multiply(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.SlashToken:
                    return Expression.Divide(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.CaretToken:
                    return Expression.Power(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.PercentToken:
                    return Expression.Modulo(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                default:
                    throw new FormatException($"unsupported operator {node.OperatorToken.ValueText}");
            }
        }

        /// <inheritdoc />
        public override Expression VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var args = new List<Expression>();
            var argVisitor = new ExpressionVisitor(this);

            foreach (var argument in node.ArgumentList.Arguments)
            {
                var argExp = argVisitor.Visit(argument);
                args.Add(argExp);
            }

            var visitor = new FromInvocationExpressionVisitor(this, args.Select(a => a.Type).ToArray());
            var mi = visitor.Visit(node.Expression);

            return Expression.Call(visitor.InstanceExpression, mi, args);
        }

        private static Expression ConvertIfNeeded(Expression expression, Type targetType)
        {
            if (expression.Type == targetType)
            {
                return expression;
            }

            if (expression.Type == typeof(double))
            {
                return expression;
            }

            if (expression.Type == typeof(float) && targetType != typeof(double))
            {
                return expression;
            }

            return Expression.Convert(expression, targetType);
        }
    }
}
