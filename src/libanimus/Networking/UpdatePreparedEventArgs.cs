using System;

namespace libanimus.Networking
{
	public class UpdatePreparedEventArgs : EventArgs
	{
		public string Path;

		public UpdatePreparedEventArgs (string path) {
			Path = path;
		}
	}
}

