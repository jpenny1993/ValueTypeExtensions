namespace ValueType.Extensions.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class LinqExtensions
    {
        /// <summary>
        /// Applies a Left Outer Join to a collection.
        /// </summary>
        /// <typeparam name="TOuter">Outer collection type</typeparam>
        /// <typeparam name="TInner">Inner collection type</typeparam>
        /// <typeparam name="TKey">Foreign key type</typeparam>
        /// <typeparam name="TResult">Resultant entity type</typeparam>
        /// <param name="outerTable">The outer collection</param>
        /// <param name="innerTable">The inner collection</param>
        /// <param name="outerKey">The outer key property</param>
        /// <param name="innerkey">The inner key property</param>
        /// <param name="transformation">Expression to join the two entities</param>
        /// <returns>An enumerable of TResult</returns>
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outerTable,
                 IEnumerable<TInner> innerTable,
                 Func<TOuter, TKey> outerKey,
                 Func<TInner, TKey> innerkey,
                 Expression<Func<TOuter, TInner, TResult>> transformation)
        {
            return from outerValue in outerTable
                join innerValue in innerTable on outerKey.Invoke(outerValue)
                equals innerkey.Invoke(innerValue) into joinedInnerValue
                from result in joinedInnerValue.DefaultIfEmpty()
                select transformation.Compile().Invoke(outerValue, result);
        }
    }
}
