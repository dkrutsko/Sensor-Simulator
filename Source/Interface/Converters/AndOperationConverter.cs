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
	/// <summary> Inputs multiple boolean values and applies an and operation. </summary>

	[ValueConversion (typeof (bool), typeof (bool))]
	public class AndOperationConverter : MarkupExtension, IMultiValueConverter
	{
		//----------------------------------------------------------------------------//
		// Static                                                                     //
		//----------------------------------------------------------------------------//

		private static AndOperationConverter mInstance = null;



		//----------------------------------------------------------------------------//
		// MarkupExtension                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			if (mInstance == null)
				mInstance = new AndOperationConverter();

			return mInstance;
		}



		//----------------------------------------------------------------------------//
		// IMultiValueConverter                                                       //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public object Convert (object[] values,
			Type targetType, object parameter, CultureInfo culture)
		{
			foreach (bool value in values)
				if (!value) return false;

			return true;
		}

		////////////////////////////////////////////////////////////////////////////////

		public object[] ConvertBack (object value,
			Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return Array.ConvertAll (targetTypes, t => value);
		}
	}
}
