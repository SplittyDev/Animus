using System;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionWhoami : HostAction
	{
		public ActionWhoami () : base ("whoami") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (!NetworkManager.Instance.IsSelected)
				return;
			
			NetworkManager.Instance.Notify (source, "{0}\\{1}",
				Environment.MachineName.ToLowerInvariant (),
				Environment.UserName.ToLowerInvariant ());
		}
	}
}

