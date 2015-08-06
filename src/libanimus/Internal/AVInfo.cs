using System;
using System.IO;

namespace libanimus.Internal
{
	public class AVInfo
	{
		public string Name;
		public string Company;
		public string RelativePath;
		public bool Generic;

		public AVInfo (string company, string path, string name = null, bool generic = false) {
			Name = generic ? "(Generic)" : string.IsNullOrEmpty (name) ? "(Unknown)" : name;
			Company = company;
			RelativePath = path;
			Generic = generic;
		}

		public bool IsMatch () {
			var root = Path.GetPathRoot (Environment.GetFolderPath (Environment.SpecialFolder.ProgramFiles));
			var result1 = Directory.Exists (Path.Combine (root, "Program Files", RelativePath));
			var result2 = result1;
			if (Directory.Exists (Path.Combine (root, "Program Files (x86)")))
				result2 = Directory.Exists (Path.Combine (root, "Program Files (x86)", RelativePath));
			return result1 || result2;
		}
	}
}

