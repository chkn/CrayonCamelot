using System;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using CrayonCamelot.Shared;

namespace CrayonCamelot.iOS {

	public class ColoringViewController : UIViewController {

		Canvas  canvas;
		UIImage image;

		public ColoringViewController (UIImage image, string imageTitle)
		{
			this.image = image;
			this.Title = imageTitle;
		}

		public override void LoadView ()
		{
			base.LoadView ();

			canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame, InterfaceOrientation, Application.Crayons);
			canvas.Image = image;

			View = canvas;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			canvas.Orientation = toInterfaceOrientation;
		}
	}
}

