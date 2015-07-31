using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace libanimus
{
	public class ModulePool
	{
		public readonly List<ModuleWrapper> modules;

		public ModulePool ()
		{
			modules = new List<ModuleWrapper> ();
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
		}

		Assembly ResolveAssembly (object sender, ResolveEventArgs args) {
			var assembly = AppDomain.CurrentDomain.GetAssemblies ()
				.FirstOrDefault (asm => asm.FullName == args.Name);
			return assembly == default (Assembly) ? null : assembly;
		}

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

