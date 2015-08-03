using System;
using System.Net.Security;

namespace libanimus {
	public struct IrcClientConnectionOptions {
		public bool Ssl;
		public string SslHostname;
		public RemoteCertificateValidationCallback SslCertificateValidationCallback;
	}
}

