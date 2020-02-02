using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Data.Dtos;
using Core.Repository.Interfaces;
using Core.Repository.Query;
using Microsoft.AspNet.OData.Query;
using MongoDB.Driver.Linq;

namespace Core.Repository.Extensions
{
    public static class QueryHelpers
    {

        public static IQueryable<T> OrderByName<T>(this IQueryable<T> source,string propertyName,Boolean isDescending)
        {

            if (source == null) throw new ArgumentNullException("source");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            propertyName = propertyName.ToPascalCase();
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");

            PropertyInfo pi = type.GetProperty(propertyName);
            Expression expr = Expression.Property(arg, pi);
            type = pi.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            String methodName = isDescending ? "OrderByDescending" : "OrderBy";
            object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)result;
        }
        public static IMongoQueryable<T> OrderByName<T>(this IMongoQueryable<T> source, string propertyName, Boolean isDescending)
        {

            if (source == null) throw new ArgumentNullException("source");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            propertyName = propertyName.ToPascalCase();

            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");

            PropertyInfo pi = type.GetProperty(propertyName);
            Expression expr = Expression.Property(arg, pi);
            type = pi.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            String methodName = isDescending ? "OrderByDescending" : "OrderBy";
            object result = typeof(MongoQueryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });
            return (IMongoQueryable<T>)result;
        }
        /// <summary>
        /// Returns the list of items of type on which method called
        /// </summary>
        /// <typeparam name="TSource">This helper can be invoked on IEnumerable type.</typeparam>
        /// <param name="source">instance on which this helper is invoked.</param>
        /// <param name="sortingModal">Page no</param>
        /// <returns>List of items after query being executed on</returns>
        public static IEnumerable<TSource> QueryOptions<TSource>(
            this IQueryable<TSource> source,
            IQueryOptions queryOptions)
        {
            // Is there any sort column supplied?
            IEnumerable<TSource> data = source;
            if (!string.IsNullOrEmpty(queryOptions.Sort))
            {
                // Gets the coloumn name that sorting to be done on
                PropertyInfo propertyInfo =
                    data.GetType().GetGenericArguments()[0].GetProperty(queryOptions.Sort);

                // Define the sorting function
                if (!string.IsNullOrEmpty(queryOptions.SortOrder))
                {
                    data = queryOptions.SortOrder == SortOrder.Ascending
                        ? data.OrderBy(x => propertyInfo.GetValue(x, null))
                        : data.OrderByDescending(x => propertyInfo.GetValue(x, null));
                }
            }

            if (queryOptions.PageSize > 0 || queryOptions.RecordCount > 0)
            {
                // Apply paging to (sorted) data
                data = queryOptions.RecordCount > 0
                    ? data.Skip(queryOptions.StartRecord).Take(queryOptions.RecordCount)
                    : data.Skip((queryOptions.Page - 1) * queryOptions.PageSize).Take(queryOptions.PageSize);
            }
            return data;
        }
        public static IQueryable<TSource> QueryOptionsAsQueryable<TSource>(
            this IQueryable<TSource> source,
            IQueryOptions queryOptions)
        {
            // Is there any sort column supplied?
            IQueryable<TSource> data = source;
            if (!string.IsNullOrEmpty(queryOptions.Sort))
            {
                // Gets the coloumn name that sorting to be done on
                PropertyInfo propertyInfo =
                    data.GetType().GetGenericArguments()[0].GetProperty(queryOptions.Sort);

                // Define the sorting function
                if (!string.IsNullOrEmpty(queryOptions.SortOrder))
                {
                    data = queryOptions.SortOrder == SortOrder.Ascending
                        ? data.OrderBy(x => propertyInfo.GetValue(x, null))
                        : data.OrderByDescending(x => propertyInfo.GetValue(x, null));
                }
            }

            if (queryOptions.PageSize > 0 || queryOptions.RecordCount > 0)
            {
                // Apply paging to (sorted) data
                data = queryOptions.RecordCount > 0
                    ? data.Skip(queryOptions.StartRecord).Take(queryOptions.RecordCount)
                    : data.Skip((queryOptions.Page - 1) * queryOptions.PageSize).Take(queryOptions.PageSize);
            }


            return data;
        }
        public static IMongoQueryable<TSource> MongoQueryOptionsAsQueryable<TSource>(
            this IMongoQueryable<TSource> source,
            IQueryOptions queryOptions)
        {
            // Is there any sort column supplied?
            IMongoQueryable<TSource> data = source;
            if (!string.IsNullOrEmpty(queryOptions.Sort))
            {
                data = data.OrderByName(queryOptions.Sort, queryOptions.SortOrder == SortOrder.Descending);
            }

            if(queryOptions.PageSize > 0 || queryOptions.RecordCount>0)
            {
                // Apply paging to (sorted) data
                data = queryOptions.RecordCount > 0
                    ? data.Skip(queryOptions.StartRecord).Take(queryOptions.RecordCount)
                    : data.Skip((queryOptions.Page - 1) * queryOptions.PageSize).Take(queryOptions.PageSize);
            }

            return data;
        }

        public static Expression ToExpression<TPublicEntity>(this OrderByQueryOption filter) where TPublicEntity : BaseDto
        {
            if(filter == null)
                throw new ArgumentNullException(nameof(filter));

            IQueryable queryable = Enumerable.Empty<TPublicEntity>().AsQueryable();
            queryable = filter.ApplyTo(queryable, new ODataQuerySettings());
            return queryable.Expression;
        }
    }
}
