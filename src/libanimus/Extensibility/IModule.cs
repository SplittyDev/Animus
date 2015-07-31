using System;

namespace libanimus
{
	/// <summary>
	/// The Animus module interface.
	/// </summary>
	public interface IModule
	{
		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		/// <value>The name of the module</value>
		string Name { get; }

		/// <summary>
		/// Invoked on initialization,
		/// guaranteed to happen before any
		/// modules are loaded.
		/// </summary>
		void Init ();

		/// <summary>
		/// Invoked when the module gets loaded,
		/// guaranteed to happen after the initialization
		/// of all modules.
		/// </summary>
		void Load ();

		/// <summary>
		/// Invoked on shutdown of the application.
		/// </summary>
		void Unload ();

		/// <summary>
		/// Process the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		void Process (byte[] data);
	}
}

