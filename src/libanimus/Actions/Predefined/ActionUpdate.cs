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
			if (!NetworkManager.Instance.IsSelected)
				return;
			
			if (args.Length != 1) {
				NetworkManager.Instance.Notify (source, "Please provide an URI.");
				return;
			}

			Updater.Instance.DownloadUpdate (args.First ());
		}
	}
}

