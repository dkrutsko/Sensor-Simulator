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
	using System.Windows.Controls;
	using System.Windows.Media.Animation;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents the ScrollViewer containing the GraphCanvas. </summary>

	public class GraphScrollViewer : ScrollViewer
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		private Point? mPrevCenter;		// Previous center on canvas

		private Cursor mCursorHand;		// Hand cursor icon
		private Cursor mCursorGrab;		// Grab cursor icon

		private bool mTranslatable;		// Is the canvas translatable
		private Point mLastMousePos;	// Last Mouse start position

		// Storyboards
		private Storyboard uiStoryboardFadeIn;
		private Storyboard uiStoryboardFadeOut;



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public bool IsTranslatable
		{
			get
			{
				return mTranslatable;
			}

			set
			{
				if (mTranslatable = value)
				{
					Cursor = mCursorHand;
					uiStoryboardFadeIn.Begin (this);
				}

				else
				{
					Cursor = null;
					uiStoryboardFadeOut.Begin (this);
				}
			}
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public GraphScrollViewer()
		{
			// Actions
			PreviewKeyUp		+= ActionKeyUp;
			PreviewKeyDown		+= ActionKeyDown;
		
			SizeChanged			+= ActionSizeChanged;
			ScrollChanged		+= ActionScrollChanged;

			PreviewMouseMove	+= ActionMouseMove;
			PreviewMouseWheel	+= ActionMouseWheel;

			PreviewMouseLeftButtonUp	+= ActionMouseLeftUp;
			PreviewMouseLeftButtonDown	+= ActionMouseLeftDown;

			// Storyboards
			uiStoryboardFadeIn  = (Storyboard) FindResource ("uiStoryboardFadeIn" );
			uiStoryboardFadeOut = (Storyboard) FindResource ("uiStoryboardFadeOut");

			// Cursors
			mCursorHand = ((Label) FindResource ("uiCursorHand")).Cursor;
			mCursorGrab = ((Label) FindResource ("uiCursorGrab")).Cursor;
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ScrollToOffsetCenter (double x, double y)
		{
			ScrollToOffsetCenter (new Point (x, y));
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public void ScrollToOffsetCenter (Point point)
		{
			ScrollToHorizontalOffset (point.X - (ActualWidth  * 0.5));
			ScrollToVerticalOffset   (point.Y - (ActualHeight * 0.5));
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionKeyDown (object sender, KeyEventArgs args)
		{
			if (args.IsRepeat) return;
			if (args.Key == Key.Space)
				IsTranslatable = true;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionKeyUp (object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Space && IsTranslatable &&
				Mouse.LeftButton == MouseButtonState.Released)
				IsTranslatable = false;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseLeftDown (object sender, MouseButtonEventArgs args)
		{
			if (IsTranslatable && !IsMouseCaptured)
			{
				mLastMousePos = args.GetPosition (this);

				Cursor = mCursorGrab;
				Mouse.Capture (this);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseLeftUp (object sender, MouseButtonEventArgs args)
		{
			if (IsTranslatable && IsMouseCaptured)
			{
				ReleaseMouseCapture();
				Cursor = mCursorHand;

				if (!Keyboard.IsKeyDown (Key.Space))
					IsTranslatable = false;
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseMove (object sender, MouseEventArgs args)
		{
			if (IsTranslatable && IsMouseCaptured)
			{
				Point position = args.GetPosition (this);

				double dx = position.X - mLastMousePos.X;
				double dy = position.Y - mLastMousePos.Y;

				mLastMousePos = position;

				ScrollToVerticalOffset   (VerticalOffset   - dy);
				ScrollToHorizontalOffset (HorizontalOffset - dx);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseWheel (object sender, MouseWheelEventArgs args)
		{
			args.Handled = true;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionScrollChanged (object sender, ScrollChangedEventArgs args)
		{
			Point centerViewport = new Point (ViewportWidth / 2, ViewportHeight / 2);
			mPrevCenter = this.TranslatePoint (centerViewport, (UIElement) Content);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionSizeChanged (object sender, SizeChangedEventArgs args)
		{
			if (mPrevCenter != null)
				ScrollToOffsetCenter (mPrevCenter.Value);
		}
	}
}
