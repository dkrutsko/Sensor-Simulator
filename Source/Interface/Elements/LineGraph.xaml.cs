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
	using System.Windows.Media.Animation;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents a line graph containing
	/// 		  results for statistical analysis. </summary>

	public partial class LineGraph : UserControl
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private Line mPrevious;
		private Line[] mLines = new Line[10];
		private Label[] mLabelRatios = new Label[10];

		private Statistics.DataPoint[] mDataPoints = new Statistics.DataPoint[10];

		// Storyboards
		private static Storyboard uiStoryboardFadeIn;
		private static Storyboard uiStoryboardFadeOut;



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public LineGraph()
		{
			InitializeComponent();

			// Load static resources
			if (uiStoryboardFadeIn == null)
			{
				// Storyboards
				uiStoryboardFadeIn  = (Storyboard) FindResource ("uiStoryboardFadeIn" );
				uiStoryboardFadeOut = (Storyboard) FindResource ("uiStoryboardFadeOut");
			}

			// Create mouse lines and label ratios
			for (int i = 0; i < mLines.Length; ++i)
			{
				mLines[i] = new Line();
				mLines[i].Opacity = 0;
				mLines[i].Stroke = Brushes.White;
				mLines[i].StrokeThickness = 4;

				mLabelRatios[i] = new Label();
				mLabelRatios[i].FontSize = 16;
				mLabelRatios[i].Margin = new Thickness (-20, 0, 0, 0);
				mLabelRatios[i].Foreground = Brushes.White;
				mLabelRatios[i].Content = 0;

				Grid.SetRow    (mLabelRatios[i], 0);
				Grid.SetColumn (mLabelRatios[i], i);
				uiGridMain.Children.Add (mLabelRatios[i]);
			}

			mLabelRatios[9].Margin = new Thickness (0, 0, -20, 0);
			mLabelRatios[9].HorizontalAlignment = HorizontalAlignment.Right;

			SizeChanged += delegate { UpdateGraph(); };
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void Update (Statistics statistics)
		{
			for (int i = 0; i < mDataPoints.Length; ++i)
				mDataPoints[i] = statistics.GetDataPoint(i);

			UpdateGraph();
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseEnter (object sender, MouseEventArgs args)
		{
			uiStoryboardFadeIn.Begin (uiGraphData);

			uiStoryboardFadeIn.BeginTime = TimeSpan.FromMilliseconds (60);
			uiStoryboardFadeIn.Begin (uiBorder1);

			uiStoryboardFadeIn.BeginTime = TimeSpan.FromMilliseconds (45);
			uiStoryboardFadeIn.Begin (uiBorder2);

			uiStoryboardFadeIn.BeginTime = TimeSpan.FromMilliseconds (30);
			uiStoryboardFadeIn.Begin (uiBorder3);

			uiStoryboardFadeIn.BeginTime = TimeSpan.FromMilliseconds (15);
			uiStoryboardFadeIn.Begin (uiBorder4);

			uiStoryboardFadeIn.BeginTime = TimeSpan.FromMilliseconds (0);
			uiStoryboardFadeIn.Begin (uiBorder5);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseLeave (object sender, MouseEventArgs args)
		{
			mPrevious = null;
			foreach (var line in mLines)
				uiStoryboardFadeOut.Begin (line);

			uiStoryboardFadeOut.Begin (uiGraphData);
			
			uiStoryboardFadeOut.BeginTime = TimeSpan.FromMilliseconds (60);
			uiStoryboardFadeOut.Begin (uiBorder1);

			uiStoryboardFadeOut.BeginTime = TimeSpan.FromMilliseconds (45);
			uiStoryboardFadeOut.Begin (uiBorder2);

			uiStoryboardFadeOut.BeginTime = TimeSpan.FromMilliseconds (30);
			uiStoryboardFadeOut.Begin (uiBorder3);

			uiStoryboardFadeOut.BeginTime = TimeSpan.FromMilliseconds (15);
			uiStoryboardFadeOut.Begin (uiBorder4);

			uiStoryboardFadeOut.BeginTime = TimeSpan.FromMilliseconds (0);
			uiStoryboardFadeOut.Begin (uiBorder5);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseMove (object sender, MouseEventArgs args)
		{
			int iteration = (int) (args.GetPosition
				(this).X / (ActualWidth / 9) + 0.5);

			if (mPrevious != mLines[iteration])
			{
				if (mPrevious != null)
					uiStoryboardFadeOut.Begin (mPrevious);

				mPrevious = mLines[iteration];
				uiStoryboardFadeIn.Begin (mPrevious);
			}

			uiTextBoxTrials.Content  = mDataPoints[iteration].Trials;
			uiTextBoxSensors.Content = mDataPoints[iteration].Sensors;
			uiTextBoxAngle.Content   = mDataPoints[iteration].Angle;
			uiTextBoxRange.Content   = mDataPoints[iteration].Range;
			uiTextBoxMissed.Content  = mDataPoints[iteration].Missed;

			uiTextBoxAvgDiff.Content = mDataPoints[iteration].AvgDiff.ToString ("0.00");
			uiTextBoxAvgDir.Content  = mDataPoints[iteration].AvgDir.ToString  ("0.00");
			uiTextBoxAvgOmni.Content = mDataPoints[iteration].AvgOmni.ToString ("0.00");
		}



		//----------------------------------------------------------------------------//
		// Internal                                                                   //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void UpdateGraph()
		{
			if (mDataPoints[0] == null) return;

			double width  = ActualWidth / 9;
			double height = uiCanvas.ActualHeight;

			PointCollection blackPoints = new PointCollection (10);
			PointCollection whitePoints = new PointCollection (10);

			double largest = 1.0;

			// Determine largest data point
			foreach (var point in mDataPoints)
				if (point.Ratio > largest)
					largest = point.Ratio;

			// Calculate graph points
			for (int i = 0; i < 10; ++i)
			{
				double y = height - (height * (mDataPoints[i].Ratio / largest));
				blackPoints.Add (new Point (width * i, y + 2));
				whitePoints.Add (new Point (width * i, y    ));
			}

			// Create black line
			Polyline blackLine = new Polyline();
			blackLine.Stroke = Brushes.Black;
			blackLine.StrokeThickness = 6;

			blackLine.StrokeEndLineCap   = PenLineCap.Round;
			blackLine.StrokeStartLineCap = PenLineCap.Round;
			blackLine.Points = blackPoints;

			// Create white line
			Polyline whiteLine = new Polyline();
			whiteLine.Stroke = Brushes.White;
			whiteLine.StrokeThickness = 6;

			whiteLine.StrokeEndLineCap   = PenLineCap.Round;
			whiteLine.StrokeStartLineCap = PenLineCap.Round;
			whiteLine.Points = whitePoints;

			// Add lines to the canvas
			uiCanvas.Children.Clear();
			uiCanvas.Children.Add (blackLine);
			uiCanvas.Children.Add (whiteLine);

			// Update mouse lines
			for (int i = 0; i < mLines.Length; ++i)
			{
				mLabelRatios[i].Content = mDataPoints[i].Ratio.ToString ("0.00");

				mLines[i].X1 = (ActualWidth / 9) * i;
				mLines[i].X2 = mLines[i].X1;
				mLines[i].Y2 = uiCanvas.ActualHeight;

				uiCanvas.Children.Add (mLines[i]);
			}
		}
	}
}
