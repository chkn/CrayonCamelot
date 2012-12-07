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

		Canvas canvas;
		UIImage image;
		Crayon selectedCrayon;
		UISlider swatchSlider;
		UIButton redoButton;
		UIButton saveButton;

		public ColoringViewController (UIImage image, string imageTitle)
		{
			this.image = image;
			this.Title = imageTitle;
		}

		public override void LoadView ()
		{
			base.LoadView ();

			//get the latest selected crayon
			foreach (var crayon in Application.Crayons) {
				if(crayon.Selected)
					selectedCrayon = crayon;
			}

			swatchSlider = new UISlider(new RectangleF(50, 30, 250, 50));
			swatchSlider.MinValue = 1;
			swatchSlider.MaxValue = 40;
			swatchSlider.Value = 10;
			swatchSlider.MinimumTrackTintColor = UIColor.FromRGB(selectedCrayon.R/255f, selectedCrayon.G/255f, selectedCrayon.B/255f);
			swatchSlider.MaximumTrackTintColor = UIColor.LightGray;

			redoButton = UIButton.FromType(UIButtonType.RoundedRect);
			saveButton = UIButton.FromType(UIButtonType.RoundedRect);
			redoButton.Frame = new RectangleF(100, 825, 200, 100);
			saveButton.Frame = new RectangleF(400, 825, 200, 100);
			redoButton.SetTitle ("Start Over", UIControlState.Normal);
			saveButton.SetTitle ("I'm Done!", UIControlState.Normal);
			redoButton.TouchUpInside += ClearCanvas;
			saveButton.TouchUpInside += SaveAsImage;

			//TODO: manage slider position wrt screen orientation
			canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame, InterfaceOrientation, Application.Crayons);
			canvas.Background = image;
			canvas.swatchSlider = swatchSlider;

			canvas.AddSubviews (swatchSlider, redoButton, saveButton);

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

		protected void ClearCanvas (object sender, System.EventArgs e)
		{
			LoadView ();
		}

		protected void SaveAsImage (object sender, System.EventArgs e)
		{
			//
		}
	}
}

