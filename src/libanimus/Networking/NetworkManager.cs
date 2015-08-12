using System;
using System.Collections.Generic;
using System.Linq;

namespace libanimus.Networking
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

		/// <summary>
		/// The identifier.
		/// </summary>
		public readonly string Identifier;

		/// <summary>
		/// The selected identifier.
		/// </summary>
		public string SelectedIdentifier;

		/// <summary>
		/// Gets a value indicating whether this instance is selected.
		/// </summary>
		/// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
		public bool IsSelected { get { return Identifier == SelectedIdentifier; } }

		/// <summary>
		/// The GUID.
		/// </summary>
		readonly Guid guid;

		/// <summary>
		/// The upstream sources.
		/// </summary>
		readonly List<IUpstream> upstreamSources;

		public NetworkManager () {
			upstreamSources = new List<IUpstream> ();
			guid = Guid.NewGuid ();
			Identifier = string.Format ("animus{0}", new string (guid.ToString ("N").Take (16).ToArray ()));
			SelectedIdentifier = string.Empty;
		}

		public void RegisterUpstream (IUpstream upstream) {
			upstreamSources.Add (upstream);
		}

		public void Broadcast (string format, params object[] args) {
			foreach (var upstream in upstreamSources)
				upstream.Notify (format, args);
		}

		public void Notify (IUpstream source, string format, params object[] args) {
			source.Notify (format, args);
		}

		public void Notify<TUpstream> (string format, params object[] args) where TUpstream : IUpstream {
			var upstreams = upstreamSources.OfType<TUpstream> ();
			foreach (var upstream in upstreams)
				upstream.Notify (format, args);
		}

		public TUpstream GetUpstream<TUpstream> () where TUpstream : class, IUpstream {
			if (upstreamSources.Any (src => src is TUpstream))
				return upstreamSources.First (src => src is TUpstream) as TUpstream;
			return null;
		}
	}
}

