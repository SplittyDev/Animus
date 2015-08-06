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
				NetworkManager.Instance.NotifySource (source, "Please provide an URI.");
				return;
			}

			Updater.Instance.UpdateAvailable += UpdateAvailableCallback;
			Updater.Instance.NoUpdateAvailable += NoUpdateAvailableCallback;

			Updater.Instance.CheckUpdate (args.First ());
		}

		static void NoUpdateAvailableCallback (object sender, EventArgs e) {
			Updater.Instance.NoUpdateAvailable -= NoUpdateAvailableCallback;
			NetworkManager.Instance.Notify ("No updates available.");
		}

		void UpdateAvailableCallback (object sender, UpdateAvailableEventArgs e) {
			Updater.Instance.UpdateAvailable -= UpdateAvailableCallback;
			Updater.Instance.DownloadUpdate (args.First ());
		}
	}
}

