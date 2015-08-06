using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libanimus.Networking
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
		/// The source.
		/// </summary>
		public IUpstream Source;

		/// <summary>
		/// Parses the specified command.
		/// </summary>
		/// <param name="com">The command.</param>
		/// <param name="source">The source.</param> 
		public static Command Parse (string com, IUpstream source = null) {
			var command = new Command ();
			command.Source = source;
			com = com.Trim ();
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

		public static explicit operator string (Command com) {
			var accum = new StringBuilder ();
			foreach (var arg in com.Args)
				accum.AppendFormat ("\"{0}\" ", arg);
			return accum.ToString ().Trim ();
		}
	}
}

