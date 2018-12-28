using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SoloX.ExpressionTools.Parser.Visitor;

namespace SoloX.ExpressionTools.Parser
{
    /// <summary>
    /// Class to parse textual lambda expression
    /// </summary>
    public class ExpressionParser
    {
        private LambdaVisitor _visitor;

        public ExpressionParser(IParameterTypeResolver parameterTypeResolver, IMethodResolver methodResolver)
        {
            _visitor = new LambdaVisitor(parameterTypeResolver, methodResolver);
        }

        /// <summary>
        /// Parse the given lambda expression and build the corresponding System Linq Lambda expression
        /// </summary>
        /// <param name="lambdaExpressionText">The expression to parse</param>
        /// <returns>The expression tree built from the textual lambda expression</returns>
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
