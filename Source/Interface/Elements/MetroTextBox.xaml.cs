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
	using System.Windows.Controls;
	using System.Windows.Media.Imaging;
	using System.Windows.Media.Animation;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Represents a metro style text box. </summary>

	public partial class MetroTextBox : UserControl
	{
		//----------------------------------------------------------------------------//
		// Static                                                                     //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		static MetroTextBox()
		{
			FocusableProperty.OverrideMetadata (typeof (MetroTextBox), new FrameworkPropertyMetadata
					(true, new PropertyChangedCallback (MetroTextBox.OnFocusableChanged)));
		}



		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//

		// Brushes
		private static Brush uiEnterBrush;
		private static Brush uiLeaveBrush;

		// Storyboards
		private static Storyboard uiStoryboardFadeIn;
		private static Storyboard uiStoryboardFadeOut;



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public string Text
		{
			get { return uiTextBox.Text;							}
			set { uiTextBox.Text= value;							}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public string Description
		{
			get { return (string) GetValue (DescriptionProperty);	}
			set { SetValue (DescriptionProperty, value);			}
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public MetroTextBox()
		{
			InitializeComponent();

			// Load static resources
			if (uiEnterBrush == null)
			{
				// Brushes
				uiEnterBrush = (Brush) FindResource ("uiEnterBrush");
				uiLeaveBrush = (Brush) FindResource ("uiLeaveBrush");

				// Storyboards
				uiStoryboardFadeIn  = (Storyboard) FindResource ("uiStoryboardFadeIn" );
				uiStoryboardFadeOut = (Storyboard) FindResource ("uiStoryboardFadeOut");
			}

			DataObject.AddPastingHandler (uiTextBox, OnCancelCommand);
			DataObject.AddCopyingHandler (uiTextBox, OnCancelCommand);
			
			uiTextBox.MouseEnter += delegate { uiDescription.Foreground = uiEnterBrush; };
			uiTextBox.MouseLeave += delegate { uiDescription.Foreground = uiLeaveBrush; };

			uiButtonClear.Click  += delegate { uiTextBox.Text = ""; };
		}



		//----------------------------------------------------------------------------//
		// Handlers                                                                   //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>
		
		private void OnCancelCommand (object sender, DataObjectEventArgs args)
		{
			args.CancelCommand();
		}



		//----------------------------------------------------------------------------//
		// Actions                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionTextInput (object sender, TextCompositionEventArgs args)
		{
			foreach(char c in args.Text)
				if (!char.IsDigit(c) && c != '-' && c != ',')
				{
					args.Handled = true;
					return;
				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionTextChanged (object sender, TextChangedEventArgs args)
		{
			if (uiTextBox.Text.Length == 0)
				uiStoryboardFadeIn .Begin (uiDescription);

			else
				uiStoryboardFadeOut.Begin (uiDescription);
		}



		//----------------------------------------------------------------------------//
		// XAML Properties                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register
			("Description", typeof (string), typeof (MetroTextBox), new FrameworkPropertyMetadata
			(null, new PropertyChangedCallback (MetroTextBox.OnDescriptionChanged)));



		//----------------------------------------------------------------------------//
		// XAML Actions                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>
		
		private static void OnFocusableChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			MetroTextBox element = (MetroTextBox) source;
			element.uiTextBox.Focusable = (bool) args.NewValue;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnDescriptionChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			MetroTextBox element = (MetroTextBox) source;
			element.uiDescription.Content = (string) args.NewValue;
		}
	}
}
