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
	/// <summary> Represents a textbox with a built-in slider feature. </summary>

	public partial class ModifyTextBox : UserControl
	{
		//----------------------------------------------------------------------------//
		// Fields                                                                     //
		//----------------------------------------------------------------------------//
		
		private bool mDeltaChanged;
		private float? mStartingValue;
		private User32.POINT mStartMousePos;

		// Brushes
		private static Brush uiEnterBrush;
		private static Brush uiLeaveBrush;



		//----------------------------------------------------------------------------//
		// Events                                                                     //
		//----------------------------------------------------------------------------//

		public event RoutedPropertyChangedEventHandler<float?> ValueChanged
		{
			add    { AddHandler    (ValueChangedEvent, value);		}
			remove { RemoveHandler (ValueChangedEvent, value);		}
		}



		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public string Description
		{
			get { return (string) GetValue (DescriptionProperty);	}
			set { SetValue (DescriptionProperty, value);			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public bool Wrap
		{
			get { return (bool) GetValue (WrapProperty);			}
			set { SetValue (WrapProperty, value);					}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public float? Value
		{
			get { return (float?) GetValue (ValueProperty);			}
			set { SetValue (ValueProperty, value);					}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public float Minimum
		{
			get { return (float) GetValue (MinimumProperty);		}
			set { SetValue (MinimumProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public float Maximum
		{
			get { return (float) GetValue (MaximumProperty);		}
			set { SetValue (MaximumProperty, value);				}
		}



		//----------------------------------------------------------------------------//
		// Constructors                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public ModifyTextBox()
		{
			InitializeComponent();

			// Load static resources
			if (uiEnterBrush == null)
			{
				// Brushes
				uiEnterBrush = (Brush) FindResource ("uiEnterBrush");
				uiLeaveBrush = (Brush) FindResource ("uiLeaveBrush");
			}

			DataObject.AddPastingHandler (uiTextBox, OnCancelCommand);
			DataObject.AddCopyingHandler (uiTextBox, OnCancelCommand);
			
			MouseEnter += delegate { uiDescription.Foreground = uiEnterBrush; };
			MouseLeave += delegate { uiDescription.Foreground = uiLeaveBrush; };

			uiTextBox.LostFocus += delegate { ApplyTextValue(); };

			SizeChanged += delegate { UpdateInterface(); };
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

		private void ActionPreviewTextInput (object sender, TextCompositionEventArgs args)
		{
			foreach(char c in args.Text)
				if (!char.IsDigit(c))
				{
					args.Handled = true;
					return;
				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionPreviewKeyUp (object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Enter)
				ApplyTextValue();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseDown (object sender, MouseButtonEventArgs args)
		{
			mDeltaChanged  = false;
			mStartingValue = Value;
			
			User32.GetCursorPos (out mStartMousePos);
			Mouse.Capture (this);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseUp (object sender, MouseButtonEventArgs args)
		{
			if (!mDeltaChanged)
			{
				uiTextBox.Focusable = true;
				uiTextBox.Cursor = Cursors.IBeam;

				uiTextBox.SelectAll();
				uiTextBox.Focus();

				uiSlider.Visibility = Visibility.Collapsed;
			}

			ReleaseMouseCapture();
			Mouse.OverrideCursor = null;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ActionMouseMove (object sender, MouseEventArgs args)
		{
			if (IsMouseCaptured)
			{
				User32.POINT position;
				User32.GetCursorPos (out position);

				float delta = (position.X - mStartMousePos.X) * 0.3f;
				User32.SetCursorPos (mStartMousePos.X, mStartMousePos.Y);

				if (delta != 0f)
				{
					if (Value == null)
						Value = delta;

					else
					{
						float current = Value.Value;
						current += delta;

						if (Wrap && current > Maximum)
							current -= Maximum;

						if (Wrap && current < Minimum)
							current += Maximum;

						Value = current;
					}

					Mouse.OverrideCursor = Cursors.None;
					mDeltaChanged = true;
				}
			}
		}



		//----------------------------------------------------------------------------//
		// Internal                                                                   //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void ApplyTextValue()
		{
			uiTextBox.Focusable = false;
			uiTextBox.Cursor = Cursors.Arrow;
			uiSlider.Visibility = (bool) Wrap ?
				Visibility.Collapsed : Visibility.Visible;

			float value = 0f;
			float.TryParse (uiTextBox.Text, out value);

			Value = value;
			UpdateInterface();
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private void UpdateInterface()
		{
			float? value = (float?) Value;
			uiTextBox.Text = value == null ?
				"--" : ((int) value.Value).ToString();

			uiSlider.Width = Value == null ? 0 :
				(Value.Value / Maximum) * ActualWidth;
		}



		//----------------------------------------------------------------------------//
		// XAML Events                                                                //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent ("ValueChanged",
			RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<float?>), typeof (ModifyTextBox));



		//----------------------------------------------------------------------------//
		// XAML Properties                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty WrapProperty = DependencyProperty.Register
			("Wrap", typeof (bool), typeof (ModifyTextBox), new FrameworkPropertyMetadata
			(false, new PropertyChangedCallback (ModifyTextBox.OnWrapChanged)));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register
			("Description", typeof (string), typeof (ModifyTextBox), new FrameworkPropertyMetadata
			(null, new PropertyChangedCallback (ModifyTextBox.OnDescriptionChanged)));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
			("Value", typeof (float?), typeof (ModifyTextBox), new FrameworkPropertyMetadata
			((float?) 0, FrameworkPropertyMetadataOptions.Journal |
						 FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				new PropertyChangedCallback (ModifyTextBox.OnValueChanged  ),
				new CoerceValueCallback     (ModifyTextBox.ConstrainToRange)),
				new ValidateValueCallback   (ModifyTextBox.IsValidValue    ));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
			("Minimum", typeof (float), typeof (ModifyTextBox), new FrameworkPropertyMetadata
			(0f, new PropertyChangedCallback (ModifyTextBox.OnMinimumChanged)),
				 new ValidateValueCallback   (ModifyTextBox.IsValidFloat    ));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
			("Maximum", typeof (float), typeof (ModifyTextBox), new FrameworkPropertyMetadata
			(1f, new PropertyChangedCallback (ModifyTextBox.OnMaximumChanged),
				 new CoerceValueCallback     (ModifyTextBox.CoerceMaximum   )),
				 new ValidateValueCallback   (ModifyTextBox.IsValidFloat    ));



		//----------------------------------------------------------------------------//
		// XAML Actions                                                               //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnDescriptionChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			ModifyTextBox element = (ModifyTextBox) source;
			element.uiDescription.Content = (string) args.NewValue;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnWrapChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			ModifyTextBox element = (ModifyTextBox) source;
			element.uiSlider.Visibility = (bool) args.NewValue ?
				Visibility.Collapsed : Visibility.Visible;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnValueChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			ModifyTextBox element = (ModifyTextBox) source;

			element.UpdateInterface();

			RoutedPropertyChangedEventArgs<float?> evt = new RoutedPropertyChangedEventArgs<float?>
				((float?) args.OldValue, (float?) args.NewValue, ModifyTextBox.ValueChangedEvent);

			element.RaiseEvent (evt);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnMinimumChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			ModifyTextBox element = (ModifyTextBox) source;

			element.CoerceValue (MaximumProperty);
			element.CoerceValue (ValueProperty  );
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static void OnMaximumChanged (DependencyObject
			source, DependencyPropertyChangedEventArgs args)
		{
			ModifyTextBox element = (ModifyTextBox) source;

			element.CoerceValue (ValueProperty);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static bool IsValidFloat (object value)
		{
			float number = (float) value;

			return !float.IsNaN (number) && !float.IsInfinity (number);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static bool IsValidValue (object value)
		{
			float? number = (float?) value;
			if (number == null) return true;

			return !float.IsNaN (number.Value) && !float.IsInfinity (number.Value);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static object CoerceMaximum (DependencyObject source, object value)
		{
			float minimum = ((ModifyTextBox) source).Minimum;
			return (float) value < minimum ? minimum : value;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		private static object ConstrainToRange (DependencyObject source, object value)
		{
			float? number = (float?) value;
			if (number == null) return value;

			var element = (ModifyTextBox) source;
			float minimum = element.Minimum;
			float maximum = element.Maximum;

			if (number.Value < minimum)
				return new Nullable<float> (minimum);

			if (number.Value > maximum)
				return new Nullable<float> (maximum);

			return value;
		}
	}
}
