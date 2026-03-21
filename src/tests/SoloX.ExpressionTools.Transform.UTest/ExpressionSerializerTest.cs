// ----------------------------------------------------------------------
// <copyright file="ExpressionSerializerTest.cs" company="Xavier Solau">
// Copyright © 2019-2026 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Transform.Impl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest
{
    public class ExpressionSerializerTest
    {
        [Fact]
        public void ItShouldSerializeBasicExpression()
        {
            Expression<Func<double, bool>> expression = d => d > 10;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => (d > 10)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithNulableType()
        {
            Expression<Func<Guid?, bool>> expression = d => d == null || d == new Guid("f45132ed-e1cf-4ddf-b8f9-62e660d2b4cb");

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => ((d == null) || (d == ((Nullable<Guid>)(new Guid(\"f45132ed-e1cf-4ddf-b8f9-62e660d2b4cb\")))))");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithPropertyAccess()
        {
            Expression<Func<IObjectWithProperty, bool>> expression = d => d.MyInt % 2 == 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => ((d.MyInt % 2) == 0)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithArrayIndexAccess()
        {
            Expression<Func<IObjectWithProperty, bool>> expression = d => d.MyArray.Length > 0 && d.MyArray[0] % 2 == 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => ((((d.MyArray).Length) > 0) && ((d.MyArray[0] % 2) == 0))");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithCascadingMemberAccess()
        {
            Expression<Func<IData1, bool>> expression = d => d.Data2.Data3 != null;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => (d.Data2.Data3 != null)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithTypedParameters()
        {
            LambdaExpression expression = (int d) => d != 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => (d != 0)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithConditional()
        {
            Expression<Func<int, bool>> expression = d => d > 0 ? true : false;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("d => ((d > 0) ? true : false)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithMathPow()
        {
            Expression<Func<double, double, double>> expression = (a, b) => Math.Pow(a, b);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("(a, b) => Math.Pow(a, b)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithDecimalOperations()
        {
            Expression<Func<double, double, double, double>> expression = (a, b, c) => a * b / c + -a * c;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("(a, b, c) => (((a * b) / c) + ((-(a)) * c))");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfStaticCultureInfo()
        {
            Expression<Func<string, string>> expression = p => p.ToUpper(CultureInfo.InvariantCulture);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("p => p.ToUpper(CultureInfo.InvariantCulture)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfContainsChar()
        {
            Expression<Func<string, bool>> expression = p => p.Contains('j');

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("p => p.Contains('j')");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfContainsString()
        {
            Expression<Func<string, bool>> expression = p => p.Contains("abc");

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("p => p.Contains(\"abc\")");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfIntArray()
        {
            Expression<Func<int, bool>> expression = i => new int[] { 1, 2, 3 }.Contains(i);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe("i => new Int32[] { 1, 2, 3 }.Contains<Int32>(i)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfStringArray()
        {
            Expression<Func<string, bool>> expression = i => new string[] { "a", "b" }.Contains(i);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe(@"i => new String[] { ""a"", ""b"" }.Contains<String>(i)");
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfStringArrayAndExplicitConvertor()
        {
            Expression<Func<string, bool>> expression = i => ((ICollection<string>)(new string[] { "a", "b" })).Contains(i);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            txt.ShouldBe(@"i => ((ICollection<String>)(new String[] { ""a"", ""b"" })).Contains(i)");
        }
    }
}
