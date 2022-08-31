// ----------------------------------------------------------------------
// <copyright file="LambdaVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
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
using SoloX.ExpressionTools.Parser.Impl.Resolver;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// Visitor to convert Lambda CSharp syntax tree to Linq Expression.
    /// </summary>
#pragma warning disable IDE0066 // Convertir l'instruction switch en expression
#pragma warning disable IDE0010 // Remplir une instruction switch
    internal class LambdaVisitor : CSharpSyntaxVisitor<LambdaVisitorAttribute>
    {
        private readonly Stack<LambdaVisitorAttribute> attributes = new Stack<LambdaVisitorAttribute>();
        private readonly TypeVisitor typeVisitor = new TypeVisitor();
        private readonly ITypeNameResolver defaultSystemTypeNameResolver;

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

            this.defaultSystemTypeNameResolver = new NameSpaceTypeNameResolver(new (string, string)[] { ("System", null), ("System.Linq", "System.Linq"), ("System.Globalization", null) });
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
                pType = this.typeVisitor.Visit(typeNode);

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
                    var args = this.VisitArgumentList(node.ArgumentList.Arguments);

                    attribute.ArgumentTypes = args.Select(a => a.Type).ToArray();

                    var expressionAttribute = this.Visit(node.Expression);

                    if (expressionAttribute.ResultingMethodInfo.IsStatic && expressionAttribute.ResultingExpression != null)
                    {
                        attribute.ResultingExpression = Expression.Call(
                            null,
                            expressionAttribute.ResultingMethodInfo,
                            ConvertTypeForMethodCall(new Expression[] { expressionAttribute.ResultingExpression }.Concat(args).ToArray(), expressionAttribute.ResultingMethodInfo));
                    }
                    else
                    {
                        attribute.ResultingExpression = Expression.Call(
                            expressionAttribute.ResultingExpression,
                            expressionAttribute.ResultingMethodInfo,
                            ConvertTypeForMethodCall(args, expressionAttribute.ResultingMethodInfo));
                    }
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
        public override LambdaVisitorAttribute VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
#pragma warning disable IDE0072 // Add missing cases
                    attribute.ResultingType = node.Keyword.Kind() switch
                    {
                        SyntaxKind.StringKeyword => typeof(string),
                        SyntaxKind.ByteKeyword => typeof(byte),
                        SyntaxKind.CharKeyword => typeof(char),
                        SyntaxKind.ShortKeyword => typeof(short),
                        SyntaxKind.UShortKeyword => typeof(ushort),
                        SyntaxKind.IntKeyword => typeof(int),
                        SyntaxKind.UIntKeyword => typeof(int),
                        SyntaxKind.LongKeyword => typeof(long),
                        SyntaxKind.ULongKeyword => typeof(long),
                        SyntaxKind.FloatKeyword => typeof(float),
                        SyntaxKind.DoubleKeyword => typeof(double),
                        SyntaxKind.DecimalKeyword => typeof(decimal),
                        _ => null,
                    };
#pragma warning restore IDE0072 // Add missing cases
                });
        }

        public override LambdaVisitorAttribute VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            return BuildArrayCreationLambda(node.Initializer.Expressions, null);
        }

        public override LambdaVisitorAttribute VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            var typeAttr = this.Visit(node.Type.ElementType);
            var itemType = typeAttr.ResultingType;

            return BuildArrayCreationLambda(node.Initializer.Expressions, itemType);
        }

        private LambdaVisitorAttribute BuildArrayCreationLambda(SeparatedSyntaxList<ExpressionSyntax> itemsNode, Type itemType)
        {
            var expressions = new List<Expression>();

            var attr = this.VisitWithNewAttribute(
                attribute =>
                {

                    foreach (var exp in itemsNode)
                    {
                        var itemAttr = this.Visit(exp);

                        expressions.Add(itemAttr.ResultingExpression);

                        if (itemType == null)
                        {
                            itemType = itemAttr.ResultingExpression.Type;
                        }
                        else if (itemType != itemAttr.ResultingExpression.Type)
                        {
                            throw new FormatException($"Expecting item type: {itemType.Name} but got {itemAttr.ResultingExpression.Type.Name}");
                        }
                    }
                });

            attr.ResultingExpression = Expression.NewArrayInit(itemType, expressions);
            return attr;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();

            var identifierAttribute = this.VisitWithNewAttribute(attr =>
            {
                this.Visit(node.Name);
            });

            var memberName = identifierAttribute.ResultingIdentifier;
            var genericParameters = identifierAttribute.GenericParameters;

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

                if (attribute.ResultingMethodInfo == null)
                {
                    if (type.IsArray)
                    {
                        var enumerable = type.Name == typeof(IEnumerable<>).Name
                            ? type
                            : type.GetTypeInfo().GetInterface(typeof(IEnumerable<>).Name);
                        var itemType = enumerable.GetGenericArguments()[0];

                        var methods = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod)
                            .Where(m => m.Name == memberName && m.GetParameters().Length == attribute.ArgumentTypes.Length + 1);

                        var method = methods.FirstOrDefault();

                        attribute.ResultingMethodInfo = method?.MakeGenericMethod(itemType);
                    }
                    else
                    {
                        var arity = genericParameters?.Length ?? 0;
                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod).Where(m => m.Name == memberName);

                        if (arity != 0)
                        {
                            methods = methods.Where(m => m.IsGenericMethodDefinition).Select(m => m.MakeGenericMethod(genericParameters));
                        }

                        methods = methods.Where(m =>
                        {
                            if (m.GetParameters().Length >= attribute.ArgumentTypes.Length && m.GetParameters().Where(p => !p.IsOptional).Count() <= attribute.ArgumentTypes.Length)
                            {
                                for (var i = 0; i < attribute.ArgumentTypes.Length; i++)
                                {
                                    var parameter = m.GetParameters()[i];
                                    var argument = attribute.ArgumentTypes[i];
                                    if (!parameter.ParameterType.IsAssignableFrom(argument))
                                    {
                                        return false;
                                    }
                                }

                                return true;
                            }
                            return false;
                        }).ToArray();

                        attribute.ResultingMethodInfo = methods.SingleOrDefault();
                    }
                }
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

            var text = $"{attribute.QualifiedNamePrefix}{node.Identifier.Text}";

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
                attribute.ResultingIdentifier = text;
                attribute.ResultingType = type;
            }
            else
            {
                attribute.ResultingIdentifier = text;
            }

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitQualifiedName(QualifiedNameSyntax node)
        {
            var attribute = this.attributes.Peek();

            var prefix = node.Left.ToString();

            attribute.QualifiedNamePrefix += prefix + '.';

            return this.Visit(node.Right);
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitGenericName(GenericNameSyntax node)
        {
            var attribute = this.attributes.Peek();

            var arguments = new List<Type>();
            foreach (var argument in node.TypeArgumentList.Arguments)
            {
                var attr = this.VisitWithNewAttribute(
                    attribute =>
                    {
                        var typeAttribute = this.Visit(argument);
                        var typeToCreate = typeAttribute.ResultingType;
                    });

                arguments.Add(attr.ResultingType);
            }

            var text = $"{attribute.QualifiedNamePrefix}{node.Identifier.Text}`{node.Arity}";

            if (string.IsNullOrEmpty(node.Identifier.Text))
            {
                throw new FormatException($"Identifier must be specified");
            }
            else if (this.TryToResolveAsAType(text, out var genType))
            {
                attribute.ResultingIdentifier = text;

                attribute.ResultingType = genType.MakeGenericType(arguments.ToArray());
            }
            else
            {
                attribute.ResultingIdentifier = node.Identifier.Text;

                attribute.GenericParameters = arguments.ToArray();
            }

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return this.Visit(node.Expression);
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitCastExpression(CastExpressionSyntax node)
        {
            var attr = this.VisitWithNewAttribute(
                attribute =>
                {
                    var typeAttribute = this.Visit(node.Type);
                    var typeToCreate = typeAttribute.ResultingType;

                    var expressionAttribute = this.Visit(node.Expression);
                    var exp = expressionAttribute.ResultingExpression;

                    attribute.ResultingExpression = Expression.Convert(exp, typeToCreate);
                });
            return attr;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    var args = this.VisitArgumentList(node.ArgumentList.Arguments);

                    var argumentTypes = args.Select(a => a.Type).ToArray();

                    var expressionAttribute = this.Visit(node.Expression);
                    var exp = expressionAttribute.ResultingExpression;
                    var methodInfo = exp.Type.GetMethod("Get", argumentTypes);

                    attribute.ResultingExpression = Expression.ArrayIndex(
                        exp,
                        ConvertTypeForMethodCall(args, methodInfo));
                });
        }

        public override LambdaVisitorAttribute VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            return this.VisitWithNewAttribute(
                attribute =>
                {
                    var args = this.VisitArgumentList(node.ArgumentList.Arguments);

                    var argumentTypes = args.Select(a => a.Type).ToArray();

                    var typeAttribute = this.Visit(node.Type);
                    var typeToCreate = typeAttribute.ResultingType;
                    var constructorInfo = typeToCreate.GetConstructor(argumentTypes);

                    attribute.ResultingExpression = Expression.New(
                        constructorInfo,
                        ConvertTypeForConstructorCall(args, constructorInfo));
                });
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
                case SyntaxKind.StringLiteralExpression:
                    attribute.ResultingExpression = Expression.Constant(node.Token.Value);
                    break;
                case SyntaxKind.NullLiteralExpression:
                    attribute.ResultingExpression = Expression.Default(typeof(object));
                    break;
                default:
                    throw new FormatException($"unsupported operator {node.Token.ValueText}");
            }

            return attribute;
        }

        /// <inheritdoc />
        public override LambdaVisitorAttribute VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            var attribute = this.attributes.Peek();

            var testAttribute = this.Visit(node.Condition);
            var testExp = testAttribute.ResultingExpression;
            var ifTrueAttribute = this.Visit(node.WhenTrue);
            var ifTrueExp = ifTrueAttribute.ResultingExpression;
            var ifFalseAttribute = this.Visit(node.WhenFalse);
            var ifFalseExp = ifFalseAttribute.ResultingExpression;

            attribute.ResultingExpression = Expression.Condition(testExp, ifTrueExp, ifFalseExp);

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
                case SyntaxKind.PercentToken:
                    return Expression.Modulo(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.GreaterThanToken:
                    return Expression.GreaterThan(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.GreaterThanEqualsToken:
                    return Expression.GreaterThanOrEqual(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.LessThanToken:
                    return Expression.LessThan(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.LessThanEqualsToken:
                    return Expression.LessThanOrEqual(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.EqualsEqualsToken:
                    return Expression.Equal(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.ExclamationEqualsToken:
                    return Expression.NotEqual(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.AmpersandAmpersandToken:
                    return Expression.And(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.BarBarToken:
                    return Expression.Or(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
                case SyntaxKind.CaretToken:
                    return Expression.ExclusiveOr(ConvertIfNeeded(le, re.Type), ConvertIfNeeded(re, le.Type));
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
                case SyntaxKind.ExclamationToken:
                    return Expression.Not(exp);
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

            // If exp is nullable we should not convert.
            if (expression.Type.IsValueType && expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return expression;
            }

            return Expression.Convert(expression, targetType);
        }

        private static IEnumerable<Expression> ConvertTypeForConstructorCall(IReadOnlyList<Expression> args, ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            return ConvertArgumentsForCall(args, parameters);
        }

        private static IEnumerable<Expression> ConvertTypeForMethodCall(IReadOnlyList<Expression> args, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            return ConvertArgumentsForCall(args, parameters);
        }

        private static IEnumerable<Expression> ConvertArgumentsForCall(IReadOnlyList<Expression> args, ParameterInfo[] parameters)
        {
            var argCount = args.Count;
            var convertedArgs = new Expression[argCount];
            for (var i = 0; i < argCount; i++)
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

        private List<Expression> VisitArgumentList(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = new List<Expression>();

            foreach (var argument in arguments)
            {
                var argumentAttribute = this.Visit(argument);
                if (argumentAttribute.ResultingExpression == null)
                {
                    throw new FormatException($"Unable to evaluate argument: {argument}");
                }

                args.Add(argumentAttribute.ResultingExpression);
            }

            return args;
        }

        private bool TryToResolveAsAType(string text, out Type type)
        {
            type = this.TypeNameResolver?.ResolveTypeName(text);

            if (type == null)
            {
                type = this.defaultSystemTypeNameResolver.ResolveTypeName(text);
            }

            return type != null;
        }

        private bool TryToResolveAsAMethod(string text, Type[] argumentTypes, out MethodInfo methodInfo)
        {
            methodInfo = (argumentTypes != null) ? this.MethodResolver?.ResolveMethod(text, argumentTypes) : null;
            return methodInfo != null;
        }
    }
}
#pragma warning restore IDE0010 // Remplir une instruction switch
#pragma warning restore IDE0066 // Convertir l'instruction switch en expression
