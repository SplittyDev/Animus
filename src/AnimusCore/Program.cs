using System;
using System.Threading;
using libanimus;

namespace AnimusCore
{
	class MainClass
	{
		readonly IrcClient client;

		public static void Main (string[] args) {
			new MainClass ().Main ();
		}

		public MainClass () {
			client = new IrcClient ();
		}

		public void Main () {
			client.Connect ("int0x10.com", 6697, true);
			client.Join ("#OperationMythicDawn");
			client.OnChannelMessage += (message, sender) =>
				Console.WriteLine ("[{0}] {1}", sender, message);
			client.OnPrivateMessage += (message, sender) =>
				Console.WriteLine ("[{0}] {1}", sender, message);
			Idle ();
		}

		static void Idle () {
			Thread.Sleep (Timeout.InfiniteTimeSpan);
		}
	}
}