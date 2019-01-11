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
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// Visitor to convert Lambda CSharp syntax tree to Linq Expression.
    /// </summary>
    internal class LambdaVisitor : CSharpSyntaxVisitor<LambdaVisitorAttribute>
    {
        private readonly Stack<LambdaVisitorAttribute> attributes = new Stack<LambdaVisitorAttribute>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaVisitor"/> class.
        /// </summary>
        /// <param name="parameterTypeResolver">Resolver that will identify the parameters type.</param>
        /// <param name="methodResolver">Resolver that will identify methods.</param>
        /// <param name="typeNameResolver">Resolver that will identify types.</param>
        public LambdaVisitor(
            IParameterTypeResolver parameterTypeResolver,
            IMethodResolver methodResolver,
            ITypeNameResolver typeNameResolver)
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

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    var parameterAttribute = this.Visit(node.ParameterList);

                    var bodyAttribute = this.Visit(node.Body);

                    attribute.ResultingExpression = Expression.Lambda(
                        bodyAttribute.ResultingExpression,
                        parameterAttribute.Parameters);
                });
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    this.Visit(node.Parameter);

                    var bodyAttribute = this.Visit(node.Body);

                    attribute.ResultingExpression = Expression.Lambda(
                        bodyAttribute.ResultingExpression,
                        bodyAttribute.Parameters);
                });
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitParameter(ParameterSyntax node)
        {
            var attribute = this.attributes.Peek();

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
                var typeAttribute = this.VisitWithNewAttribute(_ => this.Visit(typeNode));

                pType = typeAttribute.ResultingType;
                if (pType == null)
                {
                    throw new FormatException($"Unknown type {typeNode.ToString()}");
                }
            }

            var parameterExp = Expression.Parameter(pType, name);

            attribute.ParameterMap.Add(name, parameterExp);
            attribute.Parameters.Add(parameterExp);

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitParameterList(ParameterListSyntax node)
        {
            foreach (var parameter in node.Parameters)
            {
                this.VisitParameter(parameter);
            }

            return this.attributes.Peek();
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    var args = new List<Expression>();

                    foreach (var argument in node.ArgumentList.Arguments)
                    {
                        var argumentAttribute = this.Visit(argument);
                        if (argumentAttribute.ResultingExpression == null)
                        {
                            throw new FormatException($"Unable to evaluate argument: {argument}");
                        }

                        args.Add(argumentAttribute.ResultingExpression);
                    }

                    attribute.ArgumentTypes = args.Select(a => a.Type).ToArray();

                    var expressionAttribute = this.Visit(node.Expression);

                    attribute.ResultingExpression = Expression.Call(
                        expressionAttribute.ResultingExpression,
                        expressionAttribute.ResultingMethodInfo,
                        ConvertTypeForMethodCall(args, expressionAttribute.ResultingMethodInfo));
                });
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitArgument(ArgumentSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    var expAttribute = this.Visit(node.Expression);
                    attribute.ResultingExpression = expAttribute.ResultingExpression;
                });
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();

            var memberName = node.Name.Identifier.Text;

            var expressionAttribute = this.Visit(node.Expression);
            var exp = expressionAttribute.ResultingExpression;

            var type = exp != null ? exp.Type : expressionAttribute.ResultingType;

            if (type == null)
            {
                var identifier = attribute.ResultingIdentifier + $".{memberName}";
                if (this.TryToResolveAsAType(identifier, out type))
                {
                    attribute.ResultingType = type;
                }
                else
                {
                    attribute.ResultingIdentifier = identifier;
                }

                return attribute;
            }

            if (attribute.ArgumentTypes != null)
            {
                attribute.ResultingMethodInfo = type.GetMethod(memberName, attribute.ArgumentTypes);
            }

            if (attribute.ResultingMethodInfo == null)
            {
                MemberInfo member = type.GetProperty(memberName);
                if (member == null)
                {
                    member = type.GetField(memberName);
                }

                attribute.ResultingExpression = Expression.MakeMemberAccess(exp, member);
            }
            else
            {
                attribute.ResultingExpression = exp;
            }

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitIdentifierName(IdentifierNameSyntax node)
        {
            var attribute = this.attributes.Peek();

            var text = node.Identifier.Text;
            if (string.IsNullOrEmpty(text))
            {
                throw new FormatException($"Identifier must be specified");
            }
            else if (attribute.ParameterMap.TryGetValue(text, out var parameterExpression))
            {
                attribute.ResultingExpression = parameterExpression;
            }
            else if (this.TryToResolveAsAMethod(text, attribute.ArgumentTypes, out var methodInfo))
            {
                attribute.ResultingMethodInfo = methodInfo;
            }
            else if (this.TryToResolveAsAType(text, out var type))
            {
                attribute.ResultingType = type;
            }
            else
            {
                attribute.ResultingIdentifier = text;
            }

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return this.Visit(node.Expression);
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();

            var leftAttribute = this.Visit(node.Left);
            var lexp = leftAttribute.ResultingExpression;

            var rightAttribute = this.Visit(node.Right);
            var rexp = rightAttribute.ResultingExpression;

            attribute.ResultingExpression = CreateBinaryExpression(node.OperatorToken.Kind(), lexp, rexp, node);

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();

            var expAttribute = this.Visit(node.Operand);
            var exp = expAttribute.ResultingExpression;

            attribute.ResultingExpression = CreatePrefixUnaryExpression(node.OperatorToken.Kind(), exp, node);

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();
            var kind = node.Kind();
            switch (kind)
            {
                case SyntaxKind.NumericLiteralExpression:
                    attribute.ResultingExpression = Expression.Constant(node.Token.Value);
                    break;
                default:
                    throw new FormatException($"unsupported operator {node.Token.ValueText}");
            }

            return attribute;
        }

        private static Expression CreateBinaryExpression(SyntaxKind kind, Expression le, Expression re, BinaryExpressionSyntax node)
        {
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

        private static Expression CreatePrefixUnaryExpression(SyntaxKind kind, Expression exp, PrefixUnaryExpressionSyntax node)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return Expression.UnaryPlus(exp);
                case SyntaxKind.MinusToken:
                    return Expression.Negate(exp);
                default:
                    throw new FormatException($"unsupported operator {node.OperatorToken.ValueText}");
            }
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

        private static IEnumerable<Expression> ConvertTypeForMethodCall(IReadOnlyList<Expression> args, MethodInfo methodInfo)
        {
            var argCount = args.Count;
            var convertedArgs = new Expression[argCount];
            var parameters = methodInfo.GetParameters();
            for (int i = 0; i < argCount; i++)
            {
                var exp = args[i];
                var parameterInfo = parameters[i];
                var expectedType = parameterInfo.ParameterType;

                if (exp.Type != expectedType)
                {
                    convertedArgs[i] = Expression.Convert(exp, expectedType);
                }
                else
                {
                    convertedArgs[i] = exp;
                }
            }

            return convertedArgs;
        }

        private LambdaVisitorAttribute VisitWithNewAttribute(Action<LambdaVisitorAttribute> action)
        {
            var attribute = new LambdaVisitorAttribute(this.attributes.Any() ? this.attributes.Peek() : null);
            this.attributes.Push(attribute);
            action(attribute);
            return this.attributes.Pop();
        }

        private bool TryToResolveAsAType(string text, out Type type)
        {
            type = this.TypeNameResolver.ResolveTypeName(text);
            return type != null;
        }

        private bool TryToResolveAsAMethod(string text, Type[] argumentTypes, out MethodInfo methodInfo)
        {
            methodInfo = (argumentTypes != null) ? this.MethodResolver.ResolveMethod(text, argumentTypes) : null;
            return methodInfo != null;
        }
    }
}
