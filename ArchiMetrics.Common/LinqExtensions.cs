// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinqExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;

	public static class LinqExtensions
	{
		public static IEnumerable<T> DistinctBy<T, TOut>(this IEnumerable<T> source, Func<T, TOut> func)
		{
			var comparer = new FuncComparer<T, TOut>(func);
			return source.Distinct(comparer);
		}

		public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> filter)
		{
			return source.Where(x => !filter(x));
		}

		public static IEnumerable<string> WhereNotNullOrWhitespace(this IEnumerable<string> source)
		{
			return source.Where(x => !string.IsNullOrWhiteSpace(x));
		}

		public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
		{
			return new Collection<T>(source.ToArray());
		}

		private class FuncComparer<T, TOut> : IEqualityComparer<T>
		{
			private readonly Func<T, TOut> _func;

			public FuncComparer(Func<T, TOut> func)
			{
				_func = func;
			}

			public bool Equals(T x, T y)
			{
				return _func(x).Equals(_func(y));
			}

			public int GetHashCode(T obj)
			{
				return _func(obj).GetHashCode();
			}
		}
	}
}