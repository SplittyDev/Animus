using System;
using System.Reflection;

namespace libanimus.Extensibility
{
	/// <summary>
	/// Assembly loader.
	/// </summary>
	public class AssemblyLoader : MarshalByRefObject
	{
		/// <summary>
		/// The assembly.
		/// </summary>
		public Assembly assembly;

		/// <summary>
		/// Initializes the lifetime service.
		/// </summary>
		/// <returns>The lifetime service.</returns>
		public override object InitializeLifetimeService () {
			return null;
		}

		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <param name="path">Path.</param>
		public void LoadAssembly (string path) {
			assembly = Assembly.Load (AssemblyName.GetAssemblyName (path));
		}

		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <param name="data">Data.</param>
		public void LoadAssembly (byte[] data) {
			assembly = Assembly.Load (data);
		}
	}
}

