using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using libanimus.Actions;

namespace libanimus.Networking
{
	/// <summary>
	/// Telnet upstream.
	/// </summary>
	public class TelnetUpstream : IUpstream
	{
		public readonly List<TcpClient> Clients;
		readonly TcpListener listener;
		readonly Dictionary<TcpClient, List<string>> notifications;
		readonly List<string> globalLog;

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.Networking.TelnetUpstream"/> class.
		/// </summary>
		public TelnetUpstream () {
			Clients = new List<TcpClient> ();
			Actions = new List<HostAction> ();
			globalLog = new List<string> ();
			notifications = new Dictionary<TcpClient, List<string>> ();
			var local = IPAddress.Any;
			listener = new TcpListener (local, 23);
			listener.AllowNatTraversal (true);
			try {
				listener.Start ();
			} catch {
				NetworkManager.Instance.Broadcast ("Failed to start telnet client.");
				return;
			}
			Task.Factory.StartNew (Listen);
		}

		/// <summary>
		/// Listens.
		/// </summary>
		void Listen () {
			while (true) {
				var client = listener.AcceptTcpClient ();
				notifications [client] = new List<string> ();
				Clients.Add (client);
				Task.Factory.StartNew (() => HandleClient (client));
			}
		}

		/// <summary>
		/// Handles a client.
		/// </summary>
		/// <param name="client">Client.</param>
		void HandleClient (TcpClient client) {
			SendRaw (client, "Animus Telnet Interface\r\n");
			FlushNotifications (client, global: true);
			SendPrompt (client);
			using (var reader = new StreamReader (client.GetStream ())) {
				while (client.Connected) {
					try {
						var line = reader.ReadLine ();
						FlushNotifications (client, beforeCommand: true);
						ProcessTelnetCommand (client, line);
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

		/// <summary>
		/// Processes telnet-specific commands.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="command">Command.</param>
		void ProcessTelnetCommand (TcpClient client, string command) {
			switch (command) {
			case "quit":
				client.GetStream ().Close ();
				client.Close ();
				Clients.Remove (client);
				break;
			case "clear":
				// <ESC>[2J
				SendRaw (client, "\x1B[2J");
				break;
			}
		}

		/// <summary>
		/// Sends the prompt.
		/// </summary>
		/// <param name="client">Client.</param>
		void SendPrompt (TcpClient client) {
			var dir = Environment.CurrentDirectory;
			dir = new string (dir.Skip (dir.LastIndexOf ('\\')).ToArray ());
			SendRaw (client, "{0}:{1}$ ", Environment.UserName, dir);
		}

		/// <summary>
		/// Flushs the notifications.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="global">If set to <c>true</c>, flush global notifications.</param>
		/// <param name="beforeCommand">If set to <c>true</c>, flush log.</param>
		/// <param name="afterCommand">If set to <c>true</c>, flush command output.</param>
		void FlushNotifications (TcpClient client, bool @global = false, bool beforeCommand = false, bool afterCommand = false) {
			if (@global && globalLog.Count > 0) {
				SendLine (client, "--log--");
				globalLog.ForEach (str => SendLine (client, str));
				SendLine (client, "-------");
			}
			if (notifications [client].Count == 0)
				return;
			if (beforeCommand) {
				SendLine (client, "--log--");
				notifications [client].ForEach (str => SendLine (client, str));
				SendLine (client, "-------");
				notifications [client].Clear ();
			} else if (afterCommand) {
				notifications [client].ForEach (str => SendLine (client, str));
				notifications [client].Clear ();
			}
		}

		/// <summary>
		/// Sends a message to a client.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		void SendRaw (TcpClient client, string format, params object[] args) {
			var data = Encoding.ASCII.GetBytes (string.Format (format, args));
			client.GetStream ().Write (data, 0, data.Length);
			client.GetStream ().Flush ();
		}

		/// <summary>
		/// Sends a message to a client.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
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
			Clients.ForEach (client => notifications[client].Add (string.Format (format, args)));
		}

		#endregion
	}
}

