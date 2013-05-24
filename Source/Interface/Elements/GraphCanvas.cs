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
	using System.Windows.Data;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Shapes;
	using System.Windows.Controls;
	using System.Collections.Generic;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents the graph which can be manipulated. </summary>

	public class GraphCanvas : Canvas
	{
		//----------------------------------------------------------------------------//
		// Events                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Invoked when a selected sensor property has changed. </summary>

		public event Action<Sensor> SelectionPropertyChanged = delegate { };



		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private bool mGraphView;				// Graph view mode
		private bool mShortestPath;				// Shortest path mode

		private Sensor mSource;					// Pathfinding source sensor
		private Sensor mTarget;					// Pathfinding target sensor
		private Canvas mPathCanvas;				// Canvas of the shortest path
		private Brush mPathBrush;				// Color of the shortest path
		private bool mInterpolatePathColor;		// Interpolate color of path

		private Sensor mMouseOver;				// Sensor currently over

		private bool mModifyingRange;			// Whether modifying range
		private bool mModifyingAngle;			// Whether modifying angle
		private User32.POINT mStartMousePos;	// Starting mouse position

		private LinkedList<Sensor> mSelected;	// Sensors currently selected
		private Sensor mLastSelected;			// Last selected sensor

		private Point mInitialMousePos;			// Initial mouse position
		private Point mCurrentMousePos;			// Current mouse position

		private Canvas mLassoCanvas;			// Lasso selection view
		private LinkedList<Point> mSelection;	// Lasso selection model
		private Brush mLassoBrush;				// Color of the lasso

		private Graph  mGraph  = new Graph ();	// Graph  data pointer
		private Random mRandom = new Random();	// Random data pointer

		private Brush mSourceBrush;				// Start sensor brush
		private Brush mTargetBrush;				// Target sensor brush



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public int CountSelected	{ get { return mSelected.Count; }	}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public bool IsModifying		{ get; set;							}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public bool IsSelecting
		{
			get { return mLassoCanvas.Visibility == Visibility.Visible;	}
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public GraphCanvas()
		{
			// Create brushes
			mSourceBrush = new SolidColorBrush (Color.FromArgb (255, 32, 132, 32));
			mTargetBrush = new SolidColorBrush (Color.FromArgb (255, 132, 32, 32));

			// Create selection containers
			mSelected = new LinkedList<Sensor>();

			// Initialize lasso selection
			mLassoCanvas = new Canvas();
			Children.Add (mLassoCanvas);
			mSelection = new LinkedList<Point>();

			mLassoCanvas.Visibility = Visibility.Collapsed;

			Canvas.SetLeft (mLassoCanvas, 0);
			Canvas.SetTop  (mLassoCanvas, 0);
			Canvas.SetZIndex (mLassoCanvas, 1);

			Binding wBinding = new Binding ("Width" ); wBinding.Source = this;
			Binding hBinding = new Binding ("Height"); hBinding.Source = this;
			mLassoCanvas.SetBinding (Canvas.WidthProperty,  wBinding);
			mLassoCanvas.SetBinding (Canvas.HeightProperty, hBinding);

			mLassoBrush = new SolidColorBrush (Color.FromArgb (164, 255, 255, 255));

			// Initialize path canvas
			mPathCanvas = new Canvas();
			Children.Add (mPathCanvas);

			Canvas.SetLeft (mPathCanvas, 0);
			Canvas.SetTop  (mPathCanvas, 0);
			Canvas.SetZIndex (mPathCanvas, 1);

			mPathCanvas.SetBinding (Canvas.WidthProperty,  wBinding);
			mPathCanvas.SetBinding (Canvas.HeightProperty, hBinding);

			mPathBrush = new SolidColorBrush (Color.FromArgb (192, 255, 255, 255));

			// Subscribe to events
			MouseUp	  += ActionMouseUp;
			MouseDown += ActionMouseDown;
			MouseMove += ActionMouseMove;
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public float? GetSelectedProperty (byte property)
		{
			float? result = null;

			foreach (Sensor sensor in mSelected)
			{
				if (result == null)
					result = sensor.GetProperty (property);

				else if (result != sensor.GetProperty (property))
					return null;
			}

			return result;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public float? SetSelectedProperty (byte property, float value)
		{
			foreach (Sensor sensor in mSelected)
				sensor.SetProperty (property, value);

			if (mSelected.Count > 0)
				return mSelected.First.Value.GetProperty (property);

			return null;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void RandomPathSource()
		{
			ComputeShortestPath (mGraph.SelectRandom(), false);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void RandomPathTarget()
		{
			ComputeShortestPath (mGraph.SelectRandom(), true);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ReverseShortestPath()
		{
			Sensor temp = mTarget;
			ComputeShortestPath (mSource, true);
			ComputeShortestPath (temp, false);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void SelectAll()
		{
			foreach (Sensor sensor in mGraph)
			{
				if (!sensor.IsSelected)
					Select (sensor);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void Clear()
		{
			Children.Clear();
			mSelected.Clear();
			mGraph.Clear();

			Children.Add (mLassoCanvas);
			Children.Add (mPathCanvas );

			GC.Collect();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ClearSelection()
		{
			foreach (Sensor sensor in mSelected)
			{
				Children.Remove (sensor.Control);

				sensor.Control = null;
				mGraph.Remove (sensor);
			}

			mSelected.Clear();
			GC.Collect();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ToggleGraphView (bool value)
		{
			if (mGraphView = value)
				mGraph.RecomputeConnections();

			foreach (Sensor sensor in mGraph)
				sensor.Control.ToggleGraphView (sensor, value);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ToggleShortestPath (bool value)
		{
			if (mShortestPath = value)
				Select (null);

			else
			{
				mSource = null;
				mTarget = null;
				mPathCanvas.Children.Clear();
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public bool Randomize (Range sensors, Range angle, Range orient, Range range, bool constrain)
		{
			int amount = sensors.Random();
			if (amount <= 0 || amount > 1000)
				return false;

			int viewX1 = 0;
			int viewY1 = 0;
			int viewX2 = (int) ActualWidth;
			int viewY2 = (int) ActualHeight;

			if (constrain)
			{
				var parent = (ScrollViewer) Parent;

				viewX1 = (int) parent.ContentHorizontalOffset;
				viewY1 = (int) parent.ContentVerticalOffset;
				viewX2 = (int) parent.ActualWidth  + viewX1;
				viewY2 = (int) parent.ActualHeight + viewY1;
			}

			for (int i = 0; i < amount; ++i)
			{
				float x = mRandom.Next (viewX1 + 5, viewX2 - 5);
				float y = mRandom.Next (viewY1 + 5, viewY2 - 5);

				float a = angle.Random();
				float r = range.Random();
				float o = orient.Random();

				if (a < 0 || r < 0 || o < 0 || r > 1000)
					return false;

				AddSensor (new Sensor (x, y, a, r, o));
			}

			return true;
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseDown (object sender, MouseButtonEventArgs args)
		{
			Point position = args.GetPosition (this);

			bool leftButton  = args.ChangedButton == MouseButton.Left;
			bool rightButton = args.ChangedButton == MouseButton.Right;

			mInitialMousePos = position;
			mCurrentMousePos = position;

			Mouse.Capture (this);
			float x = (float) position.X;
			float y = (float) position.Y;



			//----------------------------------------------------------------------------//
			// Lasso Select                                                               //
			//----------------------------------------------------------------------------//

			if (leftButton && mMouseOver == null &&
				args.ClickCount != 2 && !mShortestPath)
				mLassoCanvas.Visibility = Visibility.Visible;



			//----------------------------------------------------------------------------//
			// Create                                                                     //
			//----------------------------------------------------------------------------//

			if (leftButton && mMouseOver == null &&
				args.ClickCount == 2 && !mShortestPath && !mGraphView)
			{
				Sensor sensor = new Sensor (x, y);

				AddSensor (sensor);
				MouseOver (sensor);
				Select    (sensor);
			}



			//----------------------------------------------------------------------------//
			// Modify                                                                     //
			//----------------------------------------------------------------------------//

			if (mMouseOver != null && IsModifying &&
				!mModifyingRange && !mModifyingAngle)
			{
				if (!mMouseOver.IsSelected)
					IsModifying = false;

				else if (leftButton)
					mModifyingRange = true;

				else if (rightButton)
				{
					mModifyingAngle = true;
					Mouse.OverrideCursor = Cursors.None;

					User32.GetCursorPos (out mStartMousePos);
				}
			}

			if (mMouseOver == null && IsModifying)
				IsModifying = false;



			//----------------------------------------------------------------------------//
			// Select / Deselect                                                          //
			//----------------------------------------------------------------------------//

			if (leftButton && mMouseOver == null)
			{
				if (!Keyboard.IsKeyDown (Key.LeftCtrl) &&
					!Keyboard.IsKeyDown (Key.RightCtrl))
					Select (null);
			}

			else if (leftButton && mMouseOver != null &&
				!mShortestPath && !mMouseOver.IsSelected)
			{
				if (!Keyboard.IsKeyDown (Key.LeftCtrl) &&
					!Keyboard.IsKeyDown (Key.RightCtrl))
					Select (null);

				Select (mMouseOver);
			}

			else if (leftButton && mMouseOver != null && mShortestPath)
				ComputeShortestPath (mMouseOver, Keyboard.IsKeyDown (Key.LeftShift) ||
												 Keyboard.IsKeyDown (Key.RightShift));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseUp (object sender, MouseButtonEventArgs args)
		{
			//----------------------------------------------------------------------------//
			// Lasso Select                                                               //
			//----------------------------------------------------------------------------//

			if (IsSelecting)
			{
				// Resolve selection
				if (mSelection.Count > 3)
				{
					var contained = mGraph.SensorsIn (mSelection);

					if (!Keyboard.IsKeyDown (Key.LeftCtrl) &&
						!Keyboard.IsKeyDown (Key.RightCtrl))
						Select (null);

					foreach (Sensor sensor in contained)
						if (!sensor.IsSelected)
						{
							mSelected.AddLast (sensor);
							sensor.IsSelected = true;
						}
				}

				// Cleanup lasso
				mLassoCanvas.Visibility = Visibility.Collapsed;

				mSelection.Clear();
				mLassoCanvas.Children.Clear();
			}



			//----------------------------------------------------------------------------//
			// Modify                                                                     //
			//----------------------------------------------------------------------------//

			else if (mModifyingRange && args.ChangedButton == MouseButton.Left)
				mModifyingRange = false;

			else if (mModifyingAngle && args.ChangedButton == MouseButton.Right)
			{
				Mouse.OverrideCursor = null;
				mModifyingAngle = false;
			}



			//----------------------------------------------------------------------------//
			// Select / Deselect                                                          //
			//----------------------------------------------------------------------------//

			else if (mInitialMousePos == mCurrentMousePos &&
				mMouseOver != null && mMouseOver != mLastSelected &&
				args.ChangedButton == MouseButton.Left && !mShortestPath)
			{
				if (!Keyboard.IsKeyDown (Key.LeftCtrl) &&
					!Keyboard.IsKeyDown (Key.RightCtrl))
					Select (null);

				Select (mMouseOver);
			}

			mLastSelected = null;
			ReleaseMouseCapture();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseMove (object sender, MouseEventArgs args)
		{
			Point position = args.GetPosition (this);
			float x = (float) position.X;
			float y = (float) position.Y;



			//----------------------------------------------------------------------------//
			// Selection                                                                  //
			//----------------------------------------------------------------------------//

			if (IsSelecting)
			{
				if (mSelection.Last == null)
					AddLassoPoint (position);

				else
				{
					float dx = x - (float) mSelection.Last.Value.X;
					float dy = y - (float) mSelection.Last.Value.Y;

					if (dx * dx + dy * dy > 64)
						AddLassoPoint (position);
				}
			}



			//----------------------------------------------------------------------------//
			// Modify                                                                     //
			//----------------------------------------------------------------------------//

			else if (!mShortestPath && !mGraphView && mModifyingRange)
			{
				mMouseOver.Range = mMouseOver.Distance (x, y);
				mMouseOver.Orientation = (float) (57.2957795 * 
					Math.Atan2 (y - mMouseOver.Y, x - mMouseOver.X));

				foreach (Sensor sensor in mSelected)
				{
					sensor.Range = mMouseOver.Range;
					sensor.Orientation = mMouseOver.Orientation;
				}

				SelectionPropertyChanged (mMouseOver);
			}

			else if (!mShortestPath && !mGraphView && mModifyingAngle)
			{
				User32.POINT point;
				User32.GetCursorPos (out point);

				mMouseOver.Angle += (point.X - mStartMousePos.X) * 0.3f;

				foreach (Sensor sensor in mSelected)
					sensor.Angle = mMouseOver.Angle;

				SelectionPropertyChanged (mMouseOver);

				User32.SetCursorPos (mStartMousePos.X, mStartMousePos.Y);
			}



			//----------------------------------------------------------------------------//
			// Move                                                                       //
			//----------------------------------------------------------------------------//

			else if (!mShortestPath && !mGraphView &&
				args.LeftButton == MouseButtonState.Pressed &&
				mMouseOver != null && mMouseOver.IsSelected)
			{
				foreach (Sensor sensor in mSelected)
				{
					sensor.X += x - (float) mCurrentMousePos.X;
					sensor.Y += y - (float) mCurrentMousePos.Y;
				}

				mCurrentMousePos = position;
			}



			//----------------------------------------------------------------------------//
			// Hover                                                                      //
			//----------------------------------------------------------------------------//

			else MouseOver (mGraph.SensorAt (x, y, 16));
		}



		//----------------------------------------------------------------------------//
		// Interface                                                                  //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ComputeShortestPath (Sensor sensor, bool isTarget)
		{
			mPathCanvas.Children.Clear();
			if (sensor == null) return;

			if (isTarget)
				 mTarget = sensor;
			else mSource = sensor;

			int hopCount = 0;

			if (mSource != null && mTarget != null)
			{
				var result = mGraph.FindPath (mSource, mTarget);
				hopCount = result.Count;

				Sensor last = mSource;
				int count = result.Count;

				while (result.Count > 0)
				{
					var value = result.Pop();

					Line line = new Line();
					line.StrokeThickness = 2;

					line.X1 = last.X;
					line.Y1 = last.Y;
					line.X2 = value.X;
					line.Y2 = value.Y;

					if (!mInterpolatePathColor)
						line.Stroke = mPathBrush;
					else line.Stroke = InterpolateColor (mSourceBrush,
						mTargetBrush, count, count - result.Count);

					last = value;
					mPathCanvas.Children.Add (line);
				}
			}

			if (isTarget)
			{
				AddShortestPathNode (mSource, true );
				AddShortestPathNode (mTarget, false);
			}

			else
			{
				AddShortestPathNode (mTarget, false);
				AddShortestPathNode (mSource, true );
			}

			if (mSource != null)
			{
				Label label = new Label();
				label.Content = hopCount;

				label.FontSize   = 18;
				label.FontWeight = FontWeights.Bold;
				label.Foreground = Brushes.White;

				Canvas.SetLeft (label, mSource.X);
				Canvas.SetTop  (label, mSource.Y);

				mPathCanvas.Children.Add (label);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void AddShortestPathNode (Sensor sensor, bool isSource)
		{
			if (sensor == null) return;
			Ellipse ellipse = new Ellipse();

			ellipse.Width  = 16;
			ellipse.Height = 16;
			ellipse.Margin = new Thickness (-8, -8, 0, 0);

			if (isSource)
				 ellipse.Fill = mSourceBrush;
			else ellipse.Fill = mTargetBrush;

			Canvas.SetLeft (ellipse, sensor.X);
			Canvas.SetTop  (ellipse, sensor.Y);

			mPathCanvas.Children.Add (ellipse);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private Brush InterpolateColor (Brush value1, Brush value2, int count, int position)
		{
			Color c1 = ((SolidColorBrush) value1).Color;
			Color c2 = ((SolidColorBrush) value2).Color;

			byte r = (byte) (c1.R + (position * ((c2.R - c1.R) / count)));
			byte g = (byte) (c1.G + (position * ((c2.G - c1.G) / count)));
			byte b = (byte) (c1.B + (position * ((c2.B - c1.B) / count)));

			return new SolidColorBrush (Color.FromArgb (255, r, g, b));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void AddLassoPoint (Point position)
		{
			mSelection.AddLast (position);

			if (mSelection.Last.Previous != null)
			{
				Line line = new Line();

				line.X1 = mSelection.Last.Previous.Value.X;
				line.Y1 = mSelection.Last.Previous.Value.Y;
				line.X2 = mSelection.Last.Value.X;
				line.Y2 = mSelection.Last.Value.Y;

				line.Stroke = mLassoBrush;
				line.StrokeThickness = 2;

				mLassoCanvas.Children.Add (line);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void AddSensor (Sensor sensor)
		{
			mGraph.Add (sensor);
			Children.Add (new SensorControl (sensor));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void MouseOver (Sensor sensor)
		{
			if (sensor == null && mMouseOver != null ||
				sensor != null && mMouseOver != sensor)
			{
				if (mMouseOver != null)
					mMouseOver.IsMouseOver = false;

				mMouseOver = sensor;

				if (mMouseOver != null)
					mMouseOver.IsMouseOver = true;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void Select (Sensor sensor)
		{
			if (sensor == null)
			{
				foreach (Sensor s in mSelected)
					s.IsSelected = false;

				mSelected.Clear();
			}

			else if (!mShortestPath)
			{
				var node = mSelected.First;

				while (node != null)
				{
					if (node.Value == sensor)
					{
						mSelected.Remove (node);
						sensor.IsSelected = false;
						return;
					}

					node = node.Next;
				}

				mLastSelected = sensor;
				mSelected.AddLast (sensor);
				sensor.IsSelected = true;
			}
		}
	}
}
