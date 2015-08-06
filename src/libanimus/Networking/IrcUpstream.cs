using System;
using System.Collections.Generic;
using libanimus.Actions;
using libanimus.Networking.IRC;

namespace libanimus.Networking
{
	public class IrcUpstream : IUpstream
	{
		public IrcClient Client;
		public string Channel;

		public IrcUpstream () {
			Client = new IrcClient ();
			Actions = new List<HostAction> ();
		}

		public void Connect (string host, int port, bool ssl = true) {
			Client.Connect (host, port, ssl);
			Client.LogIn (Client.Identifier, Client.Identifier, Client.Identifier);
			Client.OnChannelMessage += (message, sender) => {
				if (message.StartsWith ("$")) {
					message = message.Substring (1);
					ActionPool.Instance.ProcessCommand (this, message);
				}
			};
		}

		public void Join (string channel) {
			Client.Join (channel);
			if (channel.StartsWith ("#"))
				Channel = channel;
		}

		#region IUpstream implementation

		public List<HostAction> Actions { get; private set; }

		public bool CheckUpdate () {
			throw new NotImplementedException ();
		}

		public void Update () {
			throw new NotImplementedException ();
		}

		public void Notify (string format, params object[] args) {
			Client.Message (string.Format (format, args), Channel);
		}

		#endregion
	}
}

