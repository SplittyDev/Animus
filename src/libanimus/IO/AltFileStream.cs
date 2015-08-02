using System;
using System.IO;

namespace libanimus
{
	public class AltFileStream : FileStream
	{
		public AltFileStream (string path, string stream,
			FileMode mode = FileMode.OpenOrCreate,
			FileAccess access = FileAccess.ReadWrite,
			FileShare share = FileShare.None)
			: base (path, mode, access, share) {
		}
	}
}

