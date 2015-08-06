using System;
using System.Management;

namespace libanimus.Internal
{
	class VMInfo
	{
		public const string ANY = "*";
		public string Name;
		public string Manufacturer;
		public string Model;

		public VMInfo (string name, string manufacturer = ANY, string model = ANY) {
			Name = name;
			Manufacturer = manufacturer;
			Model = model;
		}

		public bool IsMatch (ManagementObject obj) {
			bool matches_manufacturer = Manufacturer == ANY
				|| obj ["Manufacturer"].ToString ().ToLowerInvariant ().Contains (Manufacturer);
			bool matches_model = Model == ANY
				|| obj ["Model"].ToString ().ToLowerInvariant ().Contains (Model);
			return (Manufacturer != ANY || Model != ANY) && (matches_manufacturer && matches_model);
		}
	}
}

