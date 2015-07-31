using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;

namespace libanimus
{
	public class AppDomainProxy : IDisposable
	{
		readonly AppDomain domain;

		public static AppDomainProxy Create () {
			return new AppDomainProxy ();
		}

		AppDomainProxy () {
			var evidence = new Evidence ();
			evidence.AddHostEvidence (new Zone (SecurityZone.MyComputer));
			var setup = new AppDomainSetup ();
			domain = AppDomain.CreateDomain (Guid.NewGuid ().ToString ("N"), evidence, setup);
			domain.AssemblyResolve += ResolveAssembly;
		}

		static Assembly ResolveAssembly (object sender, ResolveEventArgs args) {
			var assembly = AppDomain.CurrentDomain.GetAssemblies ()
				.FirstOrDefault (asm => asm.FullName == args.Name);
			return assembly == default (Assembly) ? null : assembly;
		}

		public AssemblyLoader LoadAssembly (string path) {
			var type = typeof(AssemblyLoader);
			var fullName = type.FullName;
			var assemblyFullName = type.Assembly.FullName;
			var instance = domain.CreateInstanceAndUnwrap (assemblyFullName, fullName);
			var loader = (AssemblyLoader)instance;
			loader.LoadAssembly (path);
			return loader;
		}

		public AssemblyLoader LoadAssembly (byte[] raw) {
			var type = typeof(AssemblyLoader);
			var fullName = type.FullName;
			var assemblyFullName = type.Assembly.FullName;
			var instance = domain.CreateInstanceAndUnwrap (assemblyFullName, fullName);
			var loader = (AssemblyLoader)instance;
			loader.LoadAssembly (raw);
			return loader;
		}

		public void Unload () {
			AppDomain.Unload (domain);
		}

		#region IDisposable implementation

		public void Dispose () {
			Unload ();
		}

		#endregion
	}
}

