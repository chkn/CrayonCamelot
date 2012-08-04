using System;
using System.Diagnostics;

namespace CrayonCamelot.Shared {

	public static class Application {

		const string CRAYONS_FILE = "Crayons.xml";

		static Crayon [] crayons;
		public static Crayon [] Crayons {
			get {
				if (crayons == null)
					crayons = Crayon.LoadCrayons (CRAYONS_FILE);
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

