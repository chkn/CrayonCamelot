using System;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace CrayonCamelot.iOS {

	public class ColoringViewController : UIViewController {

		UIImage image;

		public ColoringViewController (UIImage image, string imageTitle)
		{
			this.image = image;
			this.Title = imageTitle;
		}

		public override void LoadView ()
		{
			base.LoadView ();

			var canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame);
			canvas.Image = image;

			View = canvas;
		}
	}
}

