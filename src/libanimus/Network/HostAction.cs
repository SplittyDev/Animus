using System;

namespace libanimus
{
	public class HostAction
	{
		public readonly string Name;

		public HostAction (string name) {
			Name = name;
		}

		public virtual void Run (Command com) {
		}
	}
}

