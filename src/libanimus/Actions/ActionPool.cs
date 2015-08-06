using System;
using System.Collections.Generic;
using System.Linq;

namespace libanimus
{
	public class ActionPool
	{
		static readonly object syncRoot = new object ();
		static ActionPool instance;

		public static ActionPool Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new ActionPool ();
				return instance;
			}
		}

		readonly List<HostAction> actions;

		public ActionPool () {
			actions = new List<HostAction> ();
		}

		public void ProcessCommand (Command com) {
			var acts = from act in actions
			           where act.Name.ToLowerInvariant () == com.Name.ToLowerInvariant ()
			           select act;
			foreach (var act in acts)
				act.Run (com.Args);
		}

		public void ProcessCommand (string command) {
			var com = Command.Parse (command);
			ProcessCommand (com);
		}

		public void RegisterAction (HostAction action) {
			actions.Add (action);
		}

		public void RegisterActions (IEnumerable<HostAction> collection) {
			actions.AddRange (collection);
		}
	}
}

