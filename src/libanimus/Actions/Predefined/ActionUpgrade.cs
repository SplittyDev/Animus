using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionUpgrade : HostAction
	{
		public ActionUpgrade () : base ("upgrade") {
		}

		public override void Run (IUpstream source, params string[] args) {
			
			if (args.Length != 1) {
				NetworkManager.Instance.Notify ("Please provide an URI.");
				return;
			}

			Updater.Instance.DownloadUpdate (args.First ());
		}
	}
}

