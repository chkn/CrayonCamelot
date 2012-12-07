using System;
using System.IO;
using System.Collections.Generic;
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
		List<UIImage> swatches;
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

			//load our swatches (transparent, colorless pngs that we then add color to, and use as a textured brush)
			var swatchesDirectory = Path.Combine (NSBundle.MainBundle.BundlePath, "Swatches");
			swatches = new List<UIImage>();
			foreach (var swatchFile in Directory.EnumerateFiles (swatchesDirectory)) {
				var swatch = UIImage.FromFile (swatchFile);
				if (swatch == null) {
					Application.Log ("WARN: couldn't load swatch: {0}", swatchFile);
					continue;
				}
				swatches.Add(swatch);
			}

			//make our brush size slider
			swatchSlider = new UISlider(new RectangleF(50, 30, 250, 50));
			swatchSlider.MinValue = 1;
			swatchSlider.MaxValue = 40;
			swatchSlider.Value = 10;
			swatchSlider.MinimumTrackTintColor = UIColor.FromRGB(selectedCrayon.R/255f, selectedCrayon.G/255f, selectedCrayon.B/255f);
			swatchSlider.MaximumTrackTintColor = UIColor.LightGray;


			//generate our clear and save buttons
			//TODO: this should happen in a split-view, so the user can show and hide
			redoButton = UIButton.FromType(UIButtonType.RoundedRect);
			saveButton = UIButton.FromType(UIButtonType.RoundedRect);
			redoButton.Frame = new RectangleF(100, 825, 200, 100);
			saveButton.Frame = new RectangleF(400, 825, 200, 100);
			redoButton.SetTitle ("Start Over", UIControlState.Normal);
			saveButton.SetTitle ("I'm Done!", UIControlState.Normal);
			redoButton.TouchUpInside += ClearCanvas;
			saveButton.TouchUpInside += SaveAsImage;
		

			//TODO: manage slider position wrt screen orientation
			canvas = new Canvas (UIScreen.MainScreen.ApplicationFrame, InterfaceOrientation, Application.Crayons, swatches);
			canvas.Background = image;
			canvas.swatchSlider = swatchSlider;

			canvas.AddSubviews (swatchSlider, redoButton, saveButton);

			//load this puppy
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
			NSError err;

			UIGraphics.BeginImageContext (this.View.Frame.Size);
			using ( var context = UIGraphics.GetCurrentContext () ) {
				if ( context != null ) {
					//TODO: Save just the background and the user's drawing
					this.View.Layer.RenderInContext ( context );
					UIImage fuckingImage = UIGraphics.GetImageFromCurrentImageContext ();

					UIGraphics.EndImageContext ();
					string location = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);

					fuckingImage.AsPNG ().Save( Path.Combine (location, Title + ".png"), true, out err );

					Console.WriteLine ("saved image to {0}", location);
				}
			}

		}
	}
}

