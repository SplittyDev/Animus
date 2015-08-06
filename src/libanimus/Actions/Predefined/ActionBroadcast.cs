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
			ActionPool.Instance.ProcessCommand (new Command {
				Name = args.First (),
				Args = args.Skip (1).ToArray (),
				Source = source,
			});
		}
	}
}

