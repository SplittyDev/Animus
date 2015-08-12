using System;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionPwd : HostAction
	{
		public ActionPwd () : base ("pwd") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (!NetworkManager.Instance.IsSelected)
				return;
			
			NetworkManager.Instance.Notify (source, Environment.CurrentDirectory);
		}
	}
}

