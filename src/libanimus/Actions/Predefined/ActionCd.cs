﻿using System;
using System.IO;

namespace libanimus
{
	public class ActionCd : HostAction
	{
		public ActionCd () : base ("cd") {
		}

		public override void Run (IUpstream source, params string[] args) {
			if (args.Length == 1 && Directory.Exists (args [0]))
				Environment.CurrentDirectory = args [0];
			else if (args.Length > 1) {
				var path = string.Join (" ", args);
				if (Directory.Exists (path))
					Environment.CurrentDirectory = args [0];
			} else
				NetworkManager.Instance.NotifySource (source, "Invalid path.");
		}
	}
}

