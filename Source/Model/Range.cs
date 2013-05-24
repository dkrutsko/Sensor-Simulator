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
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents a range value (e.g. 1-5, 7, 11-13) </summary>

	public class Range : IEquatable<Range>, ICloneable
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private Random mRandom;
		private List<int> mMinimums;
		private List<int> mMaximums;



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		public bool IsEmpty
		{
			get { return mMinimums.Count == 0 || mMaximums.Count == 0; }
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a range with default values. </summary>

		public Range()
		{
			mRandom   = new Random();
			mMinimums = new List<int>();
			mMaximums = new List<int>();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Copies the specified value. </summary>

		public Range (Range value) : this()
		{
			for (int i = 0; i < value.mMinimums.Count; ++i)
			{
				mMinimums.Add (value.mMinimums[i]);
				mMaximums.Add (value.mMaximums[i]);
			}
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Resets this ranges' parameters. </summary>

		public void Reset()
		{
			mMinimums.Clear();
			mMaximums.Clear();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Parses a string and stores the result. </summary>

		public bool Parse (string text)
		{
			if (text == null)
				throw new ArgumentNullException ("Text cannot be null.");

			mMinimums.Clear();
			mMaximums.Clear();

			if (text.Length == 0)
			{
				mMinimums.Add (0);
				mMaximums.Add (0);
				return true;
			}

			int value1;
			int value2;

			string[] values = text.Split (',');
			foreach (string s in values)
			{
				// Try and get a number
				if (int.TryParse (s, out value1))
				{
					mMinimums.Add (value1);
					mMaximums.Add (value1);
				}

				else
				{
					// We might have a range
					string[] ranges = s.Split ('-');

					// Try and get range values
					if (ranges.Length > 1 &&
						int.TryParse (ranges[0], out value1) &&
						int.TryParse (ranges[1], out value2) && value2 > value1)
					{
						mMinimums.Add (value1);
						mMaximums.Add (value2);
					}

					else
					{
						// Format error encountered
						mMinimums.Clear();
						mMaximums.Clear();
						return false;
					}
				}
			}

			return true;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns a random value from the parsed string. </summary>

		public int Random()
		{
			if (IsEmpty)
				throw new Exception ("The range is empty");

			int random = mRandom.Next (mMinimums.Count);
			return mRandom.Next (mMinimums[random],
								 mMaximums[random] + 1);
		}



		//----------------------------------------------------------------------------//
		// Object                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns a string representation of this class. </summary>

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < mMinimums.Count; ++i)
				result.Append (mMinimums[i] + "-" + mMaximums[i] + ", ");

			if (result.Length > 0)
				result.ToString().Substring (0, result.Length - 2);
			
			return null;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Tests if this class equals another. </summary>

		public bool Equals (Range value)
		{
			return value == null ? false :
				mMinimums.SequenceEqual (value.mMinimums) &&
				mMaximums.SequenceEqual (value.mMaximums);
		}

		////////////////////////////////////////////////////////////////////////////////

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Tests if this class equals the specified object. </summary>

		public override bool Equals (object value)
		{
			return Equals (value as Range);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Return a copy of this class. </summary>

		public virtual object Clone()
		{
			return new Range (this);
		}
	}
}
