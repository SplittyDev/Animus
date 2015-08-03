using System;
using libanimus;

namespace AnimusCore
{
	class MainClass
	{
		SplittyIrcClient client;

		public static void Main (string[] args) {
			new MainClass ().Main ();
		}

		public MainClass () {
			client = new SplittyIrcClient ();
		}

		public void Main () {
			client.Connect ("int0x10.com", 6697);
			client.Join ("#OperationMythicDawn");
			while (true) {
			}
		}
	}
}
