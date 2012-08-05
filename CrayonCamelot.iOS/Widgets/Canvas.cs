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

		const int CRAYON_START = 20;
		const int CRAYON_SPACING = 30;
		List<PointF> Points = new List<PointF>();

		UIImage image;
		public UIImage Image {
			get { return image; }
			set {
				if (value != image)
					SetNeedsDisplay ();
				image = value;
			}
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

		public Canvas (RectangleF frame, UIInterfaceOrientation orientation, Crayon [] crayons)
			: base (frame)
		{
			this.orientation = orientation;
			this.crayons = crayons;
			BackgroundColor = UIColor.White;
		}

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
				//save as bitmap
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

		public override void Draw (RectangleF rect)
		{
			var ctx = UIGraphics.GetCurrentContext ();
			var frame = Frame;
			var imageSize = image.Size;

			ctx.SaveState ();
			ctx.TranslateCTM (0, frame.Height);
			ctx.ScaleCTM (1, -1);

			ctx.DrawImage (new RectangleF (
				frame.Width / 2  - imageSize.Width / 2,
				frame.Height / 2 - imageSize.Height / 2,
				imageSize.Width,
				imageSize.Height),
			    image.CGImage);

			ctx.RestoreState ();

			if(Points.Any()) {
				ctx.BeginPath();
				ctx.MoveTo (Points.First().X, Points.First().Y);
				//set fill color with current crayons
				foreach (var point in Points) {
					ctx.AddLineToPoint(point.X, point.Y);
				}
				ctx.StrokePath();
			}

			var pos = CRAYON_START;
			foreach (var crayon in Crayons) {
				ctx.SaveState ();

				//translate depending on orientation
				switch (Orientation) {

				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					ctx.TranslateCTM (frame.Width - crayon.Length, frame.Height - pos);
					ctx.RotateCTM (-(float)Math.PI / 2f);
					break;

				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					ctx.TranslateCTM (frame.Width - pos - crayon.Width, 0);
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

		public void DrawCrayon (CGContext ctx, Crayon crayon)
		{
			ctx.BeginPath ();
			ctx.SetFillColor (crayon.R / 255f, crayon.G / 255f, crayon.B / 255f, 1f);
			ctx.AddRect (new RectangleF (0, 0, crayon.Width, crayon.Length));
			ctx.FillPath ();
		}
	}
}

