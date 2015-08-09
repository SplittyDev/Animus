using System;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionIrcRaw : HostAction
	{
		public ActionIrcRaw () : base ("ircraw") {
		}

		public override void Run (IUpstream source, params string[] args) {
			var irc = NetworkManager.Instance.GetUpstream<IrcUpstream> ();
			irc.Client.SendRaw (string.Join (" ", args));
		}
	}
}

