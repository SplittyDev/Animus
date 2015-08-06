using System;
using System.IO;
using System.Linq;
using System.Text;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionLs : HostAction
	{
		public ActionLs () : base ("ls") {
		}

		public override void Run (IUpstream source, params string[] args) {
			var dir = Directory.GetCurrentDirectory ();
			if (args.Length == 1) {
				try {
					if (!Directory.Exists (Path.GetFullPath (args.First ()))) {
						NetworkManager.Instance.Notify (source, "Invalid path.");
						return;
					}
				} catch (Exception ex) {
					NetworkManager.Instance.Notify (source, ex.Message);
					return;
				}
				dir = Path.GetFullPath (args.First ());
			}
			var info = new DirectoryInfo (dir);
			var accum = new StringBuilder ();
			info.GetDirectories ().ToList ().ForEach (p => accum.AppendFormat (" {0,-10} |", p.Name));
			info.GetFiles ().ToList ().ForEach (p => accum.AppendFormat (" {0,-10} |", p.Name));
			NetworkManager.Instance.Notify (source, accum.ToString ().Trim (new [] { ' ', '|' }));
		}
	}
}

