using System;
using System.Collections.Generic;
using System.Linq;

namespace libanimus
{
	public class NetworkManager
	{
		static readonly object syncRoot = new object ();
		static NetworkManager instance;

		public static NetworkManager Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new NetworkManager ();
				return instance;
			}
		}

		readonly List<IUpstream> upstreamSources;

		public NetworkManager () {
			upstreamSources = new List<IUpstream> ();
		}

		public void RegisterUpstream (IUpstream upstream) {
			upstreamSources.Add (upstream);
		}

		public void Notify (string format, params object[] args) {
			foreach (var upstream in upstreamSources)
				upstream.Notify (format, args);
		}
	}
}

