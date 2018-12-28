using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SoloX.ExpressionTools.Parser.Visitor
{
    /// <summary>
    /// Visitor to convert Lambda CSharp syntax tree to Linq Expression
    /// </summary>
    public class LambdaVisitor : CSharpSyntaxVisitor<Expression>
    {
        public IParameterTypeResolver ParameterTypeResolver { get; }

        public IMethodResolver MethodResolver { get; }

        private Dictionary<string, ParameterExpression> _parameterMap = new Dictionary<string, ParameterExpression>();

        private List<ParameterExpression> _parameters = new List<ParameterExpression>();

        private TypeVisitor _typeVisitor = new TypeVisitor();

        public Expression ResolveIdentifier(string identifier)
        {
            if (_parameterMap.TryGetValue(identifier, out var value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"unknown identifier {identifier}");
            }
        }

        public LambdaVisitor(IParameterTypeResolver parameterTypeResolver, IMethodResolver methodResolver)
        {
            ParameterTypeResolver = parameterTypeResolver;
            MethodResolver = methodResolver;
        }

        public override Expression Visit(SyntaxNode node)
        {
            return base.Visit(node);
        }

        public override Expression VisitParameter(ParameterSyntax node)
        {
            var name = node.Identifier.Text;
            var typeNode = node.Type;

            Type pType;

            if (typeNode == null)
            {
                if (ParameterTypeResolver != null)
                {
                    pType = ParameterTypeResolver.ResolveType(name);
                }
                else
                {
                    throw new FormatException($"Could not resolve the type of {name}");
                }
            }
            else
            {
                pType = _typeVisitor.Visit(typeNode);
                if (pType == null)
                {
                    throw new FormatException($"Unknown type {typeNode.ToString()}");
                }
            }

            var parameterExp = Expression.Parameter(pType, name);

            _parameterMap.Add(name, parameterExp);
            _parameters.Add(parameterExp);

            return parameterExp;
        }

        public override Expression VisitParameterList(ParameterListSyntax node)
        {
            foreach (var parameter in node.Parameters)
            {
                VisitParameter(parameter);
            }
            return null;
        }

        public override Expression VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            _parameterMap.Clear();
            _parameters.Clear();

            Visit(node.Parameter);
            var body = Visit(node.Body);
            return Expression.Lambda(
                body,
                _parameters
            );
        }

        public override Expression VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            _parameterMap.Clear();
            _parameters.Clear();

            Visit(node.ParameterList);
            var body = Visit(node.Body);
            return Expression.Lambda(
                body,
                _parameters
            );
        }

        public override Expression VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var exp = Visit(node.Expression);
            return Expression.MakeMemberAccess(exp, exp.Type.GetProperty(node.Name.Identifier.Text));
        }

        public override Expression VisitIdentifierName(IdentifierNameSyntax node)
        {
            var text = node.Identifier.Text;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            else if (_parameterMap.TryGetValue(text, out var parameterExpression))
            {
                return parameterExpression;
            }
            else
            {
                throw new FormatException($"Unknown identifier {node.Identifier.Text}");
            }
        }

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

        public override Expression VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return Visit(node.Expression);
        }

        public override Expression VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var le = Visit(node.Left);
            var re = Visit(node.Right);

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

        public override Expression VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var args = new List<Expression>();
            var argVisitor = new ExpressionVisitor(this);

            foreach (var argument in node.ArgumentList.Arguments)
            {
                args.Add(argVisitor.Visit(argument));
            }
            
            var visitor = new FromInvocationExpressionVisitor(this, args.Select(a => a.Type).ToArray());
            var mi = visitor.Visit(node.Expression);

            return Expression.Call(visitor.Expression, mi, args);
        }
    }

    public class ExpressionVisitor : CSharpSyntaxVisitor<Expression>
    {
        private LambdaVisitor _lambdaVisitor;

        public ExpressionVisitor(LambdaVisitor lambdaVisitor)
        {
            this._lambdaVisitor = lambdaVisitor;
        }

        public override Expression VisitArgument(ArgumentSyntax node)
        {
            return Visit(node.Expression);
        }

        public override Expression VisitIdentifierName(IdentifierNameSyntax node)
        {
            return _lambdaVisitor.ResolveIdentifier(node.Identifier.Text);
        }
    }

    public class FromInvocationExpressionVisitor : CSharpSyntaxVisitor<MethodInfo>
    {
        private LambdaVisitor _lambdaVisitor;
        private Type[] _argsType;

        public Expression Expression { get; private set; }

        public FromInvocationExpressionVisitor(LambdaVisitor lambdaVisitor, Type[] argsType)
        {
            this._lambdaVisitor = lambdaVisitor;
            _argsType = argsType;
        }

        public override MethodInfo VisitIdentifierName(IdentifierNameSyntax node)
        {
            return _lambdaVisitor.MethodResolver.ResolveMethod(node.Identifier.Text, _argsType);
        }
    }
}
