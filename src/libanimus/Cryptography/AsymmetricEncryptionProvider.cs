using System;

namespace libanimus
{
	public static class AsymmetricEncryptionProvider
	{
		public static string Encrypt (string str, string publicKey) {
			return PublicKeyEncryptionProvider.Encrypt (str, publicKey, true);
		}

		public static string Encrypt (string str, KeyPair keys) {
			return Encrypt (str, keys.PublicKey);
		}

		public static string Decrypt (string str, string privateKey) {
			return PublicKeyEncryptionProvider.Decrypt (str, privateKey, true);
		}

		public static string Decrypt (string str, KeyPair keys) {
			if (!keys.HasPrivateKey)
				throw new Exception ("The KeyPair doesn't contain a private key.");
			return Decrypt (str, keys.PrivateKey);
		}
	}
}

