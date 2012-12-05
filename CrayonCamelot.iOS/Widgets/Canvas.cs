using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

using CrayonCamelot.Shared;

namespace CrayonCamelot.iOS {

	public class Canvas : UIView {

		static readonly CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();

		const int CRAYON_START = 50;
		const int CRAYON_SPACING = 30;
		List<PointF> Points = new List<PointF>();


		//set the context, image background, device orientation
		UIImage background;
		public UIImage Background {
			get { return background; }
			set {
				if (value != background)
					SetNeedsDisplay ();
				background = value;
			}
		}

		public CGLayer Drawing {
			get;
			private set;
		}

		UIInterfaceOrientation orientation;
		public UIInterfaceOrientation Orientation {
			get { return orientation; }
			set {
				if (value != orientation)
					SetNeedsDisplay ();
				orientation = value;
			}
		}

		//Crayons
		Crayon [] crayons;
		public Crayon [] Crayons {
			get { return crayons; }
			set {
				if (value != crayons)
					SetNeedsDisplay ();
				crayons = value;
			}
		}

		Crayon touchedCrayon;

		//Slider
		UISlider swatchslider;
		public UISlider swatchSlider {
			get { return swatchslider; }
			set {
				if (value != swatchslider)
					SetNeedsDisplay ();
				swatchslider = value;
			}
		}
	

		//creates the layer on which we color
		public Canvas (RectangleF frame, UIInterfaceOrientation orientation, Crayon [] crayons)
			: base (frame)
		{
			this.orientation = orientation;
			this.crayons = crayons;
			BackgroundColor = UIColor.White;
		}


		/******Touches******/
		public override void TouchesBegan (NSSet touchSet, UIEvent evt)
		{
			var touches = touchSet.ToArray<UITouch> ();
			foreach (var touch in touches) {
				touchedCrayon = GetTouchingCrayon (touch);
				if (touchedCrayon != null)
					 return; 
			}

			AddPoint (touches);
		}

		public override void TouchesMoved (NSSet touchSet, UIEvent evt)
		{
			var touches = touchSet.ToArray<UITouch> ();
			if (touchedCrayon != null) {
				if (touches.All (touch => GetTouchingCrayon (touch) == null))
					touchedCrayon = null;
				return;
			}

			AddPoint (touches);
		}

		public override void TouchesEnded (NSSet touchSet, UIEvent evt)
		{
			if (touchedCrayon != null) {
				foreach (var crayon in Crayons)
					crayon.Selected = false;
				touchedCrayon.Selected = true;

			} else {

				AddPoint (touchSet.ToArray<UITouch> ());
				DrawPoints (Drawing.Context);
				Points.Clear ();
			}
			SetNeedsDisplay ();
		}

		Crayon GetTouchingCrayon (UITouch touch)
		{
			var location = touch.LocationInView (this);

			var pos = CRAYON_START - CRAYON_SPACING / 2;
			foreach (var crayon in Crayons) {

				switch (Orientation) {

				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					if ((location.X > Frame.Width - crayon.Length)
					 && (location.Y > pos) 
					 && (location.Y < pos + CRAYON_SPACING / 2 + crayon.Width + CRAYON_SPACING / 2))
						return crayon;
					break;

				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					if ((location.Y > Frame.Height - crayon.Length)
					 && (location.X < Frame.Width - pos)
					 && (location.X > Frame.Width - pos - CRAYON_SPACING / 2 - crayon.Width - CRAYON_SPACING / 2))
						return crayon;
					break;

				}

				pos += crayon.Width + CRAYON_SPACING;
			}
			return null;
		}

		void AddPoint (UITouch [] touches)
		{
			foreach (var touch in touches) {
				PointF point = touch.LocationInView(this);
				Points.Add (point);
			}
			SetNeedsDisplay ();
		}

