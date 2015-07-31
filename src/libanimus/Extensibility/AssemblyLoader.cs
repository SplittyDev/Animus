using System;
using System.Reflection;

namespace libanimus
{
	public class AssemblyLoader : MarshalByRefObject
	{
		public Assembly assembly;

		public override object InitializeLifetimeService () {
			return null;
		}

		public void LoadAssembly (string path) {
			assembly = Assembly.Load (AssemblyName.GetAssemblyName (path));
		}

		public void LoadAssembly (byte[] data) {
			assembly = Assembly.Load (data);
		}
	}
}

