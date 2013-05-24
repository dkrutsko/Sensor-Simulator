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
	using System.Windows;
	using System.Collections;
	using System.Collections.Generic;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents a graph containing sensors. </summary>

	public class Graph : IEnumerable
	{
		//----------------------------------------------------------------------------//
		// Classes                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Represents a node for use with the A* algorithm. </summary>

		protected class AStarNode : IComparable<AStarNode>
		{
			//----------------------------------------------------------------------------//
			// Fields                                                                     //
			//----------------------------------------------------------------------------//

			public Sensor    Sensor;
			public AStarNode Parent;

			public bool Visited;

			public float FScore;
			public float GScore;
			public float HScore;



			//----------------------------------------------------------------------------//
			// Constructors                                                               //
			//----------------------------------------------------------------------------//

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Creates a new AStarNode from a sensor. </summary>

			public AStarNode (Sensor sensor)
			{
				Sensor = sensor;
			}



			//----------------------------------------------------------------------------//
			// IComparable                                                                //
			//----------------------------------------------------------------------------//

			////////////////////////////////////////////////////////////////////////////////
			/// <summary> Compares the FScore with the specified value. </summary>

			public int CompareTo (AStarNode value)
			{
				return (int) (FScore - value.FScore);
			}
		}



		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private bool mRecomputeConnectionsNeeded = false;
		private LinkedList<Sensor> mSensors = new LinkedList<Sensor>();
		private Random mRandom = new Random();



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the number of sensors in this graph. </summary>

		public int Count		{ get { return mSensors.Count;		} }

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns whether this graph is empty. </summary>

		public bool IsEmpty		{ get { return mSensors.Count < 1;	} }



		//----------------------------------------------------------------------------//
		// Graph                                                                      //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Adds a sensor to this graph. </summary>

		public void Add (Sensor sensor)
		{
			mSensors.AddLast (sensor);
			mRecomputeConnectionsNeeded = true;
			sensor.PropertyChanged += delegate
				{ mRecomputeConnectionsNeeded = true; };
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Removes a sensor from this graph. </summary>

		public void Remove (Sensor sensor)
		{
			mSensors.Remove (sensor);
			mRecomputeConnectionsNeeded = true;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Clears this graph of all sensors. </summary>

		public void Clear()
		{
			mSensors.Clear();
		}



		//----------------------------------------------------------------------------//
		// Algorithms                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns a random sensor from this graph. </summary>

		public Sensor SelectRandom()
		{
			if (mSensors.Count == 0)
				return null;

			return mSensors.ElementAt (mRandom.Next (0, mSensors.Count));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns a sensor at the specified index. </summary>

		public Sensor SelectAt (int index)
		{
			if (index < 0 || index >= mSensors.Count)
				return null;

			return mSensors.ElementAt (index);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns at sensor close to the given values. </summary>

		public Sensor SensorAt (float x, float y, float radius)
		{
			return mSensors.FirstOrDefault
				(s => s.DistanceSquared (x, y) < radius * radius);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns all sensors inside the specified rectangle. </summary>

		public IEnumerable<Sensor> SensorsIn (float x, float y, float width, float height)
		{
			return mSensors.Where (s => (s.X > x) && (s.X < x + width ) &&
										(s.Y > y) && (s.Y < y + height));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns all sensors within the specified polygon.
		/// 		  Source: http://www.visibone.com/inpoly/ </summary>

		public IEnumerable<Sensor> SensorsIn (LinkedList<System.Windows.Point> points)
		{
			var result = new LinkedList<Sensor>();

			if (points.Count < 3)
				return result;

			Point p1, p2;
			bool inside;

			Point newPoint;
			Point oldPoint;

			foreach (Sensor sensor in mSensors)
			{
				inside   = false;
				oldPoint = points.Last.Value;
 
				foreach (Point point in points)
				{
					newPoint = point;

					if (newPoint.X > oldPoint.X)
					{
						p1 = oldPoint;
						p2 = newPoint;
					}

					else
					{
						p1 = newPoint;
						p2 = oldPoint;
					}

					if ((newPoint.X < sensor.X) == (sensor.X <= oldPoint.X) &&
						((long) sensor.Y - (long) p1.Y) * (long) (p2.X - p1.X) <
						((long) p2.Y - (long) p1.Y) * (long) (sensor.X - p1.X))
						inside = !inside;
 
					oldPoint = newPoint;
				}

				if (inside)
					result.AddLast (sensor);
			}

			return result;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Resolves all sensor connections. </summary>

		public void RecomputeConnections()
		{
			if (mRecomputeConnectionsNeeded)
			{
				foreach (Sensor sensor in mSensors)
				{
					sensor.RemoveAllConnections();
					foreach (Sensor s in mSensors)
						if (sensor != s && sensor.Contains(s))
							sensor.AddConnection(s);
				}

				mRecomputeConnectionsNeeded = false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Finds the shortest path given a source and target node. </summary>

		public Stack<Sensor> FindPath (Sensor source, Sensor target)
		{
			// Recompute Connections
			RecomputeConnections();

			// Matching sensors to AStarNodes
			var nodes = new Dictionary<Sensor, AStarNode>();
			nodes.Add (source, new AStarNode (source));

			// Create an open list with source
			var open = new List<AStarNode>();
			open.Add (nodes[source]);

			while (open.Count > 0)
			{
				// Sort open list by F score
				open.Sort();

				// Select the top element
				var current = open[0];
				open.RemoveAt (0);
				current.Visited = true;

				// The target was reached
				if (current.Sensor == target)
					return BuildPath (current);

				foreach (Sensor neighbour in current.Sensor.Connected)
				{
					// Create an AStarNode if needed
					if (!nodes.ContainsKey (neighbour))
						nodes.Add (neighbour, new AStarNode (neighbour));

					AStarNode neighbourNode = nodes[neighbour];

					// Neighbour visited
					if (neighbourNode.Visited)
						continue;

					// Calculate neighbours G and H Scores
					float neighbourGScore = current.GScore +
						current.Sensor.DistanceSquared (neighbour);

					if (!open.Contains (neighbourNode))
					{
						// Add new node
						open.Add (neighbourNode);

						neighbourNode.Parent = current;
						neighbourNode.GScore = neighbourGScore;
						neighbourNode.HScore = ComputeHScore (neighbour, target);
						neighbourNode.FScore = neighbourNode.GScore + neighbourNode.HScore;
					}

					else if (neighbourGScore < neighbourNode.GScore)
					{
						// Better node to add
						neighbourNode.Parent = current;
						neighbourNode.GScore = neighbourGScore;
						neighbourNode.FScore = neighbourNode.GScore + neighbourNode.HScore;
					}
				}
			}

			return new Stack<Sensor>();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Computes the HScore for the A* Algorithm. </summary>

		private float ComputeHScore (Sensor source, Sensor target)
		{
			return source.DistanceSquared (target);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Builds the path based on AStarNode parents. </summary>

		private Stack<Sensor> BuildPath (AStarNode target)
		{
			AStarNode current = target;
			var result = new Stack<Sensor>();

			// Reconstruct path
			while (current != null)
			{
				result.Push (current.Sensor);
				current = current.Parent;
			}

			// Ignore source node
			if (result.Count > 0)
				result.Pop();

			return result;
		}



		//----------------------------------------------------------------------------//
		// IEnumerable                                                                //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Returns the enumerator allowing this object
		/// 		  to be used with a foreach statement. </summary>

		public IEnumerator GetEnumerator()
		{
			return mSensors.GetEnumerator();
		}
	}
}
