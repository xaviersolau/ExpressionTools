using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    internal class TypeVisitor : CSharpSyntaxVisitor<Type>
    {
        private static readonly IReadOnlyDictionary<string, Type> PredefinedTypeMap = new Dictionary<string, Type>()
        {
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "short", typeof(short) },
            { "int", typeof(int) },
            { "long", typeof(long) },
            { "string", typeof(String) },
        };

        public override Type VisitPredefinedType(PredefinedTypeSyntax node)
        {
            return PredefinedTypeMap[node.Keyword.Text];
        }
    }
}
