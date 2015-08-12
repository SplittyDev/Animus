using System;
using libanimus.Networking;
using System.Linq;

namespace libanimus.Actions.Predefined
{
	public class ActionBroadcast : HostAction
	{
		public ActionBroadcast () : base ("broadcast") {
		}

		public override void Run (IUpstream source, params string[] args) {
			var backup = NetworkManager.Instance.SelectedIdentifier;
			NetworkManager.Instance.SelectedIdentifier = NetworkManager.Instance.Identifier;
			try {
				ActionPool.Instance.ProcessCommand (new Command {
					Name = args.First (),
					Args = args.Skip (1).ToArray (),
					Source = source,
				});
			} catch {
				NetworkManager.Instance.Notify (source, "An exception occurred while processing the command.");
			}
			NetworkManager.Instance.SelectedIdentifier = backup;
		}
	}
}

