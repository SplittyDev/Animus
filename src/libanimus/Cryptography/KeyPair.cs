using System;

namespace libanimus
{
	public class KeyPair
	{
		public readonly string PrivateKey;
		public readonly string PublicKey;

		public bool HasPrivateKey {
			get { return !string.IsNullOrEmpty (PrivateKey); }
		}

		public KeyPair (string publicKey) {
			PublicKey = publicKey;
		}

		public KeyPair (string publicKey, string privateKey) : this (publicKey) {
			PrivateKey = privateKey;
		}
	}
}

