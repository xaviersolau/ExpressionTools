using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SoloX.ExpressionTools.Parser.Impl.Visitor;

namespace SoloX.ExpressionTools.Parser.Impl
{
    /// <inheritdoc />
    public class ExpressionParser : IExpressionParser
    {
        private LambdaVisitor _visitor;

        /// <summary>
        /// Setup the ExpressionParser with a parameter type resolver and a method resolver
        /// </summary>
        /// <param name="parameterTypeResolver">Resolver that will be used to associate a Type to a given parameter name</param>
        /// <param name="methodResolver">Resolver that will be used to identify a method given a name and an argument type list</param>
        public ExpressionParser(IParameterTypeResolver parameterTypeResolver, IMethodResolver methodResolver)
        {
            _visitor = new LambdaVisitor(parameterTypeResolver, methodResolver);
        }

        /// <inheritdoc />
        public Expression<TDelegate> Parse<TDelegate>(string lambdaExpressionText)
        {
            var stree = GetLambdaSyntaxNode(lambdaExpressionText);

            var expRes = _visitor.Visit(stree);
            return (Expression<TDelegate>)expRes;
        }

        /// <inheritdoc />
        public LambdaExpression Parse(string lambdaExpressionText)
        {
            var stree = GetLambdaSyntaxNode(lambdaExpressionText);

            var expRes = _visitor.Visit(stree);
            return (LambdaExpression)expRes;
        }

        private static LambdaExpressionSyntax GetLambdaSyntaxNode(string text)
        {
            var src = SourceText.From($"var d = {text};");
            var syntaxTree = CSharpSyntaxTree.ParseText(src, new CSharpParseOptions(LanguageVersion.Latest));
            var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

            if (root.ContainsSkippedText || root.ContainsDiagnostics)
            {
                throw new FormatException($"error in expression, syntactic error: {text}");
            }

            if (root.Members.Count != 1)
            {
                throw new FormatException($"error in expression, none or multiple member(s) detected: {text}");
            }

            return (LambdaExpressionSyntax)((FieldDeclarationSyntax)root.Members[0]).Declaration.Variables[0].Initializer.Value;
        }
    }
}
