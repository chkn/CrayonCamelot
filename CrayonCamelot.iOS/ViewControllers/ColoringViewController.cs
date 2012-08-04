using System;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

using CrayonCamelot.Shared;

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

			var canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame, Application.Crayons);
			canvas.Image = image;

			View = canvas;
		}
	}
}

