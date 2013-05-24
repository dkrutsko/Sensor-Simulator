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
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Shapes;
	using System.Windows.Controls;
	using System.Collections.Generic;
	using System.Windows.Media.Animation;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents a sensor which can be drawn on screen. </summary>

	public partial class SensorControl : Canvas
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private bool mGraphView;

		// Brushes
		private static Brush uiBrushCenter;
		private static Brush uiBrushCenterS;

		private static Brush uiBrushAngle;
		private static Brush uiBrushRange;
		private static Brush uiBrushStroke;
		private static Brush uiBrushOrientation;

		// Storyboards
		private static Storyboard uiStoryboardFadeIn;
		private static Storyboard uiStoryboardFadeOut;
		private static Storyboard uiStoryboardExpand;
		private static Storyboard uiStoryboardContract;



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public SensorControl (Sensor sensor)
		{
			sensor.Control = this;
			InitializeComponent();

			// Load static resources
			if (uiBrushCenter == null)
			{
				// Brushes
				uiBrushCenter  = (Brush) FindResource ("uiBrushCenter" );
				uiBrushCenterS = (Brush) FindResource ("uiBrushCenterS");

				uiBrushAngle  = (Brush) FindResource ("uiBrushAngle" );
				uiBrushRange  = (Brush) FindResource ("uiBrushRange" );
				uiBrushStroke = (Brush) FindResource ("uiBrushStroke");

				uiBrushOrientation = (Brush) FindResource ("uiBrushOrientation");

				// Storyboards
				uiStoryboardFadeIn   = (Storyboard) FindResource ("uiStoryboardFadeIn"  );
				uiStoryboardFadeOut  = (Storyboard) FindResource ("uiStoryboardFadeOut" );
				uiStoryboardExpand   = (Storyboard) FindResource ("uiStoryboardExpand"  );
				uiStoryboardContract = (Storyboard) FindResource ("uiStoryboardContract");
			}

			sensor.PropertyChanged += ActionPropertyChanged;
			sensor.MouseOver       += ActionMouseOver;
			sensor.Selected        += ActionSelected;

			ActionPropertyChanged (sensor);
			ActionMouseOver       (sensor);
			ActionSelected        (sensor);
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ToggleGraphView (Sensor sensor, bool value)
		{
			if (mGraphView = value)
				uiStoryboardFadeIn.Begin (uiConnections);
			else uiStoryboardFadeOut.Begin (uiConnections);

			ActionMouseOver (sensor);
			ActionSelected  (sensor);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void AddConnection (Sensor source, Sensor target)
		{
			Line line = new Line();
			line.Stroke = uiBrushCenter;

			line.X2 = target.X - source.X;
			line.Y2 = target.Y - source.Y;

			uiConnections.Children.Add (line);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void RemoveAllConnections()
		{
			uiConnections.Children.Clear();
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionPropertyChanged (Sensor sensor)
		{
			// Position
			Canvas.SetLeft (this, sensor.X);
			Canvas.SetTop  (this, sensor.Y);

			// Range
			float range = sensor.Range;

			uiRange.Width  = sensor.Range * 2;
			uiRange.Height = sensor.Range * 2;

			uiRange.Margin = new Thickness (-range, -range, 0, 0);

			// Orientation
			uiOrientation.X2 = range * Math.Cos
				(sensor.Orientation * 0.0174532925);
			uiOrientation.Y2 = range * Math.Sin
				(sensor.Orientation * 0.0174532925);

			// Angle
			uiAngle.Radius     = sensor.Range;
			uiAngle.WedgeAngle = sensor.Angle;

			uiAngle.RotationAngle = 90 + sensor.
				Orientation - (sensor.Angle / 2);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseOver (Sensor sensor)
		{
			if (sensor.IsMouseOver)
			{
				if (mGraphView)
					uiStoryboardFadeIn.Begin (uiAngle);

				uiStoryboardFadeIn.Begin (uiRange);
				uiStoryboardFadeIn.Begin (uiOrientation);
				uiStoryboardExpand.Begin (uiCenter);
			}

			else
			{
				if (!mGraphView)
					uiStoryboardFadeIn.Begin (uiAngle);

				else if (!sensor.IsSelected)
					uiStoryboardFadeOut.Begin (uiAngle);

				uiStoryboardFadeOut.Begin (uiRange);
				uiStoryboardFadeOut.Begin (uiOrientation);
				uiStoryboardContract.Begin (uiCenter);
			}

		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionSelected (Sensor sensor)
		{
			if (sensor.IsSelected)
			{
				if (mGraphView && !sensor.IsMouseOver)
					uiStoryboardFadeIn.Begin (uiAngle);

				uiCenter.Fill = uiBrushCenterS;
			}

			else
			{
				if (mGraphView && !sensor.IsMouseOver)
					uiStoryboardFadeOut.Begin (uiAngle);

				uiCenter.Fill = uiBrushCenter;
			}
		}
	}
}
