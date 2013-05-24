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
	using System.Windows.Media;
	using System.Windows.Shapes;

	////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a shape defining the parameters for a pie piece.
	/// http://www.codeproject.com/KB/WPF/PieChartDataBinding.aspx
	/// </summary>

	public class PiePiece : Shape
	{
		//----------------------------------------------------------------------------//
		// Properties                                                                 //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> The radius of this pie piece. </summary>

		public double Radius
		{
			get { return (double) GetValue (RadiusProperty);		}
			set { SetValue (RadiusProperty, value);					}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The distance to "push" this pie piece out from the centre.
		/// </summary>

		public double PushOut
		{
			get { return (double) GetValue (PushOutProperty);		}
			set { SetValue (PushOutProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> The inner radius of this pie piece. </summary>

		public double InnerRadius
		{
			get { return (double) GetValue (InnerRadiusProperty);	}
			set { SetValue (InnerRadiusProperty, value);			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The X coordinate of centre of the circle from which this pie piece is cut.
		/// </summary>

		public double CenterX
		{
			get { return (double) GetValue (CenterXProperty);		}
			set { SetValue (CenterXProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The Y coordinate of centre of the circle from which this pie piece is cut.
		/// </summary>

		public double CenterY
		{
			get { return (double) GetValue (CenterYProperty);		}
			set { SetValue (CenterYProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> The percentage of a full pie that this piece occupies. </summary>

		public double Percentage
		{
			get { return (double) GetValue (PercentageProperty);	}
			private set { SetValue (PercentageProperty, value);		}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> The value that this pie piece represents. </summary>

		public double PieceValue
		{
			get { return (double) GetValue (PieceValueProperty);	}
			set { SetValue (PieceValueProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> The wedge angle of this pie piece in degrees. </summary>

		public double WedgeAngle
		{
			get
			{
				return (double) GetValue (WedgeAngleProperty);
			}

			set
			{
				if (value == 360.0) value = 359.99;
				this.Percentage = (value / 360.0);
				SetValue (WedgeAngleProperty, value);
			}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// The rotation, in degrees, from the Y axis vector of this pie piece.
		/// </summary>

		public double RotationAngle
		{
			get { return (double) GetValue (RotationAngleProperty);		}
			set { SetValue (RotationAngleProperty, value);				}
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Defines the geometry. </summary>

		protected override Geometry DefiningGeometry
		{
			get
			{
				// Create a StreamGeometry for describing the shape
				StreamGeometry geometry = new StreamGeometry();
				geometry.FillRule = FillRule.EvenOdd;

				using (StreamGeometryContext context = geometry.Open())
					DrawGeometry (context);

				// Freeze the geometry for performance benefits
				geometry.Freeze();

				return geometry;
			}
		}



		//----------------------------------------------------------------------------//
		// Methods                                                                    //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> Draws the geometry. </summary>

		private void DrawGeometry (StreamGeometryContext context)
		{
			Point startPoint = new Point (CenterX, CenterY);

			Point innerArcStartPoint = ComputeCartesian (RotationAngle, InnerRadius);
			Point innerArcEndPoint   = ComputeCartesian (RotationAngle + WedgeAngle, InnerRadius);
			Point outerArcStartPoint = ComputeCartesian (RotationAngle, Radius);
			Point outerArcEndPoint   = ComputeCartesian (RotationAngle + WedgeAngle, Radius);

			innerArcStartPoint.Offset (CenterX, CenterY);
			innerArcEndPoint.Offset   (CenterX, CenterY);
			outerArcStartPoint.Offset (CenterX, CenterY);
			outerArcEndPoint.Offset   (CenterX, CenterY);

			bool largeArc = WedgeAngle > 180;

			if (PushOut > 0)
			{
				Point offset = ComputeCartesian (RotationAngle + WedgeAngle / 2, PushOut);

				innerArcStartPoint.Offset (offset.X, offset.Y);
				innerArcEndPoint.Offset   (offset.X, offset.Y);
				outerArcStartPoint.Offset (offset.X, offset.Y);
				outerArcEndPoint.Offset   (offset.X, offset.Y);

			}

			Size outerArcSize = new Size (Radius, Radius);
			Size innerArcSize = new Size (InnerRadius, InnerRadius);

			context.BeginFigure (innerArcStartPoint, true, true);

			context.LineTo (outerArcStartPoint, true, true);
			context.ArcTo  (outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
			context.LineTo (innerArcEndPoint, true, true);
			context.ArcTo  (innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
		}

		////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Converts a coordinate from the polar coordinate
		/// system to the cartesian coordinate system.
		/// </summary>

		private Point ComputeCartesian (double angle, double radius)
		{
			// Convert to radians
			double angleRad = (Math.PI / 180.0) * (angle - 90);

			double x = radius * Math.Cos (angleRad);
			double y = radius * Math.Sin (angleRad);

			return new Point (x, y);
		}



		//----------------------------------------------------------------------------//
		// XAML Properties                                                            //
		//----------------------------------------------------------------------------//

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register
			("Radius", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty PushOutProperty = DependencyProperty.Register
			("PushOut", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register
			("InnerRadius", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty CenterXProperty = DependencyProperty.Register
			("CentreX", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty CenterYProperty = DependencyProperty.Register
			("CentreY", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register
			("Percentage", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata (0.0));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty PieceValueProperty = DependencyProperty.Register
			("PieceValueProperty", typeof(double), typeof(PiePiece), new FrameworkPropertyMetadata (0.0));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty WedgeAngleProperty = DependencyProperty.Register
			("WedgeAngle", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		////////////////////////////////////////////////////////////////////////////////
		/// <summary> </summary>

		public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register
			("RotationAngle", typeof (double), typeof (PiePiece), new FrameworkPropertyMetadata
			(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
	}
}
