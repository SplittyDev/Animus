using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionSelect : HostAction
	{
		public ActionSelect () : base ("select") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (args.Length > 0) {
				NetworkManager.Instance.SelectedIdentifier = args.First ();
				if (NetworkManager.Instance.IsSelected)
					NetworkManager.Instance.Notify (source, "This client is now the active client.");
			}
			else
				NetworkManager.Instance.Notify (source, "Please provide the short ident as parameter.");
		}
	}
}
