using System;
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
			client.PRIVMSG ("#OperationMythicDawn", "Hello from Animus!");
			while (true) {
			}
		}
	}
}