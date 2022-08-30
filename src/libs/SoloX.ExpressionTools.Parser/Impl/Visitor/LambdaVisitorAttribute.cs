// ----------------------------------------------------------------------
// <copyright file="LambdaVisitorAttribute.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SoloX.ExpressionTools.Parser.Impl.Visitor
{
    /// <summary>
    /// LambdaVisitor attribute.
    /// </summary>
    internal class LambdaVisitorAttribute
    {
        public LambdaVisitorAttribute(LambdaVisitorAttribute attribute)
        {
            if (attribute != null)
            {
                this.Parameters = attribute.Parameters;
                this.ParameterMap = attribute.ParameterMap;
            }
            else
            {
                this.Parameters = new List<ParameterExpression>();
                this.ParameterMap = new Dictionary<string, ParameterExpression>();
            }
        }

        public Type[] ArgumentTypes { get; internal set; }
        public Type[] GenericParameters { get; internal set; }

        public Expression ResultingExpression { get; internal set; }

        public Type ResultingType { get; internal set; }

        public MethodInfo ResultingMethodInfo { get; internal set; }

        public string ResultingIdentifier { get; internal set; }

        public List<ParameterExpression> Parameters { get; }

        public Dictionary<string, ParameterExpression> ParameterMap { get; }

        public string QualifiedNamePrefix { get; internal set; } = string.Empty;
    }
}
