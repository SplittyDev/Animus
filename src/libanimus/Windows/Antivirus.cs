using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;

namespace libanimus
{
	public static class Antivirus
	{
		public const string ENTRY_WIN_NT5 = "rootSecurityCenter";
		public const string ENTRY_WIN_NT6 = "rootSecurityCenter2";

		public static string[] ListAntivirus () {
			return ListProduct ("AntivirusProduct");
		}

		public static string[] ListAntispyware () {
			return ListProduct ("AntispywareProduct");
		}

		public static string[] ListFirewalls () {
			return ListProduct ("FirewallProduct");
		}

		static string[] ListProduct (string product) {
			try {
				return Environment.OSVersion.Version.Major > 5 ?
					ListGeneric (ENTRY_WIN_NT6, product) :
					ListGeneric (ENTRY_WIN_NT5, product);
			}
			catch (Exception) {
				// Add error logging here
				return new string[0];
			}
		}

		static string[] ListGeneric (string entryPoint, string queryProduct) {
			var query = string.Format ("SELECT * FROM {0}", queryProduct);
			var entry = string.Format ("\\root\\{0}", entryPoint);
			return new ManagementObjectSearcher (entry, query)
				.Get ()
				.GetManagementObjects ()
				.Select (obj => obj["displayName"].ToString ())
				.ToArray ();
		}

		static ICollection<ManagementObject> GetManagementObjects (this ManagementObjectCollection moc) {
			var lst = new List<ManagementObject> ();
			foreach (var obj in moc)
				lst.Add (obj as ManagementObject);
			return lst;
		}
	}
}

