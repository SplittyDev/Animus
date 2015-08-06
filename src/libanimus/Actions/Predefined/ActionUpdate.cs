using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionUpdate : HostAction
	{
		public ActionUpdate () : base ("update") {
		}

		public override void Run (IUpstream source, params string[] args) {
			
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

		static void UpdateAvailableCallback (object sender, UpdateAvailableEventArgs e) {
			NetworkManager.Instance.Notify ("Update available.");
			NetworkManager.Instance.Notify ("[CurVer] {0}", e.currentVersion);
			NetworkManager.Instance.Notify ("[NewVer] {0}", e.newVersion);
		}
	}
}

