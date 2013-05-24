////////////////////////////////////////////////////////////////////////////////
// -------------------------------------------------------------------------- //
//                                                                            //
//                          Copyright (C) 2011-2013                           //
//                            github.com/dkrutsko                             //
//                            github.com/AbsMechanik                          //
//                                                                            //
//                        See LICENSE.md for copyright                        //
//                                                                            //
// -------------------------------------------------------------------------- //
////////////////////////////////////////////////////////////////////////////////

namespace Simulator
{
	using System;
	using System.Collections.Generic;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Defines extension methods for various classes. </summary>

	public static class Extensions
	{
		//----------------------------------------------------------------------------//
		// Static                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Removes the specified items from the list. </summary>

		public static TInput Remove<TInput> (this LinkedList<TInput> list, Func<TInput, bool> predicate)
		{
			var node = list.First;
			while (node != null)
			{
				if (predicate (node.Value))
				{
					list.Remove (node);
					return node.Value;
				}

				node = node.Next;
			}

			return default (TInput);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Removes all occurences of a specified item from the list. </summary>

		public static int RemoveAll<TInput> (this LinkedList<TInput> list, Func<TInput, bool> predicate)
		{
			int count = 0;
			var node  = list.First;

			while (node != null)
			{
				var next = node.Next;
				if (predicate (node.Value))
				{
					list.Remove (node);
					++count;
				}

				node = next;
			}

			return count;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Does a foreach statement on an enumerable object. </summary>

		public static void ForEach<TInput> (this IEnumerable<TInput> enumeration, Action<TInput> action)
		{
			foreach (TInput item in enumeration)
				action (item);
		}
	}
}
