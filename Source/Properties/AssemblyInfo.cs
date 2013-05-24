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

//----------------------------------------------------------------------------//
// Prefaces                                                                   //
//----------------------------------------------------------------------------//

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



//----------------------------------------------------------------------------//
// Application                                                                //
//----------------------------------------------------------------------------//

#if WIN64
	[assembly: AssemblyProduct		("Sensor Simulator x64")]
#else
	[assembly: AssemblyProduct		("Sensor Simulator x32")]
#endif

[assembly: AssemblyTitle		("Sensor Simulator")]

#if DEBUG
	[assembly: AssemblyConfiguration	("Debug")]
#else
	[assembly: AssemblyConfiguration	("Release")]
#endif

[assembly: AssemblyCopyright	("(C) 2011-2013 D. Krutsko and A. Shukla")]



//----------------------------------------------------------------------------//
// Version                                                                    //
//----------------------------------------------------------------------------//

////////////////////////////////////////////////////////////////////////////////
// Major.Minor.Build.Revision
// Build equals to the number of days after January 1, 2000.
// Revision equals to the number of seconds past midnight / 2.
//   Daylight savings time is ignored.

[assembly: AssemblyVersion			("1.0.*")]



//----------------------------------------------------------------------------//
// COM                                                                        //
//----------------------------------------------------------------------------//

[assembly: ComVisible				(false)]
