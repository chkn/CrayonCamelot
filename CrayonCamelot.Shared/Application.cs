using System;
using System.Diagnostics;
using System.IO;

namespace CrayonCamelot.Shared {

	public static class Application {

		const string CRAYONS_FILE = "Crayons.xml";

		static Crayon [] crayons;
		public static Crayon [] Crayons {
			get {
				if (crayons == null) {
					crayons = Crayon.LoadCrayons (CRAYONS_FILE);
					crayons [0].Selected = true;
				}
				return crayons;
			}
		}


		[Conditional ("DEBUG")]
		public static void Log (string format, params object [] args)
		{
			Console.WriteLine (format, args);
		}
	 
	}
}

