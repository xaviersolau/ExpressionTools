// ----------------------------------------------------------------------
// <copyright file="ConstantVisitor.cs" company="Xavier Solau">
// Copyright © 2019 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SoloX.ExpressionTools.Transform.Impl.Visitor
{
    /// <summary>
    /// Evaluate all constant node.
    /// </summary>
    internal class ConstantVisitor : ExpressionVisitor
    {
        /// <inheritdoc/>
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        /// <inheritdoc/>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.Member is FieldInfo fieldInfo)
            {
                var expNode = base.Visit(node.Expression);

                if (expNode is ConstantExpression constantExpression)
                {
                    var value = fieldInfo.GetValue(constantExpression.Value);

                    return BuildConstantExpression(fieldInfo.FieldType, value);
                }
                else
                {
                    return Expression.MakeMemberAccess(expNode, fieldInfo);
                }
            }
            else if (node.Member is PropertyInfo propertyInfo)
            {
                var expNode = base.Visit(node.Expression);

                if (expNode is ConstantExpression constantExpression)
                {
                    var value = propertyInfo.GetValue(constantExpression.Value);

                    return BuildConstantExpression(propertyInfo.PropertyType, value);
                }
                if (expNode == null)
                {
                    var value = propertyInfo.GetValue(null);

                    return BuildConstantExpression(propertyInfo.PropertyType, value);
                }
                else
                {
                    return Expression.MakeMemberAccess(expNode, propertyInfo);
                }
            }

            return base.VisitMember(node);
        }

        private static Expression BuildConstantExpression(Type type, object value)
        {
            if (type == typeof(DateTime))
            {
                var date = (DateTime)value;

                return Expression.New(typeof(DateTime).GetConstructor(new Type[] { typeof(long) }), Expression.Constant(date.Ticks));
            }
            else if (type == typeof(DateTimeOffset))
            {
                var date = (DateTimeOffset)value;

                return Expression.New(
                    typeof(DateTimeOffset).GetConstructor(new Type[] { typeof(long), typeof(TimeSpan) }),
                    Expression.Constant(date.Ticks),
                    Expression.New(
                        typeof(TimeSpan).GetConstructor(new Type[] { typeof(long) }),
                        Expression.Constant(date.Offset.Ticks)));
            }
            else if (type == typeof(Guid))
            {
                var guid = (Guid)value;

                return Expression.New(
                    typeof(Guid).GetConstructor(new Type[] { typeof(string) }),
                    Expression.Constant(guid.ToString()));
            }
            else if (type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);

                var hasValueProperty = type.GetProperty(nameof(Nullable<int>.HasValue));

                if ((bool)hasValueProperty.GetValue(value))
                {
                    var valueProperty = type.GetProperty(nameof(Nullable<int>.Value));
                    return Expression.Convert(
                        BuildConstantExpression(underlyingType, valueProperty.GetValue(value)),
                        type);
                }
                else
                {
                    return Expression.Default(type);
                }
            }
            else if (type != typeof(string))
            {
                var enumerable = type.Name == typeof(IEnumerable<>).Name
                    ? type
                    : type.GetTypeInfo().GetInterface(typeof(IEnumerable<>).Name);

                if (enumerable != null)
                {
                    var itemType = enumerable.GetGenericArguments()[0];

                    return Expression.NewArrayInit(itemType, Tools.BuildConstantExpressionList(itemType, value));
                }
            }

            return Expression.Constant(value);
        }

        private static class Tools
        {
            public static IEnumerable<Expression> BuildConstantExpressionList(Type itemType, object list)
            {
                var methodGeneric = typeof(Tools).GetMethod(nameof(Tools.GenericBuildConstantExpressionList), new Type[] { typeof(object) });

                var method = methodGeneric.MakeGenericMethod(itemType);

                return (IEnumerable<Expression>)method.Invoke(null, new object[] { list });
            }

            public static IEnumerable<Expression> GenericBuildConstantExpressionList<T>(object list)
            {
                var items = (IEnumerable<T>)list;

                return items.Select(i => BuildConstantExpression(typeof(T), i));
            }
        }
    }
}
