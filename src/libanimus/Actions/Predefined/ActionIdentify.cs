using System;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionIdentify : HostAction
	{
		public ActionIdentify () : base ("identify") {
		}

		public override void Run (IUpstream source, params string[] args) {
			NetworkManager.Instance.Notify (source, NetworkManager.Instance.FullGuid ());
		}
	}
}

