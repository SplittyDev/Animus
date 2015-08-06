using System;

namespace libanimus.Extensibility
{
	/// <summary>
	/// Module wrapper.
	/// </summary>
	public class ModuleWrapper
	{
		/// <summary>
		/// The proxy.
		/// </summary>
		public readonly AppDomainProxy Proxy;

		/// <summary>
		/// The module.
		/// </summary>
		public readonly IModule Module;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleWrapper"/> class.
		/// </summary>
		/// <param name="proxy">Proxy.</param>
		/// <param name="module">Module.</param>
		public ModuleWrapper (AppDomainProxy proxy, IModule module) {
			Proxy = proxy;
			Module = module;
		}
	}
}

