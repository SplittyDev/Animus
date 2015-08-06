using System;
using System.Collections.Generic;
using libanimus.Actions;

namespace libanimus.Networking
{
	/// <summary>
	/// Upstream.
	/// </summary>
	public interface IUpstream
	{
		List<HostAction> Actions { get; }

		// Communication
		void Notify (string format, params object[] args);
	}
}

