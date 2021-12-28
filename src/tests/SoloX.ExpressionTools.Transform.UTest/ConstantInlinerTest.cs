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
            Expression<Func<DateTime, bool>> exp = d => d < new DateTime(2021, 12, 24);

            Assert.Equal("d => (d < new DateTime(2021, 12, 24))", exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstDateTime()
        {
            var inliner = new ConstantInliner();

            var externalValue = new DateTime(2021, 12, 24);

            Expression<Func<DateTime, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);

            Assert.Equal($"d => (d < new DateTime({externalValue.Ticks}))", exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstDateTimeOffset()
        {
            var inliner = new ConstantInliner();

            var offset = new TimeSpan(2, 0, 0);
            var externalValue = new DateTimeOffset(2021, 12, 24, 10, 30, 12, offset);

            Expression<Func<DateTimeOffset, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);

            Assert.Equal($"d => (d < new DateTimeOffset({externalValue.Ticks}, new TimeSpan({offset.Ticks})))", exp.ToString());
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

            Assert.Equal($"d => (d < 123)", exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstArrayOfInt()
        {
            var inliner = new ConstantInliner();

            var externalValue = new int[] { 1, 2, 3 };

            Expression<Func<int, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            Assert.Equal("x => new [] {1, 2, 3}.Contains(x)", exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstArrayOfString()
        {
            var inliner = new ConstantInliner();

            var externalValue = new string[] { "abc" };

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            Assert.Equal("x => new [] {\"abc\"}.Contains(x)", exp.ToString());
        }

        [Fact]
        public void IsShouldConvertExpressionWithConstEnumerableOfString()
        {
            var inliner = new ConstantInliner();

            var list = new string[] { "abc", "123" };
            var externalValue = list.Where(x => x == "abc").Select(x => x);

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            Assert.Equal("x => new [] {\"abc\"}.Contains(x)", exp.ToString());
        }
    }
}
