using System;

namespace libanimus
{
	/// <summary>
	/// Module wrapper.
	/// </summary>
	public class ModuleWrapper
	{
		public readonly AppDomainProxy Proxy;
		public readonly IModule Module;

		public ModuleWrapper (AppDomainProxy proxy, IModule module) {
			Proxy = proxy;
			Module = module;
		}
	}
}

