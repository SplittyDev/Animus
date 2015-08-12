using System;
using System.Diagnostics;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionKill : HostAction
	{
		public ActionKill () : base ("kill") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (!NetworkManager.Instance.IsSelected)
				return;
			
			Process.GetCurrentProcess ().Kill ();
		}
	}
}