		//draw our coloring screen
		public override void Draw (RectangleF rect)
		{
			var ctx = UIGraphics.GetCurrentContext ();
			var frame = Frame;
			var imageSize = background.Size;

			ctx.SaveState ();
			ctx.TranslateCTM (0, frame.Height);
			ctx.ScaleCTM (1, -1);

			ctx.DrawImage (new RectangleF (
				frame.Width / 2  - imageSize.Width / 2,
				frame.Height / 2 - imageSize.Height / 2,
				imageSize.Width,
				imageSize.Height),
			    background.CGImage);

			ctx.RestoreState ();

			if (Drawing != null)
					ctx.DrawLayer (Drawing, PointF.Empty);
			if (Points.Any ()) {
				if (Drawing == null)
					Drawing = CGLayer.Create (ctx, frame.Size);
				DrawPoints (ctx);
			}

			var pos = CRAYON_START;
			foreach (var crayon in Crayons) {
				ctx.SaveState ();

				//translate depending on orientation
				switch (Orientation) {

				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					ctx.TranslateCTM (frame.Width - crayon.Length, pos);
					ctx.RotateCTM (-(float)Math.PI / 2f);
					break;

				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					ctx.TranslateCTM (frame.Width - pos - crayon.Width, frame.Height - crayon.Length);
					break;

				default:
					Application.Log ("WARN: Cannot detect device orientation");
					break;
				}

				DrawCrayon (ctx, crayon);

				ctx.RestoreState ();
				pos += crayon.Width + CRAYON_SPACING;
			}
		}

		//does the actual coloring
		void DrawPoints (CGContext dctx)
		{
			dctx.BeginPath ();
			dctx.MoveTo (Points.First().X, Points.First().Y);
			dctx.SetLineWidth(swatchSlider.Value);
			foreach (var crayon in Crayons) {
				if(crayon.Selected) {
					if (crayon.Name == "Eraser") {
						dctx.SetBlendMode  (CGBlendMode.Copy);
						dctx.SetStrokeColor (UIColor.Clear.CGColor);
					} else {
						dctx.SetBlendMode  (CGBlendMode.Normal);
						dctx.SetStrokeColor (crayon.R / 255f, crayon.G / 255f, crayon.B / 255f, 1f);
					}
					dctx.SetLineCap (CGLineCap.Round);
				}
			}
			//set fill color with current crayons
			foreach (var point in Points) {
				dctx.AddLineToPoint(point.X, point.Y);
			}
			dctx.StrokePath();
		}

		void DrawCrayon (CGContext context, Crayon crayon)
		{
			// I <3 Paintcode

			//// Color Declarations
			var gradientColor  = UIColor.FromRGBA(crayon.R / 255f, crayon.G / 255f, crayon.B / 255f, 1.00f);
			var gradientColor2 = UIColor.FromRGBA(crayon.R / 255f, crayon.G / 255f, crayon.B / 255f, 0.69f);
			var gradientColor3 = UIColor.FromRGBA(crayon.R / 255f, crayon.G / 255f, crayon.B / 255f, 0.37f);

			//// Gradient Declarations
			var gradientColors = new CGColor [] {
				gradientColor.CGColor,
				gradientColor2.CGColor,
				gradientColor3.CGColor,
				gradientColor2.CGColor,
				gradientColor.CGColor
			};
			var gradientLocations = new float [] {0, 0.37f, 0.66f, 1, 1};
			var gradient = new CGGradient(colorSpace, gradientColors, gradientLocations);

			//// Rectangle Drawing
			var rectanglePath = UIBezierPath.FromRoundedRect(new RectangleF(0.5f, 20.5f, 25, 100), UIRectCorner.TopLeft | UIRectCorner.TopRight, new SizeF(8, 8));
			context.SaveState();
			rectanglePath.AddClip();
			context.DrawLinearGradient(gradient, new PointF(0.5f, 70.5f), new PointF(25.5f, 70.5f), 0);
			context.RestoreState();

			//// Bezier Drawing
			UIBezierPath bezierPath = new UIBezierPath();
			bezierPath.MoveTo(new PointF(3.5f, 24.5f));
			bezierPath.AddCurveToPoint(new PointF(12, 0.09f), new PointF(6.5f, 17.57f), new PointF(9.79f, -4.27f));
			bezierPath.AddCurveToPoint(new PointF(22.5f, 24.5f), new PointF(16.3f, 8.57f), new PointF(22.5f, 24.5f));
			context.SaveState();
			bezierPath.AddClip();
			context.DrawLinearGradient(gradient, new PointF(3.5f, 12.01f), new PointF(22.5f, 12.01f), 0);
			context.RestoreState();
		}
	}
}

