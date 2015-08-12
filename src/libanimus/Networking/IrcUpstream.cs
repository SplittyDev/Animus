using System;
using System.Collections.Generic;
using libanimus.Actions;

namespace libanimus.Networking
{
	public class IrcUpstream : IUpstream
	{
		public IrcClient Client;
		public string Channel;

		string Host;
		int Port;
		bool SSL;

		public event EventHandler Connected;

		public IrcUpstream () {
			Client = new IrcClient ();
			Actions = new List<HostAction> ();
		}

		public void Connect (string host, int port, bool ssl = true) {
			Host = host;
			Port = port;
			SSL = ssl;
			Client = new IrcClient ();
			Client.Disconnected += _Connect;
			Client.ChannelMessage += (message, sender) => {
				if (message.StartsWith ("$")) {
					message = message.Substring (1);
					ActionPool.Instance.ProcessCommand (this, message);
				}
			};
			_Connect (this, EventArgs.Empty);
		}

		void _Connect (object sender, EventArgs e) {
			if (sender != this)
				Console.WriteLine ("Catched OnDisconnect event. Reconnecting...");
			Client.Connect (Host, Port, SSL);
			var ident = NetworkManager.Instance.Identifier;
			Client.LogIn (ident, ident, ident);
			Connected (this, EventArgs.Empty);
		}

		public void Join (string channel) {
			Client.Join (channel);
			if (channel.StartsWith ("#"))
				Channel = channel;
		}

		#region IUpstream implementation

		public List<HostAction> Actions { get; private set; }

		public void Notify (string format, params object[] args) {
			Client.Message (string.Format (format, args), Channel);
		}

		#endregion
	}
}

