using System;

namespace libanimus
{
	/// <summary>
	/// Host action.
	/// </summary>
	public class HostAction
	{
		/// <summary>
		/// The name.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.HostAction"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public HostAction (string name) {
			Name = name;
		}

		/// <summary>
		/// Runs the specified command.
		/// </summary>
		/// <param name="args"></param>
		public virtual void Run (params string[] args) {
		}
	}
}

