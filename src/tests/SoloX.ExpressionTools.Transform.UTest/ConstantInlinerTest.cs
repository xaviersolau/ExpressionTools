// ----------------------------------------------------------------------
// <copyright file="ConstantInlinerTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ExpressionTools.Transform.Impl;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace SoloX.ExpressionTools.Transform.UTest
{
    public class ConstantInlinerTest
    {
        [Fact]
        public void IsShouldConvertExternalDateVariableAsConst()
        {
            var inliner = new ConstantInliner();

            var externalValue = DateTime.Now;

            Expression<Func<DateTime, bool>> expToInline = i => i < externalValue;

            var exp = inliner.Amend(expToInline);

            externalValue = DateTime.Now.AddDays(10);

            var func = exp.Compile();

            Assert.False(func(DateTime.Now.AddDays(1)));
            Assert.True(func(DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void IsShouldConvertExternalVariableAsConst()
        {
            var inliner = new ConstantInliner();

            var externalValue = 0.01d;

            Expression<Func<double, double>> expToInline = i => i * externalValue;

            var exp = inliner.Amend(expToInline);

            externalValue = 0.1d;

            var func = exp.Compile();

            Assert.Equal(100d, func(10000));

            Expression<Func<double, double>> expectedExp = i => i * 0.01d;
            Assert.Equal(expectedExp.ToString(), exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExternalVariableAsConstInLambda()
        {
            var inliner = new ConstantInliner();

            var externalValue = 0.01d;

            LambdaExpression expToInline = (Expression<Func<double, double>>)(i => i * externalValue);

            var exp = inliner.Amend(expToInline);

            externalValue = 0.1d;

            var func = (Func<double, double>)exp.Compile();

            Assert.Equal(100d, func(10000));

            Expression<Func<double, double>> expectedExp = i => i * 0.01d;
            Assert.Equal(expectedExp.ToString(), exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithDateTime()
        {
            var inliner = new ConstantInliner();

            Expression<Func<DateTime, bool>> expToInline = d => d < new DateTime(2021, 12, 24);

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal("d => (d < new DateTime(2021, 12, 24))", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstDateTime()
        {
            var inliner = new ConstantInliner();

            var externalValue = new DateTime(2021, 12, 24);

            Expression<Func<DateTime, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal($"d => (d < new DateTime({externalValue.Ticks}))", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstDateTimeOffset()
        {
            var inliner = new ConstantInliner();

            var offset = new TimeSpan(2, 0, 0);
            var externalValue = new DateTimeOffset(2021, 12, 24, 10, 30, 12, offset);

            Expression<Func<DateTimeOffset, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal($"d => (d < new DateTimeOffset({externalValue.Ticks}, new TimeSpan({offset.Ticks})))", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithGuid()
        {
            var inliner = new ConstantInliner();

            var externalValue = Guid.NewGuid();

            Expression<Func<Guid, bool>> expToInline = d => d == externalValue;

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal($"d => (d == new Guid(\"{externalValue}\"))", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithNullableGuid()
        {
            var inliner = new ConstantInliner();

            Guid? externalValue = Guid.NewGuid();

            Expression<Func<Guid?, bool>> expToInline = d => d == null || d == externalValue;

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal($"d => ((d == null) || (d == ((Nullable<Guid>)(new Guid(\"{externalValue}\")))))", txt);
        }

        internal class TestModel
        {
            public int Property { get; set; }
        }

        [Fact]
        public void IsShouldConvertExpressionWithMemberAccess()
        {
            var inliner = new ConstantInliner();

            var externalModelValue = new TestModel() { Property = 123 };

            Expression<Func<int, bool>> expToInline = d => d < externalModelValue.Property;

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal($"d => (d < 123)", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstArrayOfInt()
        {
            var inliner = new ConstantInliner();

            var externalValue = new int[] { 1, 2, 3 };

            Expression<Func<int, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal("x => Enumerable.Contains<Int32>(new Int32[] { 1, 2, 3 }, x)", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstArrayOfString()
        {
            var inliner = new ConstantInliner();

            var externalValue = new string[] { "abc" };

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal("x => Enumerable.Contains<String>(new String[] { \"abc\" }, x)", txt);
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstEnumerableOfString()
        {
            var inliner = new ConstantInliner();

            var list = new string[] { "abc", "123" };
            var externalValue = list.Where(x => x == "abc").Select(x => x);

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);
            var txt = exp.Serialize();

            Assert.Equal("x => Enumerable.Contains<String>(new String[] { \"abc\" }, x)", txt);
        }
    }
}
