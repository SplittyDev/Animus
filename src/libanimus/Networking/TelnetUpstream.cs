using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace libanimus
{
	public class TelnetUpstream : IUpstream
	{
		readonly List<TcpClient> clients;
		readonly TcpListener listener;
		readonly Dictionary<TcpClient, List<string>> notifications;
		readonly List<string> globalLog;

		public TelnetUpstream () {
			clients = new List<TcpClient> ();
			Actions = new List<HostAction> ();
			globalLog = new List<string> ();
			notifications = new Dictionary<TcpClient, List<string>> ();
			var local = IPAddress.Any;
			listener = new TcpListener (local, 23);
			listener.Start ();
			Task.Factory.StartNew (Listen);
		}

		void Listen () {
			while (true) {
				var client = listener.AcceptTcpClient ();
				notifications [client] = new List<string> ();
				clients.Add (client);
				Task.Factory.StartNew (() => HandleClient (client));
			}
		}

		void HandleClient (TcpClient client) {
			SendRaw (client, "Animus Telnet Interface\r\n");
			FlushNotifications (client, global: true);
			SendPrompt (client);
			using (var reader = new StreamReader (client.GetStream ())) {
				while (client.Connected) {
					try {
						var line = reader.ReadLine ();
						FlushNotifications (client, beforeCommand: true);
						ActionPool.Instance.ProcessCommandOn (Actions, this, line);
						ActionPool.Instance.ProcessCommand (this, line);
						FlushNotifications (client, afterCommand: true);
						SendPrompt (client);
					} catch (Exception e) {
						// Log the exception here
						SendLine (client, "\r\nEXCEPTION: {0}", e.Message);
					}
				}
			}
		}

		void SendPrompt (TcpClient client) {
			var dir = Environment.CurrentDirectory;
			dir = new string (dir.Skip (dir.Last (c => c == '\\')).ToArray ());
			SendRaw (client, "{0}@{1}:{2}$ ", Environment.UserName, Environment.MachineName, dir);
		}

		void FlushNotifications (TcpClient client, bool @global = false, bool beforeCommand = false, bool afterCommand = false) {
			if (@global && globalLog.Count > 0) {
				SendLine (client, "--- log");
				globalLog.ForEach (str => SendLine (client, str));
				SendLine (client, "---");
			} else if (notifications [client].Count == 0)
				return;
			if (beforeCommand) {
				SendLine (client, "--- log");
				notifications [client].ForEach (str => SendLine (client, str));
				SendLine (client, "---");
				notifications [client].Clear ();
			} else if (afterCommand) {
				notifications [client].ForEach (str => SendLine (client, str));
				notifications [client].Clear ();
			}
		}

		void SendRaw (TcpClient client, string format, params object[] args) {
			var data = Encoding.ASCII.GetBytes (string.Format (format, args));
			client.GetStream ().Write (data, 0, data.Length);
			client.GetStream ().Flush ();
		}

		void SendLine (TcpClient client, string format, params object[] args) {
			SendRaw (client, string.Format ("{0}\r\n", string.Format (format, args)));
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
			globalLog.Add (string.Format (format, args));
			clients.ForEach (client => notifications[client].Add (string.Format (format, args)));
		}

		#endregion
	}
}

