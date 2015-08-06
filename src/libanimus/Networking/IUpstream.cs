using System;
using System.Collections.Generic;

namespace libanimus
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

