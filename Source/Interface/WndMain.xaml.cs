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
	using System.Windows.Data;
	using System.Windows.Media;
	using System.Windows.Input;
	using System.Windows.Shapes;
	using System.Windows.Interop;
	using System.Windows.Controls;
	using System.Windows.Media.Animation;
	using System.Collections.Generic;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents the main application window. </summary>

	public partial class WndMain : System.Windows.Window
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private WindowState mState;

		// Brushes
		private Brush uiBrushNormal;
		private Brush uiBrushOver;

		// Storyboards
		private Storyboard uiStoryboardShowDlg;
		private Storyboard uiStoryboardHideDlg;
		private Storyboard uiStoryboardShowIcon;
		private Storyboard uiStoryboardHideIcon;
		private Storyboard uiStoryboardShowMenu;
		private Storyboard uiStoryboardHideMenu;
		private Storyboard uiStoryboardShowModify;
		private Storyboard uiStoryboardHideModify;

		// SysMenu Identifiers
		private IntPtr mSysMenu;
		private const Int32 SysMenu_TopMost = 1000;



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Gets or sets the value indicating whether this
		/// 		  window should be in front of all other windows. </summary>

		public new bool Topmost
		{
			get
			{
				return base.Topmost;
			}

			set
			{
				base.Topmost = value;
				User32.ModifyMenu (mSysMenu, 3, User32.MF_BYPOSITION | User32.MF_STRING |
					(value ? User32.MF_CHECKED : User32.MF_UNCHECKED), SysMenu_TopMost, "Top Most");
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the main menu. </summary>

		public bool DisplayMenu
		{
			get
			{
				return uiGridMenu.IsHitTestVisible;
			}

			set
			{
				if (uiGridMenu.IsHitTestVisible = value)
				{
					uiButtonExpand.Focus();
					uiStoryboardShowMenu.Begin (uiGridMenu);
					uiStoryboardShowIcon.Begin (uiButtonAbout );
					uiStoryboardShowIcon.Begin (uiButtonExpand);
				}

				else
				{
					uiGraphCanvas.Focus();
					uiStoryboardHideMenu.Begin (uiGridMenu);
					uiStoryboardHideIcon.Begin (uiButtonAbout );
					uiStoryboardHideIcon.Begin (uiButtonExpand);
				}

				uiButtonAbout .IsHitTestVisible = value;
				uiButtonExpand.IsHitTestVisible = value;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the sensor modification menu. </summary>

		public bool DisplayModify
		{
			get
			{
				return uiGridModify.IsHitTestVisible;
			}

			set
			{
				if (uiGridModify.IsHitTestVisible = value)
					 uiStoryboardShowModify.Begin (uiGridModify);
				else uiStoryboardHideModify.Begin (uiGridModify);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the about dialog. </summary>

		public bool DisplayAbout
		{
			get
			{
				return uiGridAbout.IsHitTestVisible;
			}

			set
			{
				if (uiGridAbout.IsHitTestVisible = value)
				{
					uiButtonImage.Focus();
					uiStoryboardShowDlg.Begin (uiGridAbout);
				}

				else
				{
					uiButtonAbout.Focus();
					uiStoryboardHideDlg.Begin (uiGridAbout);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the randomize dialog. </summary>

		public bool DisplayRandom
		{
			get
			{
				return uiGridRandom.IsHitTestVisible;
			}

			set
			{
				if (uiGridRandom.IsHitTestVisible = value)
				{
					uiButtonRandomDone.Focus();
					uiStoryboardShowDlg.Begin (uiGridRandom);
				}

				else
				{
					uiButtonRandom.Focus();
					uiStoryboardHideDlg.Begin (uiGridRandom);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the statistics dialog. </summary>

		public bool DisplayStats
		{
			get
			{
				return uiGridStats.IsHitTestVisible;
			}

			set
			{
				if (uiGridStats.IsHitTestVisible = value)
				{
					uiButtonStatsDone.Focus();
					uiStoryboardShowDlg.Begin (uiGridStats);
				}

				else
				{
					uiButtonStats.Focus();
					uiStoryboardHideDlg.Begin (uiGridStats);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the line graph. </summary>

		public bool DisplayLineGraph
		{
			get
			{
				return uiLineGraphStats.IsHitTestVisible;
			}

			set
			{
				if (uiLineGraphStats.IsHitTestVisible = value)
				{
					uiLineGraphStats.Focus();
					uiStoryboardShowDlg.Begin (uiLineGraphStats);
				}

				else
				{
					uiGraphCanvas.Focus();
					uiStoryboardHideDlg.Begin (uiLineGraphStats);
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Shows or hides the remove all confirmation dialog. </summary>

		public bool DisplayClear
		{
			get
			{
				return uiGridClear.IsHitTestVisible;
			}

			set
			{
				if (uiGridClear.IsHitTestVisible = value)
				{
					uiButtonNo.Focus();
					uiStoryboardShowDlg.Begin (uiGridClear);
				}

				else
				{
					uiButtonClear.Focus();
					uiStoryboardHideDlg.Begin (uiGridClear);
				}
			}
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Creates a new main window object. </summary>

		public WndMain()
		{
			InitializeComponent();

			// Brushes
			uiBrushNormal = (Brush) FindResource ("uiBrushNormal");
			uiBrushOver   = (Brush) FindResource ("uiBrushOver"  );

			// Storyboards
			uiStoryboardShowDlg     = (Storyboard) FindResource ("uiStoryboardShowDlg"   );
			uiStoryboardHideDlg     = (Storyboard) FindResource ("uiStoryboardHideDlg"   );
			uiStoryboardShowIcon    = (Storyboard) FindResource ("uiStoryboardShowIcon"  );
			uiStoryboardHideIcon    = (Storyboard) FindResource ("uiStoryboardHideIcon"  );
			uiStoryboardShowMenu    = (Storyboard) FindResource ("uiStoryboardShowMenu"  );
			uiStoryboardHideMenu    = (Storyboard) FindResource ("uiStoryboardHideMenu"  );
			uiStoryboardShowModify  = (Storyboard) FindResource ("uiStoryboardShowModify");
			uiStoryboardHideModify  = (Storyboard) FindResource ("uiStoryboardHideModify");

			// Background
			CreateBackground();

			uiGraphCanvas.SelectionPropertyChanged += ActionSelectionPropertyChanged;
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseDown (object sender, MouseButtonEventArgs args)
		{
			//----------------------------------------------------------------------------//
			// Menu                                                                       //
			//----------------------------------------------------------------------------//

			if (args.RightButton == MouseButtonState.Pressed &&
				!uiGraphCanvas.IsSelecting && !uiGraphCanvas.IsModifying &&
				!DisplayMenu && !uiGraphScrollViewer.IsTranslatable)
			{
				if (uiGraphCanvas.CountSelected == 0)
					 uiLabelClear.Content = "Remove All";
				else uiLabelClear.Content = "Delete";

				DisplayMenu = true;
			}

			else CloseMenus();

			if (DisplayModify && !uiGraphCanvas.IsModifying)
				DisplayModify = false;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMenuButton (object sender, RoutedEventArgs args)
		{
			//----------------------------------------------------------------------------//
			// Menu                                                                       //
			//----------------------------------------------------------------------------//

			if (sender == uiButtonGraph)
			{
				DisplayAbout  = false;
				DisplayClear  = false;
				DisplayRandom = false;
				DisplayStats  = false;
				uiButtonGraph.Focus();

				uiGraphCanvas.ToggleGraphView
					(uiButtonGraph.IsChecked.Value);
			}

			else if (sender == uiButtonShort)
			{
				DisplayAbout  = false;
				DisplayClear  = false;
				DisplayRandom = false;
				DisplayStats  = false;
				uiButtonShort.Focus();

				uiGraphCanvas.ToggleShortestPath
					(uiButtonShort.IsChecked.Value);
			}

			else if (sender == uiButtonAbout)
			{
				DisplayAbout  = !DisplayAbout;
				DisplayClear  = false;
				DisplayRandom = false;
				DisplayStats  = false;
			}

			else if (sender == uiButtonClear)
			{
				if (uiGraphCanvas.CountSelected == 0)
				{
					DisplayAbout  = false;
					DisplayClear  = !DisplayClear;
					DisplayRandom = false;
					DisplayStats  = false;
				}

				else
				{
					CancelSpecialForm();
					uiGraphCanvas.ClearSelection();
					uiLabelClear.Content = "Remove All";
				}
			}

			else if (sender == uiButtonRandom)
			{
				DisplayAbout  = false;
				DisplayClear  = false;
				DisplayRandom = !DisplayRandom;
				DisplayStats  = false;
			}

			else if (sender == uiButtonStats)
			{
				DisplayAbout  = false;
				DisplayClear  = false;
				DisplayRandom = false;
				DisplayStats  = !DisplayStats;
			}



			//----------------------------------------------------------------------------//
			// Clear                                                                      //
			//----------------------------------------------------------------------------//

			else if (sender == uiButtonYes)
			{
				CancelSpecialForm();
				uiGraphCanvas.Clear();
				DisplayClear = false;
			}

			else if (sender == uiButtonNo)
				DisplayClear = false;



			//----------------------------------------------------------------------------//
			// Random                                                                     //
			//----------------------------------------------------------------------------//

			else if (sender == uiButtonRandomDone)
				CreateRandom();



			//----------------------------------------------------------------------------//
			// Stats                                                                      //
			//----------------------------------------------------------------------------//

			else if (sender == uiButtonStatsDone)
				PerformStats();



			//----------------------------------------------------------------------------//
			// About                                                                      //
			//----------------------------------------------------------------------------//

			else if (sender == uiButtonImage)
				DisplayAbout = false;



			//----------------------------------------------------------------------------//
			// Expand                                                                     //
			//----------------------------------------------------------------------------//

			else if (sender == uiButtonExpand)
			{
				if (uiButtonExpand.IsChecked.Value)
				{
					mState = WindowState;
					WindowStyle = WindowStyle.None;
					WindowState = WindowState.Maximized;
				}

				else
				{
					WindowStyle = WindowStyle.SingleBorderWindow;
					WindowState = mState;
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionKeyDown (object sender, KeyEventArgs args)
		{
			if (!args.IsRepeat)
			{
				//----------------------------------------------------------------------------//
				// Menu                                                                       //
				//----------------------------------------------------------------------------//

				if (args.Key == Key.Escape)
				{
					if (!DisplayMenu)
						CancelSpecialForm();

					CloseMenus();
				}



				//----------------------------------------------------------------------------//
				// Clear Selection                                                            //
				//----------------------------------------------------------------------------//

				else if (args.Key == Key.Delete)
				{
					uiGraphCanvas.ClearSelection();
					uiLabelClear.Content = "Remove All";
					CancelSpecialForm();
				}



				//----------------------------------------------------------------------------//
				// Modify                                                                     //
				//----------------------------------------------------------------------------//

				else if (args.Key == Key.Enter &&
					uiGraphCanvas.CountSelected > 0 && !uiGraphCanvas.IsModifying &&
					!uiButtonShort.IsChecked.Value && !uiButtonGraph.IsChecked.Value)
				{
					uiTextBoxActiveAngle.Value  = uiGraphCanvas.GetSelectedProperty (Sensor.AngleProperty );
					uiTextBoxActiveRange.Value  = uiGraphCanvas.GetSelectedProperty (Sensor.RangeProperty );
					uiTextBoxActiveOrient.Value = uiGraphCanvas.GetSelectedProperty (Sensor.OrientProperty);

					DisplayModify = true;
					uiGraphCanvas.IsModifying = true;
				}



				//----------------------------------------------------------------------------//
				// Reverse Shortest Path                                                      //
				//----------------------------------------------------------------------------//

				else if (args.Key == Key.R && uiButtonShort.IsChecked.Value)
					uiGraphCanvas.ReverseShortestPath();



				//----------------------------------------------------------------------------//
				// Select All                                                                 //
				//----------------------------------------------------------------------------//

				else if (args.Key == Key.A &&
					(Keyboard.IsKeyDown (Key.LeftCtrl) || Keyboard.IsKeyDown (Key.RightCtrl)))
					uiGraphCanvas.SelectAll();
			}



			//----------------------------------------------------------------------------//
			// Random Shortest Path                                                       //
			//----------------------------------------------------------------------------//

			if (args.Key == Key.Enter && uiButtonShort.IsChecked.Value)
			{
				uiGraphCanvas.RandomPathSource();
				uiGraphCanvas.RandomPathTarget();
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionRandomKeyUp (object sender, KeyEventArgs args)
		{
			//----------------------------------------------------------------------------//
			// Random                                                                     //
			//----------------------------------------------------------------------------//

			if (args.Key == Key.Enter)
				CreateRandom();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionStatsKeyUp (object sender, KeyEventArgs args)
		{
			//----------------------------------------------------------------------------//
			// Stats                                                                      //
			//----------------------------------------------------------------------------//

			if (args.Key == Key.Enter)
				PerformStats();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionValueChanged (object sender, RoutedPropertyChangedEventArgs<float?> args)
		{
			//----------------------------------------------------------------------------//
			// Modify                                                                     //
			//----------------------------------------------------------------------------//

			if (args.NewValue == null) return;

			if (sender == uiTextBoxActiveAngle)
				uiGraphCanvas.SetSelectedProperty
					(Sensor.AngleProperty, args.NewValue.Value);

			else if (sender == uiTextBoxActiveRange)
				uiGraphCanvas.SetSelectedProperty
					(Sensor.RangeProperty, args.NewValue.Value);

			else if (sender == uiTextBoxActiveOrient)
				uiGraphCanvas.SetSelectedProperty
					(Sensor.OrientProperty, args.NewValue.Value);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionSelectionPropertyChanged (Sensor sensor)
		{
			uiTextBoxActiveAngle .Value = sensor.Angle;
			uiTextBoxActiveRange .Value = sensor.Range;
			uiTextBoxActiveOrient.Value = sensor.Orientation;
		}



		//----------------------------------------------------------------------------//
		// Interface                                                                  //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void CancelSpecialForm()
		{
			uiButtonGraph.IsChecked = false;
			uiButtonShort.IsChecked = false;

			uiGraphCanvas.ToggleGraphView    (false);
			uiGraphCanvas.ToggleShortestPath (false);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void CreateRandom()
		{
			Range sensors = new Range();
			Range angle   = new Range();
			Range orient  = new Range();
			Range range   = new Range();

			if (sensors.Parse (uiTextBoxRandomSensors.Text) && angle.Parse (uiTextBoxRandomAngle.Text) &&
				orient.Parse  (uiTextBoxRandomOrient.Text ) && range.Parse (uiTextBoxRandomRange.Text) &&
				uiGraphCanvas.Randomize (sensors, angle, orient, range, true))
			{
				CancelSpecialForm();
				DisplayRandom = false;
				DisplayMenu   = false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void PerformStats()
		{
			Range trials  = new Range();
			Range sensors = new Range();
			Range angle   = new Range();
			Range range   = new Range();

			if (trials.Parse (uiTextBoxStatsTrials.Text) && sensors.Parse (uiTextBoxStatsSensors.Text) &&
				angle.Parse  (uiTextBoxStatsAngle.Text ) && range.Parse   (uiTextBoxStatsRange.Text))
			{
				Statistics statistics = new Statistics (trials, sensors,
					angle, range, (int) ActualWidth, (int) ActualHeight);

				if (statistics.PerformTests())
				{
					uiLineGraphStats.Update (statistics);

					if (!DisplayLineGraph)
						DisplayLineGraph = true;
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void CloseMenus()
		{
				 if (DisplayAbout    ) DisplayAbout     = false;
			else if (DisplayClear    ) DisplayClear     = false;
			else if (DisplayRandom   ) DisplayRandom    = false;
			else if (DisplayStats    ) DisplayStats     = false;
			else if (DisplayMenu     ) DisplayMenu      = false;
			else if (DisplayLineGraph) DisplayLineGraph = false;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void CreateBackground()
		{
			Random random = new Random();
			const int rectangleCount = 35;
			
			int max = (int) (SystemParameters.VirtualScreenWidth / rectangleCount) + 1;
			SolidColorBrush stroke = new SolidColorBrush (Color.FromArgb (8, 0, 0, 0));

			for (int x = 0; x < max; ++x)
			{
				Rectangle rectangle = new Rectangle();

				rectangle.Width  = rectangleCount;
				rectangle.Height = SystemParameters.VirtualScreenHeight;
				rectangle.Stroke = stroke;

				rectangle.Fill = new SolidColorBrush (Color.FromArgb
					((byte) random.Next (0, 12), 255, 255, 255));

				uiCanvasBackground.Children.Add (rectangle);
				Canvas.SetLeft (rectangle, x * rectangleCount - 5);
			}
		}



		//----------------------------------------------------------------------------//
		// Window                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void WindowLoaded (object sender, RoutedEventArgs args)
		{
			// Focus the graph canvas
			uiGraphCanvas.Focus();

			// Get the Handle for the Forms System Menu
			IntPtr handle = new WindowInteropHelper (this).Handle;
			mSysMenu = User32.GetSystemMenu (handle, false);

			// Create new System Menu items at various positions
			User32.InsertMenu (mSysMenu, 3, User32.MF_BYPOSITION |
				User32.MF_STRING, SysMenu_TopMost, "Top Most");

			// Attach a WndProc handler to this Window
			HwndSource source = HwndSource.FromHwnd (handle);
			source.AddHook (new HwndSourceHook (WndProc));

			// Scroll to the center of graph canvas
			uiGraphScrollViewer.ScrollToOffsetCenter
				(uiGraphCanvas.ActualWidth  / 2,
				 uiGraphCanvas.ActualHeight / 2);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Windows procedure handling system menu commands. </summary>

		private IntPtr WndProc (IntPtr handle, int message,
			IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			// Handle the system menu
			if (message == Win32.WM_SYSCOMMAND)
				switch (wParam.ToInt32())
				{
					case SysMenu_TopMost:
						Topmost = !Topmost;
						handled = true; break;
				}

			return IntPtr.Zero;
		}
	}
}
