using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;

namespace libanimus
{
	/// <summary>
	/// App domain proxy.
	/// </summary>
	public class AppDomainProxy : IDisposable
	{
		/// <summary>
		/// The app domain that's being proxied.
		/// </summary>
		readonly AppDomain domain;

		/// <summary>
		/// Creates a new proxied app domain.
		/// </summary>
		public static AppDomainProxy Create () {
			return new AppDomainProxy ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.AppDomainProxy"/> class.
		/// </summary>
		AppDomainProxy () {
			var evidence = new Evidence ();
			evidence.AddHostEvidence (new Zone (SecurityZone.MyComputer));
			var setup = new AppDomainSetup ();
			domain = AppDomain.CreateDomain (Guid.NewGuid ().ToString ("N"), evidence, setup);
			domain.AssemblyResolve += ResolveAssembly;
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
		/// Loads an assembly.
		/// </summary>
		/// <returns>The assembly.</returns>
		/// <param name="path">Path.</param>
		public AssemblyLoader LoadAssembly (string path) {
			var type = typeof(AssemblyLoader);
			var fullName = type.FullName;
			var assemblyFullName = type.Assembly.FullName;
			var instance = domain.CreateInstanceAndUnwrap (assemblyFullName, fullName);
			var loader = (AssemblyLoader)instance;
			loader.LoadAssembly (path);
			return loader;
		}

		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <returns>The assembly.</returns>
		/// <param name="raw">Raw.</param>
		public AssemblyLoader LoadAssembly (byte[] raw) {
			var type = typeof(AssemblyLoader);
			var fullName = type.FullName;
			var assemblyFullName = type.Assembly.FullName;
			var instance = domain.CreateInstanceAndUnwrap (assemblyFullName, fullName);
			var loader = (AssemblyLoader)instance;
			loader.LoadAssembly (raw);
			return loader;
		}

		/// <summary>
		/// Unloads the app domain.
		/// </summary>
		public void Unload () {
			AppDomain.Unload (domain);
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="libanimus.AppDomainProxy"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="libanimus.AppDomainProxy"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="libanimus.AppDomainProxy"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="libanimus.AppDomainProxy"/> so the
		/// garbage collector can reclaim the memory that the <see cref="libanimus.AppDomainProxy"/> was occupying.</remarks>
		public void Dispose () {
			Unload ();
		}

		#endregion
	}
}

