using System;
using System.IO;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionCd : HostAction
	{
		public ActionCd () : base ("cd") {
		}

		public override void Run (IUpstream source, params string[] args) {
			try {
				if (args.Length == 1 && Directory.Exists (args [0]))
					Environment.CurrentDirectory = args [0];
				else if (args.Length > 1) {
					var path = string.Join (" ", args);
					if (Directory.Exists (path))
						Environment.CurrentDirectory = args [0];
				} else
					NetworkManager.Instance.Notify (source, "Invalid path.");
			} catch {
				NetworkManager.Instance.Notify (source, "Invalid path.");
			}
		}
	}
}

