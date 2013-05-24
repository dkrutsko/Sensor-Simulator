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
	/// <summary> Represents a Sensor object. </summary>

	public class Sensor : IEquatable<Sensor>, IComparable<Sensor>
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		protected float mX;
		protected float mY;

		protected float mAngle;
		protected float mRange;
		protected float mOrient;
		
		protected bool mMouseOver;
		protected bool mSelected;

		protected LinkedList<Sensor> mConnected = new LinkedList<Sensor>();

		// Property identifiers
		public const byte AngleProperty  = 3;
		public const byte RangeProperty  = 4;
		public const byte OrientProperty = 5;



		//----------------------------------------------------------------------------//
		// Events                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Invoked whenever a property is changed. </summary>

		public event Action<Sensor> PropertyChanged		= delegate { };

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Invoked if the mouse over state is changed. </summary>

		public event Action<Sensor> MouseOver			= delegate { };

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Invoked if the select state is changed. </summary>

		public event Action<Sensor> Selected			= delegate { };



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor X coordinate. </summary>

		public float X
		{
			get
			{
				return mX;
			}

			set
			{
				if (mX != value)
				{
					mX = value;
					PropertyChanged (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor Y coordinate. </summary>

		public float Y
		{
			get
			{
				return mY;
			}

			set
			{
				if (mY != value)
				{
					mY = value;
					PropertyChanged (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor angle value. </summary>

		public float Angle
		{
			get
			{
				return mAngle;
			}

			set
			{
				if (mAngle != value)
				{
						 if (value < 0  ) mAngle = 0;
					else if (value > 360) mAngle = 360;
					else mAngle = value;
					
					PropertyChanged (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor range value. </summary>

		public float Range
		{
			get
			{
				return mRange;
			}
			
			set
			{
				if (mRange != value)
				{
						 if (value < 0   ) mRange = 0;
					else if (value > 1000) mRange = 1000;
					else mRange = value;

					PropertyChanged (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor orientation value. </summary>

		public float Orientation
		{
			get
			{
				return mOrient;
			}

			set
			{
				if (mOrient != value)
				{
					mOrient = (value %= 360) < 0 ? value + 360 : value;
					PropertyChanged (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor mouse over state. </summary>

		public bool IsMouseOver
		{
			get
			{
				return mMouseOver;
			}

			set
			{
				if (mMouseOver != value)
				{
					mMouseOver = value;
					MouseOver (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the sensor selection state. </summary>

		public bool IsSelected
		{
			get
			{
				return mSelected;
			}

			set
			{
				if (mSelected != value)
				{
					mSelected = value;
					Selected (this);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the sensor connection list. </summary>

		public IEnumerable<Sensor> Connected
		{
			get { return mConnected; }
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the SensorControl object belonging to this sensor. </summary>

		public SensorControl Control { get; set; }



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates an empty sensor. </summary>

		public Sensor() { }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a sensor at coordinate x and y. </summary>

		public Sensor (float x, float y)
		{
			X = x;
			Y = y;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a sensor with the specified attributes. </summary>

		public Sensor (float x, float y, float angle, float range, float orientation)
		{
			X = x;
			Y = y;

			Angle = angle;
			Range = range;

			Orientation = orientation;
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns a specified property. </summary>

		public float? GetProperty (byte property)
		{
			if (property == AngleProperty)
				return Angle;

			else if (property == RangeProperty)
				return Range;

			else if (property == OrientProperty)
				return Orientation;

			else return null;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Sets the specified property. </summary>

		public void SetProperty (byte property, float value)
		{
			if (property == AngleProperty)
				Angle = value;

			else if (property == RangeProperty)
				Range = value;

			else if (property == OrientProperty)
				Orientation = value;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the distance between this
		/// 		  sensor and the specified value. </summary>

		public float Distance (Sensor value)
		{
			float dx = value.mX - mX;
			float dy = value.mY - mY;

			return (float) Math.Sqrt (dx * dx + dy * dy);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the distance between this
		/// 		  sensor and the specified coordinate. </summary>

		public float Distance (float x, float y)
		{
			float dx = x - mX;
			float dy = y - mY;

			return (float) Math.Sqrt (dx * dx + dy * dy);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the distance squared between
		/// 		  this sensor and the specified value. </summary>

		public float DistanceSquared (Sensor value)
		{
			float dx = value.mX - mX;
			float dy = value.mY - mY;

			return dx * dx + dy * dy;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the distance squared between this
		/// 		  sensor and the specified coordinate. </summary>

		public float DistanceSquared (float x, float y)
		{
			float dx = x - mX;
			float dy = y - mY;

			return dx * dx + dy * dy;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the angle between this
		/// 		  sensor and the specified value. </summary>

		public float AngleBetween (Sensor sensor)
		{
			float angle = (float) (57.2957795 *
				Math.Atan2 (sensor.mY - mY, sensor.mX - mX)) % 360;

			return angle < 0 ? angle + 360 : angle;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the angle between this
		/// 		  sensor and the specified coordinate. </summary>

		public float AngleBetween (float x, float y)
		{
			float angle = (float) (57.2957795 *
				Math.Atan2 (y - mY, x - mX)) % 360;

			return angle < 0 ? angle + 360 : angle;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Determines whether this sensor
		/// 		  contains the specified value. </summary>

		public bool Contains (Sensor sensor)
		{
			if (DistanceSquared (sensor) > mRange * mRange || mAngle == 0)
				return false;

			if (mAngle == 360)
				return true;

			float angle    = AngleBetween (sensor);
			float minAngle = (mOrient - (mAngle * 0.5f) + 360) % 360;
			float maxAngle = (mOrient + (mAngle * 0.5f) + 360) % 360;

			if (minAngle < maxAngle)
				 return minAngle <= angle && angle <= maxAngle;
			else return minAngle <= angle || angle <= maxAngle;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Determines whether this sensor
		/// 		  contains the specified coordinate. </summary>

		public bool Contains (float x, float y)
		{
			if (DistanceSquared (x, y) > mRange * mRange || mAngle == 0)
				return false;

			if (mAngle == 360)
				return true;

			float angle    = AngleBetween (x, y);
			float minAngle = (mOrient - (mAngle * 0.5f) + 360) % 360;
			float maxAngle = (mOrient + (mAngle * 0.5f) + 360) % 360;

			if (minAngle < maxAngle)
				 return minAngle <= angle && angle <= maxAngle;
			else return minAngle <= angle || angle <= maxAngle;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Adds a connection between this
		/// 		  sensor and the specified value. </summary>

		public void AddConnection (Sensor sensor)
		{
			if (!mConnected.Contains (sensor))
			{
				mConnected.AddLast (sensor);

				if (Control != null)
					Control.AddConnection (this, sensor);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Removes all connections. </summary>

		public void RemoveAllConnections()
		{
			mConnected.Clear();

			if (Control != null)
				Control.RemoveAllConnections();
		}



		//----------------------------------------------------------------------------//
		// Object                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the string representation of this sensor. </summary>

		public override string ToString()
		{
			return string.Format ("({0:f}, {1:f})", mX, mY);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Tests whether this sensor equals another. </summary>

		public bool Equals (Sensor value)
		{
			return value == null ? false :
				value.mX == mX && value.mRange == mRange &&
				value.mY == mY && value.mAngle == mAngle &&
				value.mOrient == mOrient;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Lexographically compares this sensor to another. </summary>

		public int CompareTo (Sensor value)
		{
			if (mX <  value.mX)
				return -1;

			if (mX == value.mX)
			{
				if (mY <  value.mY) return -1;
				if (mY == value.mY) return  0;
			}

			return 1;
		}

		////////////////////////////////////////////////////////////////////////////////

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Tests whether this sensor equals the specified object. </summary>

		public override bool Equals (object value)
		{
			return Equals (value as Sensor);
		}
	}
}
