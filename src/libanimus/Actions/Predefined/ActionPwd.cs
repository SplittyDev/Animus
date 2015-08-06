using System;

namespace libanimus
{
	public class ActionPwd : HostAction
	{
		public ActionPwd () : base ("pwd") {
		}

		public override void Run (IUpstream source, params string[] args) {
			NetworkManager.Instance.NotifySource (source, Environment.CurrentDirectory);
		}
	}
}

