using System;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Security.Authentication;
using System.Threading;
using System.Text;

namespace libanimus {
	public class IrcClient {
		public string Server { get; private set; }
		public int Port { get; private set; }

		public bool IsConnected { get; private set; }
		public bool IsJoined { get; private set; }

		public Stream Stream { get; private set; }
		public StreamReader Reader { get; private set; }
		public StreamWriter Writer { get; private set; }

		public string Nickname { get; private set; }

		public IrcClient () {
			this.IsConnected = false;
			this.IsJoined = false;
		}

		public void Connect (string server, int port, IrcClientConnectionOptions options) {
			this.Server = server;
			this.Port = port;
			TcpClient client = new TcpClient (this.Server, this.Port);

			if (client.Connected) {
				Stream = client.GetStream ();

				if (options.Ssl) {
					var sslCallback = options.SslCertificateValidationCallback;
					Stream = new SslStream (client.GetStream (), false, sslCallback);
					try {
						((SslStream)Stream).AuthenticateAsClient (this.Server);
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
				var exceptionText = string.Format ("Could not connect to {0}:{1}", this.Server, this.Port);
				throw new ConnectionFailedException (exceptionText);
			}

			this.IsConnected = true;

			this.Reader = new StreamReader (this.Stream);
			this.Writer = new StreamWriter (this.Stream);

			Thread thread = new Thread (new ThreadStart(delegate() {
				while (true) {
					string line = "";
					while ((line = this.Reader.ReadLine()) != null) {
						Console.WriteLine (line);

						string[] commandParts = new string[line.Split(' ').Length];
						commandParts = line.Split(' ');
						if (commandParts[0].Substring(0, 1) == ":") {
							commandParts[0] = commandParts[0].Remove(0, 1);
						}

						if (commandParts[0].Contains(this.Server))
							switch (commandParts[1]) {
						case "001":
							this.IsJoined = true;
							break;
							}

						if (commandParts[0] == "PING") {
							this.Write("PONG {0}", commandParts[1]);
						}
					}
				}
			}));
			thread.Start ();
		}

		public void LogIn (string username, string realname, string nickname) {
			this.Write ("USER {0} 0 * :{1}", username, realname);
			this.Write ("NICK {0}", nickname);

			this.Nickname = nickname;
		}

		public void Join (string channel) {
			this.Write ("JOIN {0}", channel);
		}

		public void Mode (string modes) {
			this.Write ("MODE {0} {1}", this.Nickname, modes);
		}

		public void Write(string format, params object[] args) {
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat (format, args);
			sb.Append ("\r\n");
			this.Writer.Write (sb.ToString ());
			this.Writer.Flush ();
		}
	}
}

