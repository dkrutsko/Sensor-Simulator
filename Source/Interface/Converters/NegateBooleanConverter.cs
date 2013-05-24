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
	using System.Windows.Data;
	using System.Windows.Markup;
	using System.Globalization;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Converts a boolean to its negated equivalent. </summary>

	[ValueConversion (typeof (bool), typeof (bool))]
	public class NegateBooleanConverter : MarkupExtension, IValueConverter
	{
		//----------------------------------------------------------------------------//
		// Static                                                                     //
		//----------------------------------------------------------------------------//

		private static NegateBooleanConverter mInstance = null;



		//----------------------------------------------------------------------------//
		// MarkupExtension                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			if (mInstance == null)
				mInstance = new NegateBooleanConverter();

			return mInstance;
		}



		//----------------------------------------------------------------------------//
		// IValueConverter                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public object Convert (object value,
			Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool) value;
		}

		////////////////////////////////////////////////////////////////////////////////

		public object ConvertBack (object value,
			Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool) value;
		}
	}
}
