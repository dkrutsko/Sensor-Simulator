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
	using System.Runtime.InteropServices;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Defines native procedures from WIN32.dll. </summary>

	public static class Win32
	{
		//----------------------------------------------------------------------------//
		// Constants                                                                  //
		//----------------------------------------------------------------------------//

		public const uint WM_MOVE				= 0x0003;
		public const uint WM_SIZE				= 0x0005;
		public const uint WM_ACTIVATE			= 0x0006;
		public const uint WM_SYSCOMMAND			= 0x0112;
		public const uint WM_NCHITTEST			= 0x0084;

		public const uint SIZE_RESTORED			= 0;
		public const uint SIZE_MINIMIZED		= 1;
		public const uint SIZE_MAXIMIZED		= 2;
		public const uint SIZE_MAXSHOW			= 3;
		public const uint SIZE_MAXHIDE			= 4;



		//----------------------------------------------------------------------------//
		// Bit                                                                        //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		public static int HiWord (int value)
		{
			return ((value >> 16) & 0xFFFF);
		}

		////////////////////////////////////////////////////////////////////////////////

		public static int LoWord (int value)
		{
			return ((value >> 0 ) & 0xFFFF);
		}
	}



	////////////////////////////////////////////////////////////////////////////////
	/// <summary> Defines native procedures from User32.dll. </summary>

	public static class User32
	{
		//----------------------------------------------------------------------------//
		// Structures                                                                 //
		//----------------------------------------------------------------------------//

		[StructLayout (LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT (int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
		}

		//----------------------------------------------------------------------------//
		// Constants                                                                  //
		//----------------------------------------------------------------------------//

		public const uint MF_STRING			= 0x0000;
		public const uint MF_SEPARATOR		= 0x0800;
		public const uint MF_BYPOSITION		= 0x0400;
		public const uint MF_CHECKED		= 0x0008;
		public const uint MF_UNCHECKED		= 0x0000;



		//----------------------------------------------------------------------------//
		// System Menu                                                                //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		[DllImport ("user32.dll")]
		public static extern IntPtr GetSystemMenu (IntPtr windowHandle, bool revert);

		////////////////////////////////////////////////////////////////////////////////

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool InsertMenu (IntPtr menuHandle,
			uint position, uint flags, uint newItemID, string newItem);

		////////////////////////////////////////////////////////////////////////////////

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool ModifyMenu (IntPtr menuHandle,
			uint position, uint flags, uint newItemID, string newItem);



		//----------------------------------------------------------------------------//
		// Clip Cursor                                                                //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////

		[DllImport ("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool SetCursorPos (int X, int Y);

		////////////////////////////////////////////////////////////////////////////////

		[DllImport("user32.dll")]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool GetCursorPos (out POINT point);
	}
}
