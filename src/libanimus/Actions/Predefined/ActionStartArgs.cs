using System;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionStartArgs : HostAction
	{
		public ActionStartArgs () : base ("startargs") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (!NetworkManager.Instance.IsSelected)
				return;
			
			NetworkManager.Instance.Notify (source,
				"Arguments: {0}", string.Join (" ", Environment.GetCommandLineArgs ()));
		}
	}
}

