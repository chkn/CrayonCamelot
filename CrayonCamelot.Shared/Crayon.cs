using System;
using System.IO;
using System.Xml.Serialization;

namespace CrayonCamelot.Shared {

	public class Crayon {

		const int DESELECTED_LENGTH = 100;
		const int SELECTED_LENGTH = 125;
		const int WIDTH = 25;

		[XmlAttribute]
		public string Name {
			get;
			set;
		}

		[XmlAttribute]
		public int R {
			get;
			set;
		}

		[XmlAttribute]
		public int G {
			get;
			set;
		}

		[XmlAttribute]
		public int B {
			get;
			set;
		}

		[XmlIgnore]
		public bool Selected {
			get;
			set;
		}

		[XmlIgnore]
		public int Length {
			get {
				return Selected ? SELECTED_LENGTH : DESELECTED_LENGTH;
			}
		}

		[XmlIgnore]
		public int Width {
			get {
				return WIDTH;
			}
		}

		static XmlSerializer serializer;
		static XmlSerializer Serializer {
			get {
				if (serializer == null)
					serializer = new XmlSerializer (typeof (Crayon []));
				return serializer;
			}
		}

		public static Crayon [] LoadCrayons (string fileName)
		{
			using (var stream = File.OpenRead (fileName))
				return (Crayon [])Serializer.Deserialize (stream);
		}

		public static void SaveCrayons (string fileName, Crayon [] crayons)
		{
			using (var stream = File.Create (fileName))
				Serializer.Serialize (stream, crayons);
		}
	}
}

