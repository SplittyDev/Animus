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

		bool CheckUpdate ();
		void Update ();

		// Communication
		void Notify (string format, params object[] args);
	}
}

