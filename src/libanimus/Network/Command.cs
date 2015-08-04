using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libanimus
{
	/// <summary>
	/// Command.
	/// </summary>
	public class Command {
		
		/// <summary>
		/// The name.
		/// </summary>
		public string Name;

		/// <summary>
		/// The arguments.
		/// </summary>
		public string[] Args;

		/// <summary>
		/// Parses the specified command.
		/// </summary>
		/// <param name="com">The command.</param>
		public static Command Parse (string com) {
			var command = new Command ();
			if (com.Contains (" ")) {
				command.Name = new string (com.TakeWhile (c => c != ' ').ToArray ());
				var argstr = new string (com.Skip (com.IndexOf (' ') + 1).ToArray ());
				var accum = new StringBuilder ();
				var i = 0;
				var awaitquote = false;
				var lst = new List<string> ();
				while (i < argstr.Length) {
					if (argstr [i] == ' ' && !awaitquote) {
						lst.Add (accum.ToString ());
						accum.Clear ();
					} else {
						accum.Append (argstr [i]);
					}
					if (argstr [i] == '"' && awaitquote)
						awaitquote = false;
					else if (argstr [i] == '"' && !awaitquote)
						awaitquote = true;
					++i;
				}
				lst.Add (accum.ToString ());
				command.Args = lst.ToArray ();
			} else {
				command.Name = com;
				command.Args = new string[0];
			}
			return command;
		}
	}
}

