﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GovITHub.Auth.Common.Data
{
    public static class LinqExtensions
    {
        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name == name);
            if (matchedProperty == null)
                throw new ArgumentException("name");

            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            if (propInfo == null)
                throw new ArgumentException("name");
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            if (propInfo == null)
                throw new ArgumentException("name");
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            if (propInfo == null)
                throw new ArgumentException("name");
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string name)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            if (propInfo == null)
                throw new ArgumentException("name");
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            var orderedEnumberable = (System.Linq.IOrderedEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
            return (IQueryable<T>)orderedEnumberable.AsQueryable<T>();
        }
    }

    public static class StringExtensions
    {
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
            }

            return string.Join(" ", tokens);
        }
    }
}
