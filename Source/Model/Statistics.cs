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
	/// <summary> Represents a class which can run statistical analysis. </summary>

	public class Statistics
	{
		//----------------------------------------------------------------------------//
		// Classes                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Represents a single data point. </summary>

		public class DataPoint
		{
			//----------------------------------------------------------------------------//
			// Properties                                                                 //
			//----------------------------------------------------------------------------//

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the ratio value. </summary>

			public double Ratio		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gers or sets the number of trials. </summary>

			public int Trials		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the number of sensors. </summary>

			public int Sensors		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the angle value. </summary>

			public int Angle		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the range value. </summary>

			public int Range		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the number of failed shortest paths. </summary>

			public int Missed		{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the average difference between AvgDir and AvgOmni. </summary>

			public double AvgDiff	{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the average directional graph result. </summary>

			public double AvgDir	{ get; set; }

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Gets or sets the average omnidirectional graph result. </summary>

			public double AvgOmni	{ get; set; }



			//----------------------------------------------------------------------------//
			// Constructors                                                               //
			//----------------------------------------------------------------------------//

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Creates a new DataPoint with default values. </summary>

			public DataPoint()
			{
				this.Ratio		= 0.0;

				this.Trials		= 0;
				this.Sensors	= 0;
				this.Angle		= 0;
				this.Range		= 0;

				this.Missed		= 0;
				this.AvgDiff	= 0.0;

				this.AvgDir		= 0.0;
				this.AvgOmni	= 0.0;
			}
		}



		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private bool mIsSuccess;
		private Random mRandom = new Random();
		private DataPoint[] mDataPoints;



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the the number of iterations to coduct. </summary>

		public int Iterations	{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the range of trials to conduct. </summary>

		public Range Trials		{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the range of sensors to use. </summary>

		public Range Sensors	{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the range of angles to use. </summary>

		public Range Angle		{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the range of distances to use. </summary>

		public Range Distance	{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the bounding width. </summary>

		public int Width		{ get; set; }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the bounding height. </summary>

		public int Height		{ get; set; }



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a new statistics object with the specified values. </summary>

		public Statistics(Range trials, Range sensors, Range angle, Range distance)
			: this (trials, sensors, angle, distance, 1024, 768) { }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a new statistics object with the specified values. </summary>

		public Statistics(Range trials, Range sensors,
			Range angle, Range distance, int width, int height)
		{
			Trials   = trials;
			Sensors  = sensors;
			Angle    = angle;
			Distance = distance;

			Width    = width;
			Height   = height;

			Iterations = 10;
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the specified data point. </summary>

		public DataPoint GetDataPoint (int iteration)
		{
			if (!mIsSuccess) return null;
			return mDataPoints[iteration];
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Runs the statistical analysis. </summary>

		public bool PerformTests()
		{
			mDataPoints = new DataPoint[Iterations];
			for (int i = 0; i < Iterations; ++i)
			{
				//----------------------------------------------------------------------------//
				// Initialize Iteration Data                                                  //
				//----------------------------------------------------------------------------//

				mDataPoints[i] = new DataPoint();

				Graph directional     = new Graph();
				Graph omnidirectional = new Graph();

				mDataPoints[i].Sensors = Sensors.Random();
				if (mDataPoints[i].Sensors <= 0 || mDataPoints[i].Sensors > 1000)
					return (mIsSuccess = false);

				mDataPoints[i].Angle = Angle.Random();
				mDataPoints[i].Range = Distance.Random();

				if (mDataPoints[i].Angle < 0 ||
					mDataPoints[i].Range < 0 ||
					mDataPoints[i].Range > 1000)
					return (mIsSuccess = false);



				//----------------------------------------------------------------------------//
				// Generate Sensors                                                           //
				//----------------------------------------------------------------------------//

				for (int s = 0; s < mDataPoints[i].Sensors; ++s)
				{
					float x = mRandom.Next (0, Width);
					float y = mRandom.Next (0, Height);
					float o = mRandom.Next (0, 360);

					if (o < 0) return (mIsSuccess = false);
					
					directional.Add (new Sensor (x, y, mDataPoints[i].Angle, mDataPoints[i].Range, o));
					omnidirectional.Add (new Sensor (x, y, 360, mDataPoints[i].Range, o));
				}



				//----------------------------------------------------------------------------//
				// Compute Connections                                                        //
				//----------------------------------------------------------------------------//

				directional.RecomputeConnections();
				omnidirectional.RecomputeConnections();



				//----------------------------------------------------------------------------//
				// Initialize Trial Data                                                      //
				//----------------------------------------------------------------------------//

				mDataPoints[i].Trials = Trials.Random();
				if (mDataPoints[i].Trials <= 0 || mDataPoints[i].Trials > 5000)
					return (mIsSuccess = false);



				//----------------------------------------------------------------------------//
				// Perform Trials                                                             //
				//----------------------------------------------------------------------------//

				for (int t = 0; t < mDataPoints[i].Trials; ++t)
				{
					int sourceIndex = mRandom.Next (0, directional.Count);
					int targetIndex = mRandom.Next (0, directional.Count);

					Sensor dSource = directional.SelectAt (sourceIndex);
					Sensor dTarget = directional.SelectAt (targetIndex);

					Sensor oSource = omnidirectional.SelectAt (sourceIndex);
					Sensor oTarget = omnidirectional.SelectAt (targetIndex);

					var dPath = directional.FindPath     (dSource, dTarget);
					var oPath = omnidirectional.FindPath (oSource, oTarget);

					if (dPath.Count == 0 || oPath.Count == 0)
						++mDataPoints[i].Missed;

					else
					{
						mDataPoints[i].Ratio += (dPath.Count / oPath.Count);

						mDataPoints[i].AvgDir  += dPath.Count;
						mDataPoints[i].AvgOmni += oPath.Count;
					}
				}



				//----------------------------------------------------------------------------//
				// Compute Final results                                                      //
				//----------------------------------------------------------------------------//

				mDataPoints[i].Ratio   /= mDataPoints[i].Trials;
				mDataPoints[i].AvgDir  /= mDataPoints[i].Trials;
				mDataPoints[i].AvgOmni /= mDataPoints[i].Trials;

				if (mDataPoints[i].AvgDir < mDataPoints[i].AvgOmni)
					mDataPoints[i].AvgOmni = mDataPoints[i].AvgDir;

				mDataPoints[i].AvgDiff  = mDataPoints[i].AvgDir - mDataPoints[i].AvgOmni;
			}

			return (mIsSuccess = true);
		}
	}
}
