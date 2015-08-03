using System;
using libanimus;

namespace AnimusCore
{
	class MainClass
	{
		IrcClient client;

		public static void Main (string[] args) {
			new MainClass ().Main ();
		}

		public MainClass () {
			client = new IrcClient ();
		}

		public void Main () {
			client.Connect ("int0x10.com", 6697);
			client.Join ("#OperationMythicDawn");
			while (true) {
			}
		}
	}
}
