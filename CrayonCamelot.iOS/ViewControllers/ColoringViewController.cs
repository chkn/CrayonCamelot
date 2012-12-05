using System;
using System.Drawing;
using MonoTouch.AssetsLibrary;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreImage;
using MonoTouch.CoreGraphics;

using CrayonCamelot.Shared;

namespace CrayonCamelot.iOS {

	public class ColoringViewController : UIViewController {

		Canvas  canvas;
		UIImage image;
		UISlider swatchSlider;

		public ColoringViewController (UIImage image, string imageTitle)
		{
			this.image = image;
			this.Title = imageTitle;
		}

		public override void LoadView ()
		{
			base.LoadView ();

			swatchSlider = new UISlider(new RectangleF(50,  30, 250, 50));
			swatchSlider.MinValue = 1;
			swatchSlider.MaxValue = 40;
			swatchSlider.Value = 10;

			canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame, InterfaceOrientation, Application.Crayons);
			canvas.Background = image;
			canvas.swatchSlider = swatchSlider;


			canvas.Add (swatchSlider);

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

