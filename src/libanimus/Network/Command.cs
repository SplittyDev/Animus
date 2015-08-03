using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libanimus
{
	public class Command {
		public string name;
		public string[] args;

		public static Command Parse (string com) {
			var command = new Command ();
			if (com.Contains (" ")) {
				command.name = new string (com.TakeWhile (c => c != ' ').ToArray ());
				var argstr = new string (com.Skip (com.IndexOf (' ') + 1).ToArray ());
				var accum = new StringBuilder ();
				var i = 0;
				var awaitquote = false;
				var lst = new List<string> ();
				while (i < argstr.Length) {
					var _status = awaitquote;
					if (argstr [i] == '"' && awaitquote)
						awaitquote = false;
					else if (argstr [i] == '"' && !awaitquote)
						awaitquote = true;
					var _i = awaitquote != _status ? 1 : 0;
					if (argstr [_i] == ' ' && !awaitquote) {
						lst.Add (accum.ToString ());
						accum.Clear ();
					} else
						accum.Append (argstr [_i]);
					++i;
				}
				command.args = lst.ToArray ();
			} else {
				command.name = com;
				command.args = new string[0];
			}
			return command;
		}
	}
}

