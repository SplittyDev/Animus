using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;

namespace libanimus
{
	public static class Antivirus
	{
		public static string[] List () {
			try {
				return Environment.OSVersion.Version.Major > 5 ? ListVistaPlus () : ListXP ();
			}
			catch (Exception) {
				// Add error logging here
				return new string[0];
			}
		}

		static string[] ListXP () {
			const string entry = "rootSecurityCenter";
			return ListGeneric (entry);
		}

		static string[] ListVistaPlus () {
			const string entry = "rootSecurityCenter2";
			return ListGeneric (entry);
		}

		static string[] ListGeneric (string entryPoint) {
			const string query = "SELECT * FROM AntivirusProduct";
			var str = string.Format ("\\{0}{1}", Environment.MachineName, entryPoint);
			return new ManagementObjectSearcher (str, query)
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

