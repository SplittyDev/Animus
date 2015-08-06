using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionUpdate : HostAction
	{
		IUpstream src;

		public ActionUpdate () : base ("update") {
		}

		public override void Run (IUpstream source, params string[] args) {
			src = source;

			if (args.Length != 1) {
				NetworkManager.Instance.Notify (src, "Please provide an URI.");
				return;
			}

			Updater.Instance.UpdateAvailable += UpdateAvailableCallback;
			Updater.Instance.NoUpdateAvailable += NoUpdateAvailableCallback;

			Updater.Instance.CheckUpdate (args.First ());
		}

		void NoUpdateAvailableCallback (object sender, EventArgs e) {
			Updater.Instance.NoUpdateAvailable -= NoUpdateAvailableCallback;
			NetworkManager.Instance.Notify (src, "No updates available.");
		}

		void UpdateAvailableCallback (object sender, UpdateAvailableEventArgs e) {
			Updater.Instance.UpdateAvailable -= UpdateAvailableCallback;
			NetworkManager.Instance.Notify (src, "Update available.");
			NetworkManager.Instance.Notify (src, "[CurVer] {0}", e.currentVersion);
			NetworkManager.Instance.Notify (src, "[NewVer] {0}", e.newVersion);
		}
	}
}

