using System;
using Gtk;

namespace IrcTest
{
	public class UserModel : ListStore
	{
		public UserModel () : base (typeof (string)) {
		}
	}
}

