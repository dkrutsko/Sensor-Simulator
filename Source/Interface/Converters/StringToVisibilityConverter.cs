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
	using System.Windows.Markup;
	using System.Globalization;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Converts a string to Visibility. </summary>

	[ValueConversion (typeof (String), typeof (Visibility))]
	public class StringToVisibilityConverter : MarkupExtension, IValueConverter
	{
		//----------------------------------------------------------------------------//
		// Static                                                                     //
		//----------------------------------------------------------------------------//

		private static StringToVisibilityConverter mInstance = null;



		//----------------------------------------------------------------------------//
		// MarkupExtension                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			if (mInstance == null)
				mInstance = new StringToVisibilityConverter();

			return mInstance;
		}



		//----------------------------------------------------------------------------//
		// IValueConverter                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public object Convert (object value,
			Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty ((string) value) ?
				Visibility.Hidden : Visibility.Visible;
		}

		////////////////////////////////////////////////////////////////////////////////

		public object ConvertBack (object value,
			Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
