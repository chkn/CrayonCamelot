using System;
using System.IO;
using System.Linq;

using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

using CrayonCamelot.Shared;

namespace CrayonCamelot.iOS {

	public class ImageListViewController : DialogViewController {

		public ImageListViewController ()
			: base (null, true)
		{
			Title = "Crayon Camelot";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var section = new Section ();

			var imagesDirectory = Path.Combine (NSBundle.MainBundle.BundlePath, "Images");
			foreach (var imageFile in Directory.EnumerateFiles (imagesDirectory)) {
				var image = UIImage.FromFile (imageFile);
				if (image == null) {
					Application.Log ("WARN: couldn't load image: {0}", imageFile);
					continue;
				}

				var title = Path.GetFileNameWithoutExtension (imageFile);

				section.Add (new BadgeElement (image, title, () => {
					NavigationController.PushViewController (new ColoringViewController (image, title), true);
				}));
			}

			Root = new RootElement (null) { section };
		}
	}
}

