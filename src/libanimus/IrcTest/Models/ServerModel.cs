using System;
using Gtk;

namespace IrcTest
{
	public class ServerModel : TreeStore
	{
		public ServerModel () : base (typeof (string)) {
			
		}
	}
}

