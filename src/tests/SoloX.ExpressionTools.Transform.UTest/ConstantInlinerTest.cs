// ----------------------------------------------------------------------
// <copyright file="ConstantInlinerTest.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Shouldly;
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
        public void ItShouldConvertExternalDateVariableAsConst()
        {
            var inliner = new ConstantInliner();

            var externalValue = DateTime.Now;

            Expression<Func<DateTime, bool>> expToInline = i => i < externalValue;

            var exp = inliner.Amend(expToInline);

            externalValue = DateTime.Now.AddDays(10);

            var func = exp.Compile();

            func(DateTime.Now.AddDays(1)).ShouldBeFalse();
            func(DateTime.Now.AddDays(-1)).ShouldBeTrue();
        }

        [Fact]
        public void ItShouldConvertExternalVariableAsConst()
        {
            var inliner = new ConstantInliner();

            var externalValue = 0.01d;

            Expression<Func<double, double>> expToInline = i => i * externalValue;

            var exp = inliner.Amend(expToInline);

            externalValue = 0.1d;

            var func = exp.Compile();

            func(10000).ShouldBe(100d);

            Expression<Func<double, double>> expectedExp = i => i * 0.01d;

            exp.ToString().ShouldBe(expectedExp.ToString());
        }

        [Fact]
        public void ItShouldConvertExternalVariableAsConstInLambda()
        {
            var inliner = new ConstantInliner();

            var externalValue = 0.01d;

            LambdaExpression expToInline = (Expression<Func<double, double>>)(i => i * externalValue);

            var exp = inliner.Amend(expToInline);

            externalValue = 0.1d;

            var func = (Func<double, double>)exp.Compile();

            func(10000).ShouldBe(100d);

            Expression<Func<double, double>> expectedExp = i => i * 0.01d;
            exp.ToString().ShouldBe(expectedExp.ToString());
        }

        [Fact]
        public void ItShouldConvertExpressionWithDateTime()
        {
            var inliner = new ConstantInliner();

            Expression<Func<DateTime, bool>> expToInline = d => d < new DateTime(2021, 12, 24);

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(new DateTime(2020, 12, 24)).ShouldBeTrue();
            func(new DateTime(2022, 12, 24)).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe("d => (d < new DateTime(2021, 12, 24))");
        }

        [Fact]
        public void ItShouldConvertExpressionWithStaticMember()
        {
            var inliner = new ConstantInliner();

            Expression<Func<DateTime, bool>> expToInline = d => d < DateTime.Now.Date;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(DateTime.Now.Date.AddDays(-1)).ShouldBeTrue();
            func(DateTime.Now.Date.AddDays(1)).ShouldBeFalse();

            var txt = exp.Serialize();

            var expected = $"d => (d < new DateTime({DateTime.Now.Ticks}).Date)";

            txt.ShouldStartWith(expected.Substring(0, 30));
            txt.ShouldEndWith(").Date)");
        }

        [Fact]
        public void ItShouldConvertExpressionWithConstDateTime()
        {
            var inliner = new ConstantInliner();

            var externalValue = new DateTime(2021, 12, 24);

            Expression<Func<DateTime, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(new DateTime(2020, 12, 24)).ShouldBeTrue();
            func(new DateTime(2022, 12, 24)).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe($"d => (d < new DateTime({externalValue.Ticks}))");
        }

        [Fact]
        public void ItShouldConvertExpressionWithConstDateTimeOffset()
        {
            var inliner = new ConstantInliner();

            var offset = new TimeSpan(2, 0, 0);
            var externalValue = new DateTimeOffset(2021, 12, 24, 10, 30, 12, offset);

            Expression<Func<DateTimeOffset, bool>> expToInline = d => d < externalValue;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(externalValue.AddDays(-1)).ShouldBeTrue();
            func(externalValue.AddDays(1)).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe($"d => (d < new DateTimeOffset({externalValue.Ticks}, new TimeSpan({offset.Ticks})))");
        }

        [Fact]
        public void ItShouldConvertExpressionWithGuid()
        {
            var inliner = new ConstantInliner();

            var externalValue = Guid.NewGuid();

            Expression<Func<Guid, bool>> expToInline = d => d == externalValue;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(externalValue).ShouldBeTrue();
            func(Guid.NewGuid()).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe($"d => (d == new Guid(\"{externalValue}\"))");
        }

        [Fact]
        public void ItShouldConvertExpressionWithNullableGuid()
        {
            var inliner = new ConstantInliner();

            Guid? externalValue = Guid.NewGuid();

            Expression<Func<Guid?, bool>> expToInline = d => d == null || d == externalValue;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(externalValue).ShouldBeTrue();
            func(Guid.NewGuid()).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe($"d => ((d == null) || (d == ((Nullable<Guid>)(new Guid(\"{externalValue}\")))))");
        }

        internal sealed class TestModel
        {
            public int Property { get; set; }
        }

        [Fact]
        public void ItShouldConvertExpressionWithMemberAccess()
        {
            var inliner = new ConstantInliner();

            var externalModelValue = new TestModel() { Property = 123 };

            Expression<Func<int, bool>> expToInline = d => d < externalModelValue.Property;

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(124).ShouldBeFalse();
            func(122).ShouldBeTrue();

            var txt = exp.Serialize();

            txt.ShouldBe($"d => (d < 123)");
        }

        [Fact]
        public void ItShouldConvertExpressionWithConstArrayOfInt()
        {
            var inliner = new ConstantInliner();

            var externalValue = new int[] { 1, 2, 3 };

            Expression<Func<int, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func(0).ShouldBeFalse();
            func(1).ShouldBeTrue();
            func(2).ShouldBeTrue();
            func(3).ShouldBeTrue();
            func(4).ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe("x => new Int32[] { 1, 2, 3 }.Contains<Int32>(x)");
        }

        [Fact]
        public void ItShouldConvertExpressionWithConstArrayOfString()
        {
            var inliner = new ConstantInliner();

            var externalValue = new string[] { "abc" };

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func("abc").ShouldBeTrue();
            func("xyz").ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe("x => new String[] { \"abc\" }.Contains<String>(x)");
        }

        [Fact]
        public void ItShouldConvertExpressionWithConstEnumerableOfString()
        {
            var inliner = new ConstantInliner();

            var list = new string[] { "abc", "123" };
            var externalValue = list.Where(x => x == "abc").Select(x => x);

            Expression<Func<string, bool>> expToInline = x => externalValue.Contains(x);

            var exp = inliner.Amend(expToInline);

            var func = exp.Compile();

            func("abc").ShouldBeTrue();
            func("123").ShouldBeFalse();

            var txt = exp.Serialize();

            txt.ShouldBe("x => new String[] { \"abc\" }.Contains<String>(x)");
        }
    }
}
