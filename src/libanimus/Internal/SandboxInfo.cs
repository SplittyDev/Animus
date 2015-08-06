using System;
using Microsoft.Win32;

namespace libanimus.Internal
{
	public class SandboxInfo
	{
		public string Name;

		readonly string Module;
		readonly string RegKey;
		readonly string RegVal;
		readonly string RegEq;

		readonly bool useModule;
		readonly bool useRegistry;

		public SandboxInfo (string name, string module = null, string regKey = null, string regValue = null, string regEquals = null) {
			Name = name;
			Module = module;
			RegKey = regKey;
			RegVal = regValue;
			RegEq = regEquals;
			useModule = !string.IsNullOrEmpty (module);
			useRegistry = !string.IsNullOrEmpty (regKey)
				&& !string.IsNullOrEmpty (regValue)
				&& !string.IsNullOrEmpty (regEquals);
		}

		public bool IsMatch () {
			if (useModule)
				return Native.Native.GetModuleHandle (Module).ToInt32 () != 0;
			if (useRegistry) {
				try {
					bool ret;
					using (var key = Registry.LocalMachine.OpenSubKey (RegKey))
						ret = key.GetValue (RegVal).ToString () == RegEq;
					return ret;
				} catch {
					// Log exception here
					return false;
				}
			}
			return false;
		}
	}
}

