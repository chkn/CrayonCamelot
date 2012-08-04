using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using CrayonCamelot.Shared;

namespace CrayonCamelot.iOS {

	public class Canvas : UIView {

		UIImage image;
		public UIImage Image {
			get { return image; }
			set {
				if (value != image)
					SetNeedsDisplay ();
				image = value;
			}
		}

		public Crayon [] Crayons {
			get;
			private set;
		}

		public Canvas (RectangleF frame, Crayon [] crayons)
			: base (frame)
		{
			Crayons = crayons;
			BackgroundColor = UIColor.White;
		}

		public override void Draw (RectangleF rect)
		{
			var ctx = UIGraphics.GetCurrentContext ();
			var frame = Frame;
			var imageSize = image.Size;

			ctx.TranslateCTM (0, frame.Height);
			ctx.ScaleCTM (1, -1);

			ctx.DrawImage (new RectangleF (
				frame.Width / 2  - imageSize.Width / 2,
				frame.Height / 2 - imageSize.Height / 2,
				imageSize.Width,
				imageSize.Height),
			    image.CGImage);
		}
	}
}

