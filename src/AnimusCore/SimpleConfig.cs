using System;
using System.IO;
using System.Linq;

namespace AnimusCore
{
	public class SimpleConfig
	{
		readonly string[] lines;

		public SimpleConfig (string path) {
			lines = File.ReadAllLines (path);
		}

		public string GetField (string field) {
			return new string (
				lines.Single (str => str.StartsWith (field))
				.SkipWhile (c => c != ':')
				.Skip (1)
				.ToArray ());
		}
	}
}

