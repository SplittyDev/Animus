using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace libanimus.Networking {
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

		/// <summary>
		/// Gets the channels.
		/// </summary>
		/// <value>The channels.</value>
		public List<string> Channels { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Message handler.
		/// </summary>
		public delegate void MessageHandler (string message, string sender);

		/// <summary>
		/// Connection handler.
		/// </summary>
		public delegate void ConnectionHandler (string hostname, int port);

		/// <summary>
		/// Name handler.
		/// </summary>
		public delegate void NameHandler (string[] names);

		/// <summary>
		/// Joined handler.
		/// </summary>
		public delegate void JoinedHandler (string channel);

		public event NameHandler NamesObtained;

		public event JoinedHandler ChannelJoined;

		public event EventHandler LoggedIn;

		/// <summary>
		/// Occurs when a channel message has been received.
		/// </summary>
		public event MessageHandler ChannelMessage;

		/// <summary>
		/// Occurs when a private message has been received.
		/// </summary>
		public event MessageHandler PrivateMessage;

		/// <summary>
		/// Occurs when the clients is connected to the host.
		/// </summary>
		public event ConnectionHandler Connected;

		/// <summary>
		/// Occurs when the clients gets disconnected from the irc.
		/// </summary>
		public event EventHandler Disconnected;

		#endregion

		#region Public Fields

		/// <summary>
		/// The identifier.
		/// </summary>
		public readonly string Identifier;

		#endregion

		#region Private Fields

		/// <summary>
		/// The GUID.
		/// </summary>
		readonly Guid guid;

		/// <summary>
		/// The validation callback.
		/// </summary>
		RemoteCertificateValidationCallback validationCallback;

		/// <summary>
		/// A flag indicating whether a connection error occurred.
		/// </summary>
		bool connection_error;

		/// <summary>
		/// The last connection error.
		/// </summary>
		string connection_lasterror;

		/// <summary>
		/// The client.
		/// </summary>
		TcpClient client;

		/// <summary>
		/// The tmp names.
		/// </summary>
		List<string> tmp_names;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="IrcClient"/> class.
		/// </summary>
		public IrcClient () {
			guid = Guid.NewGuid ();
			Identifier = string.Format ("animus{0}", new string (guid.ToString ("N").Take (16).ToArray ()));
			tmp_names = new List<string> ();
			Channels = new List<string> ();
			LoggedIn += (sender, e) => { };
			NamesObtained += names => { };
			ChannelJoined += channel => { };
			ChannelMessage += (message, sender) => { };
			PrivateMessage += (message, sender) => { };
			Connected += (hostname, port) => { };
			Disconnected += (sender, e) => { };
		}

		/// <summary>
		/// Connect to specified server:port using the specified options.
		/// </summary>
		/// <param name="server">Server.</param>
		/// <param name="port">Port.</param>
		/// <param name="ssl">Whether the connection should use SSL..</param>
		/// <param name="callback">The callback that checks the SSL certificate for validity</param> 
		public void Connect (string server, int port, bool ssl, RemoteCertificateValidationCallback callback = null) {
			IsConnected = false;
			HasJoined = false;
			connection_error = false;
			validationCallback = callback;

			if (validationCallback == null)
				validationCallback = new RemoteCertificateValidationCallback
					((sender, certificate, chain, sslPolicyErrors) => true);
			
			_Connect (server, port, ssl);
			if (connection_error) {
				Console.WriteLine ("Error while trying to connect.");
				Console.WriteLine ("Call GetLastError to get the exception message.");
				return;
			}

			while (!IsConnected) {
			}

			Task.Factory.StartNew (() => {
				try {
					Connected (server, port);
					_Listen ();
				} catch {
					try {
						Stream.Close ();
						Stream.Dispose ();
					} catch {
						// Log exception here
					}
				}
			}).ContinueWith (task => {
				Console.WriteLine ("Disconnected from IRC.");
				if (task.IsFaulted)
					Console.WriteLine ("IRCClient listener task faulted.");
				Disconnected (this, EventArgs.Empty);
			});
		}

		/// <summary>
		/// Gets the last error.
		/// </summary>
		/// <returns>The last error.</returns>
		public string GetLastError () {
			if (connection_error) {
				connection_error = false;
				return connection_lasterror;
			}
			return string.Empty;
		}

		/// <summary>
		/// Sends a raw command.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="args">Arguments.</param>
		public void SendRaw (string format, params object[] args) {
			if (!IsConnected)
				return;
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

		public void PART (string channel) {
			SendRaw ("PART {0}", channel);
		}

		public void PRIVMSG (string target, string message) {
			SendRaw ("PRIVMSG {0} {1}", target, message);
		}

		public void NAMES (string channel) {
			SendRaw ("NAMES {0}", channel);	
		}

		#endregion

		#region User-friendly wrappers

		public void LogIn (string username, string realname, string nickname) {
			USER (username, realname);
			NICK (nickname);

			while (!HasJoined) {
			}

			LoggedIn (this, EventArgs.Empty);
		}

		public void Mode (string mode) {
			MODE (Nickname, mode);
		}

		public void Join (string channel) {
			JOIN (channel);
		}

		public void Message (string msg, string target) {
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

			client = new TcpClient (Server, Port);
			Stream = client.GetStream ();

			if (!client.Connected) {
				var exceptionText = string.Format ("Could not connect to {0}:{1}", Server, Port);
				connection_error = true;
				connection_lasterror = exceptionText;
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
		}

		/// <summary>
		/// Listens for incoming commands.
		/// </summary>
		void _Listen () {
			string line;
			while (client.Connected && !Reader.EndOfStream) {
				line = null;
				var task = Task.Factory.StartNew (() => line = Reader.ReadLine ());
				task.Wait (10000);
				if (line != null) {
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
						// RPL_TOPIC
						case "332":
							var chan = commandParts.Skip (3).First ();
							if (!Channels.Contains (chan)) {
								Channels.Add (chan);
								ChannelJoined (chan);
							}
							break;
						// RPL_NAMEREPLY
						case "353":
							var users = commandParts.Skip (5);
							foreach (var user in users)
								tmp_names.Add (user.Trim (' ', ':'));
							break;
						// RPL_ENDOFNAMES
						case "366":
							NamesObtained (tmp_names.ToArray ());
							tmp_names.Clear ();
							break;
						}

						// Ignore all other numeric messages
						if (Regex.IsMatch (commandParts.Skip (1).First (), @"^\d+$"))
							continue;
					}
					else if (commandParts.First ().Contains ("@")) {
						
						ident = commandParts.First ();
						commandParts = commandParts.Skip (1).ToArray ();
					}

					var com = Command.Parse (string.Join (" ", commandParts));
					switch (com.Name) {

					// PING
					case "PING":
						PONG (com.Args [0], com.Args.Length > 1 ? com.Args [1] : null);
						break;

					// PRIVMSG
					case "PRIVMSG":
						var sender_nick = new string (ident.TakeWhile (c => c != '!').ToArray ());
						var msg = string.Join (" ", com.Args.Skip (1)).TrimStart (':');
						if (com.Args.Length >= 2) {
							if (com.Args [0].StartsWith ("#"))
								ChannelMessage (msg, sender_nick);
							else
								PrivateMessage (msg, sender_nick);
						}
						break;
					}
				}
			}
		}

		#endregion
	}
}

