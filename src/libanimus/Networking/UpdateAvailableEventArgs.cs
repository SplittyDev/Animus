using System;

namespace libanimus.Networking
{
	public class UpdateAvailableEventArgs : EventArgs
	{
		public Version currentVersion;
		public Version newVersion;

		public UpdateAvailableEventArgs (Version current, Version @new) {
			currentVersion = current;
			newVersion = @new;
		}
	}
}

