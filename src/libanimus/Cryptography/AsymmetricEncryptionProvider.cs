using System;

namespace libanimus
{
	public static class AsymmetricEncryptionProvider
	{
		public static string Encrypt (string str, string publicKey) {
			return PublicKeyEncryptionProvider.Encrypt (str, publicKey, true);
		}

		public static byte[] Encrypt (byte[] data, string publicKey) {
			return PublicKeyEncryptionProvider.EncryptRaw (data, publicKey, true);
		}

		public static string Encrypt (string str, KeyPair keys) {
			return Encrypt (str, keys.PublicKey);
		}

		public static byte[] Encrypt (byte[] data, KeyPair keys) {
			return Encrypt (data, keys.PublicKey);
		}

		public static string Decrypt (string str, string privateKey) {
			return PublicKeyEncryptionProvider.Decrypt (str, privateKey, true);
		}

		public static byte[] Decrypt (byte[] data, string privateKey) {
			return PublicKeyEncryptionProvider.DecryptRaw (data, privateKey, true);
		}

		public static string Decrypt (string str, KeyPair keys) {
			if (!keys.HasPrivateKey)
				throw new Exception ("The KeyPair doesn't contain a private key.");
			return Decrypt (str, keys.PrivateKey);
		}

		public static byte[] Decrypt (byte[] data, KeyPair keys) {
			if (!keys.HasPrivateKey)
				throw new Exception ("This KeyPair doesn't contain a private key.");
			return Decrypt (data, keys.PrivateKey);
		}
	}
}

