// ----------------------------------------------------------------------
// <copyright file="ExpressionSerializerTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Sample;
using SoloX.ExpressionTools.Transform.Impl;
using System;
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

            Assert.Equal("d => (d > 10)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithNulableType()
        {
            Expression<Func<Guid?, bool>> expression = d => d == null || d == new Guid("f45132ed-e1cf-4ddf-b8f9-62e660d2b4cb");

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => ((d == null) || (d == ((Nullable<Guid>)(new Guid(\"f45132ed-e1cf-4ddf-b8f9-62e660d2b4cb\")))))", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithPropertyAccess()
        {
            Expression<Func<IObjectWithProperty, bool>> expression = d => d.MyInt % 2 == 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => ((d.MyInt % 2) == 0)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithArrayIndexAccess()
        {
            Expression<Func<IObjectWithProperty, bool>> expression = d => d.MyArray.Length > 0 && d.MyArray[0] % 2 == 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => ((((d.MyArray).Length) > 0) && ((d.MyArray[0] % 2) == 0))", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithCascadingMemberAccess()
        {
            Expression<Func<IData1, bool>> expression = d => d.Data2.Data3 != null;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => (d.Data2.Data3 != null)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithTypedParameters()
        {
            LambdaExpression expression = (int d) => d != 0;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => (d != 0)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithConditional()
        {
            Expression<Func<int, bool>> expression = d => d > 0 ? true : false;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("d => ((d > 0) ? true : false)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithMathPow()
        {
            Expression<Func<double, double, double>> expression = (a, b) => Math.Pow(a, b);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("(a, b) => Math.Pow(a, b)", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithDecimalOperations()
        {
            Expression<Func<double, double, double, double>> expression = (a, b, c) => a * b / c + -a * c;

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("(a, b, c) => (((a * b) / c) + ((-(a)) * c))", txt);
        }

        [Fact]
        public void ItShouldSerializeExpressionWithUseOfStaticCultureInfo()
        {
            Expression<Func<string, string>> expression = p => p.ToUpper(CultureInfo.InvariantCulture);

            var serializer = new ExpressionSerializer();

            var txt = serializer.Serialize(expression);

            Assert.Equal("p => p.ToUpper(CultureInfo.InvariantCulture)", txt);
        }
    }
}
