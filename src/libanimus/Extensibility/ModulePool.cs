using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace libanimus.Extensibility
{
	/// <summary>
	/// Module pool.
	/// </summary>
	public class ModulePool
	{
		/// <summary>
		/// The loaded modules.
		/// </summary>
		public readonly List<ModuleWrapper> modules;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModulePool"/> class.
		/// </summary>
		public ModulePool () {
			modules = new List<ModuleWrapper> ();
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
		}

		/// <summary>
		/// Resolves an assembly.
		/// </summary>
		/// <returns>The assembly.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		static Assembly ResolveAssembly (object sender, ResolveEventArgs args) {
			var assembly = AppDomain.CurrentDomain.GetAssemblies ()
				.FirstOrDefault (asm => asm.FullName == args.Name);
			return assembly == default (Assembly) ? null : assembly;
		}

		/// <summary>
		/// Loads a module.
		/// </summary>
		/// <returns>The module.</returns>
		/// <param name="path">Path.</param>
		public IModule LoadModule (string path) {
			var proxy = AppDomainProxy.Create ();
			var loader = proxy.LoadAssembly (path);
			var assembly = loader.assembly;
			var types = assembly.GetExportedTypes ();
			foreach (var type in types) {
				if (type.IsClass && type.GetInterface (typeof (IModule).FullName) != null) {
					var module = Activator.CreateInstance (type) as IModule;
					modules.Add (new ModuleWrapper (proxy, module));
					return module;
				}
			}
			return null;
		}
	}
}

