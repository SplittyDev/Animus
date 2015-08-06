using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionAutoUpgrade : HostAction
	{
		public ActionAutoUpgrade () : base ("autoupgrade") {
		}

		string[] args;

		public override void Run (IUpstream source, params string[] args) {
			this.args = args;

			if (args.Length != 1) {
				NetworkManager.Instance.Notify ("Please provide an URI.");
				return;
			}

			Updater.Instance.UpdateAvailable -= UpdateAvailableCallback;
			Updater.Instance.UpdateAvailable += UpdateAvailableCallback;

			Updater.Instance.NoUpdateAvailable -= NoUpdateAvailableCallback;
			Updater.Instance.NoUpdateAvailable += NoUpdateAvailableCallback;

			Updater.Instance.CheckUpdate (args.First ());
		}

		static void NoUpdateAvailableCallback (object sender, EventArgs e) {
			NetworkManager.Instance.Notify ("No updates available.");
		}

		void UpdateAvailableCallback (object sender, UpdateAvailableEventArgs e) {
			Updater.Instance.DownloadUpdate (args.First ());
		}
	}
}

