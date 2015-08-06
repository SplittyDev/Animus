using System;
using System.Diagnostics;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionQuit : HostAction
	{
		public ActionQuit () : base ("quit") {
		}

		public override void Run (IUpstream source, params string[] args) {
			Process.GetCurrentProcess ().Kill ();
		}
	}
}

