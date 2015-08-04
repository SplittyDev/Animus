using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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

			if (client.Connected) {
				Stream = client.GetStream ();

				if (ssl) {
					Stream = new SslStream (client.GetStream (), false, validationCallback);
					try {
						((SslStream)Stream).AuthenticateAsClient (Server);
					} catch (AuthenticationException e) {
						Console.WriteLine ("Exception: {0}", e.Message);
						if (e.InnerException != null) {
							Console.WriteLine ("Inner exception: {0}", e.InnerException.Message);
						}
						Console.WriteLine ("Authentication failed - closing the connection.");
						client.Close ();
						return;
					}
				}
			} else {
				var exceptionText = string.Format ("Could not connect to {0}:{1}", Server, Port);
				throw new ConnectionFailedException (exceptionText);
			}

			IsConnected = true;

			Reader = new StreamReader (Stream);
			Writer = new StreamWriter (Stream);

			Task.Factory.StartNew (() => {
				string line;
				while (true) {
					while ((line = Reader.ReadLine ()) != null) {
						Console.WriteLine (line);

						var commandParts = line.Split (' ');
						if (commandParts [0].Substring (0, 1) == ":") {
							commandParts [0] = commandParts [0].Remove (0, 1);
						}

						if (commandParts [0].Contains (Server))
							switch (commandParts [1]) {
						case "001":
							HasJoined = true;
							break;
							}

						if (commandParts[0] == "PING") {
							SendRaw ("PONG {0}", commandParts [1]);
						}
					}
				}
			});
		}

		#endregion
	}
}

