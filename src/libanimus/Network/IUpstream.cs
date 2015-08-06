using System;

namespace libanimus
{
	/// <summary>
	/// Upstream.
	/// </summary>
	public interface IUpstream
	{
		bool CheckUpdate ();
		void Update ();

		// Communication
		void Notify (string format, params object[] args);
	}
}

