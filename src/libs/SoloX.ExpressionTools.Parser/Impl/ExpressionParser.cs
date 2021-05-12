// ----------------------------------------------------------------------
// <copyright file="ExpressionParser.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SoloX.ExpressionTools.Parser.Impl.Visitor;

namespace SoloX.ExpressionTools.Parser.Impl
{
    /// <inheritdoc />
    public class ExpressionParser : IExpressionParser
    {
        private readonly LambdaVisitor visitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParser"/> class with a parameter
        /// type resolver and a method resolver.
        /// </summary>
        /// <param name="parameterTypeResolver">Resolver that will be used to associate a Type to a given parameter name.</param>
        /// <param name="methodResolver">Resolver that will be used to identify a method given a name and an argument type list.</param>
        /// <param name="typeNameResolver">Resolver that will be used to identify a type given a name.</param>
        public ExpressionParser(IParameterTypeResolver parameterTypeResolver = null, IMethodResolver methodResolver = null, ITypeNameResolver typeNameResolver = null)
        {
            this.visitor = new LambdaVisitor(parameterTypeResolver, methodResolver, typeNameResolver);
        }

        /// <inheritdoc />
        public Expression<TDelegate> Parse<TDelegate>(string lambdaExpressionText)
        {
            var stree = GetLambdaSyntaxNode(lambdaExpressionText);

            var expRes = this.visitor.Visit(stree).ResultingExpression;
            return (Expression<TDelegate>)expRes;
        }

        /// <inheritdoc />
        public LambdaExpression Parse(string lambdaExpressionText)
        {
            var stree = GetLambdaSyntaxNode(lambdaExpressionText);

            var expRes = this.visitor.Visit(stree).ResultingExpression;
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

            var node = (GlobalStatementSyntax)root.Members[0];

            var decl = (LocalDeclarationStatementSyntax)node.Statement;

            return (LambdaExpressionSyntax)decl.Declaration.Variables[0].Initializer.Value;
        }
    }
}
