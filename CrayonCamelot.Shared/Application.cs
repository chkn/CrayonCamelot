using System;

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
	 
	}
}

