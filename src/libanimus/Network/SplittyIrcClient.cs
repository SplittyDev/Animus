using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using NetIrc2;

namespace libanimus
{
	public class SplittyIrcClient
	{
		readonly ICollection<HostAction> actions;
		readonly NetIrc2.IrcClient client;
		readonly Guid guid;
		readonly string id;

		public SplittyIrcClient () {
			actions = new List<HostAction> ();
			client = new NetIrc2.IrcClient ();
			guid = Guid.NewGuid ();
			id = string.Format ("animus{0}", new string (guid.ToString ("N").Take (16).ToArray ()));
		}

		public void Connect (string server, int port, bool useSsl = true) {
			var options = new IrcClientConnectionOptions { Ssl = useSsl, SslHostname = server };
			options.SslCertificateValidationCallback =
				new RemoteCertificateValidationCallback ((sender, certificate, chain, sslPolicyErrors) => true);
			client.Connect (server, port, options);
			client.LogIn (id, id, id);
			client.Join ("#OperationMythicDawn");
			client.GotChatAction += Client_GotChatAction;
		}

		void Client_GotChatAction (object sender, NetIrc2.Events.ChatMessageEventArgs e) {
			if (e.Sender == null || e.Sender.ToIrcString () != "#OperationMythicDawn")
				return;
			var acts = actions.Where (act => e.Message.StartsWith (act.Name));
			foreach (var act in acts)
				act.Run (Command.Parse (e.Message));
		}

		public bool IsConnected () {
			return client.IsConnected;
		}

		public void Join (string channel) {
			client.Join (channel);
		}

		public void Send (string msg) {
			client.ChatAction ("#OperationMythicDawn", msg);
		}

		public void AddAction (HostAction action) {
			actions.Add (action);
		}
	}
}

