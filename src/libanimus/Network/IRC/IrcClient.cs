using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace libanimus {
	public class IrcClient {

		#region Public Properties

		/// <summary>
		/// Gets the server.
		/// </summary>
		/// <value>The server.</value>
		public string Server { get; private set; }

		/// <summary>
		/// Gets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is connected.
		/// </summary>
		/// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
		public bool IsConnected { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance has joined.
		/// </summary>
		/// <value><c>true</c> if this instance has joined; otherwise, <c>false</c>.</value>
		public bool HasJoined { get; private set; }

		/// <summary>
		/// Gets the stream.
		/// </summary>
		/// <value>The stream.</value>
		public Stream Stream { get; private set; }

		/// <summary>
		/// Gets the reader.
		/// </summary>
		/// <value>The reader.</value>
		public StreamReader Reader { get; private set; }

		/// <summary>
		/// Gets the writer.
		/// </summary>
		/// <value>The writer.</value>
		public StreamWriter Writer { get; private set; }

		/// <summary>
		/// Gets the nickname.
		/// </summary>
		/// <value>The nickname.</value>
		public string Nickname { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Message handler.
		/// </summary>
		public delegate void MessageHandler (string message, string sender);

		/// <summary>
		/// Occurs when a channel message has been received.
		/// </summary>
		public event MessageHandler OnChannelMessage;

		/// <summary>
		/// Occurs when a private message has been received.
		/// </summary>
		public event MessageHandler OnPrivateMessage;

		#endregion

		#region Private Fields

		/// <summary>
		/// The actions.
		/// </summary>
		readonly ICollection<HostAction> actions;

		/// <summary>
		/// The GUID.
		/// </summary>
		readonly Guid guid;

		/// <summary>
		/// The identifier.
		/// </summary>
		readonly string id;

		/// <summary>
		/// The validation callback.
		/// </summary>
		RemoteCertificateValidationCallback validationCallback;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.IrcClient"/> class.
		/// </summary>
		public IrcClient () {
			actions = new List<HostAction> ();
			guid = Guid.NewGuid ();
			id = string.Format ("animus{0}", new string (guid.ToString ("N").Take (16).ToArray ()));
			IsConnected = false;
			HasJoined = false;
			OnChannelMessage += (message, sender) => { };
			OnPrivateMessage += (message, sender) => { };
		}

		/// <summary>
		/// Connect to specified server:port using the specified options.
		/// </summary>
		/// <param name="server">Server.</param>
		/// <param name="port">Port.</param>
		/// <param name="ssl">Whether the connection should use SSL..</param>
		public void Connect (string server, int port, bool ssl, RemoteCertificateValidationCallback callback = null) {
			validationCallback = callback;
			if (validationCallback == null)
				validationCallback = new RemoteCertificateValidationCallback ((sender, certificate, chain, sslPolicyErrors) => true);
			_Connect (server, port, ssl);
			while (!IsConnected) {}
			USER (id);
			NICK (id);
			while (!HasJoined) {}
			MODE (Nickname, "+B");
		}

		/// <summary>
		/// Registers an action.
		/// </summary>
		/// <param name="action">Action.</param>
		public void RegisterAction (HostAction action) {
			actions.Add (action);
		}

		/// <summary>
		/// Sends a raw command.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void SendRaw (string format, params object[] args) {
			var sb = new StringBuilder ();
			sb.AppendFormat (format, args);
			sb.Append ("\r\n");
			Writer.Write (sb);
			Writer.Flush ();
		}

		#region Raw IRC commands

		public void PONG (string server1, string server2 = null) {
			if (string.IsNullOrEmpty (server2))
				SendRaw ("PONG {0}", server1);
			else
				SendRaw ("PONG {0} {1}", server1, server2);
		}

		public void USER (string username, string realname = null) {
			if (string.IsNullOrEmpty (realname))
				realname = username;
			SendRaw ("USER {0} 0 * :{1}", username, realname);
		}

		public void NICK (string nickname) {
			SendRaw ("NICK {0}", nickname);
			Nickname = nickname;
		}

		public void MODE (string nickname, string modes) {
			SendRaw ("MODE {0} {1}", nickname, modes);
		}

		public void JOIN (string channel) {
			SendRaw ("JOIN {0}", channel);
		}

		public void PRIVMSG (string target, string message) {
			SendRaw ("PRIVMSG {0} {1}", target, message);
		}

		#endregion

		#region User-friendly wrappers

		public void LogIn (string username, string realname, string nickname) {
			USER (username, realname);
			NICK (nickname);
		}

		public void Mode (string mode) {
			MODE (Nickname, mode);
		}

		public void Join (string channel) {
			JOIN (channel);
		}

		public void Privmsg (string msg, string target) {
			PRIVMSG (target, msg);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Connect to specified server:port using the specified options.
		/// </summary>
		/// <param name="server">Server.</param>
		/// <param name="port">Port.</param>
		/// <param name="ssl">Whether the connection should use SSL.</param>
		void _Connect (string server, int port, bool ssl) {
			
			Server = server;
			Port = port;

			var client = new TcpClient (Server, Port);
			Stream = client.GetStream ();

			if (!client.Connected) {
				var exceptionText = string.Format ("Could not connect to {0}:{1}", Server, Port);
				throw new ConnectionFailedException (exceptionText);
			}

			if (ssl) {
				Stream = new SslStream (Stream, false, validationCallback);
				try {
					((SslStream)Stream).AuthenticateAsClient (Server);
				} catch (AuthenticationException e) {
					Console.Error.WriteLine ("Authentication failed - closing the connection");
					Console.Error.WriteLine ("Reason: {0}", e.Message);
					client.Close ();
					return;
				}
			}

			IsConnected = true;

			Reader = new StreamReader (Stream);
			Writer = new StreamWriter (Stream);

			Task.Factory.StartNew (_Listen);
		}

		/// <summary>
		/// Listens for incoming commands.
		/// </summary>
		void _Listen () {
			string line;
			while (true) {
				while ((line = Reader.ReadLine ()) != null) {
					var commandParts = line.TrimStart (':').Split (' ');

					string ident = string.Empty;
					if (commandParts.First ().Contains (Server)) {
						switch (commandParts [1]) {
						case "001":
						case "002":
						case "003":
						case "004":
							HasJoined = true;
							break;
						}
						if (Regex.IsMatch (commandParts.Skip (1).First (), @"^\d+$"))
							continue;
					} else if (commandParts.First ().Contains ("@")) {
						ident = commandParts.First ();
						commandParts = commandParts.Skip (1).ToArray ();
					}

					var com = Command.Parse (string.Join (" ", commandParts));
					//Console.WriteLine ("Command: '{0}'", com.name);
					switch (com.name) {
					case "PING":
						PONG (com.args [0], com.args.Length > 1 ? com.args [1] : null);
						break;
					case "PRIVMSG":
						if (com.args.Length > 1) {
							if (com.args [0].StartsWith ("#"))
								OnChannelMessage (string.Join (" ", com.args.Skip (1)).TrimStart (':'), com.args [0]);
							else
								OnPrivateMessage (string.Join (" ", com.args.Skip (1)).TrimStart (':'),
									new string (ident.TakeWhile (c => c != '!').ToArray ()));
						}
						break;
					default:
						//Console.WriteLine ("Unknown command: {0}", com.name);
						break;
					}
				}
			}
		}

		#endregion
	}
}

