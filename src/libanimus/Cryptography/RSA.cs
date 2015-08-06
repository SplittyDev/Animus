using System;

namespace libanimus.Cryptography
{
	/// <summary>
	/// RSA encryption provider.
	/// </summary>
	public class RSA
	{
		/// <summary>
		/// The key pair.
		/// </summary>
		KeyPair keyPair;

		/// <summary>
		/// Initializes a new instance of the <see cref="RSA"/> class.
		/// </summary>
		/// <param name="publicKey">Public key.</param>
		public RSA (string publicKey) {
			keyPair = new KeyPair (publicKey);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RSA"/> class.
		/// </summary>
		/// <param name="publicKey">Public key.</param>
		/// <param name="privateKey">Private key.</param>
		public RSA (string publicKey, string privateKey) {
			keyPair = new KeyPair (publicKey, privateKey);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RSA"/> class.
		/// </summary>
		/// <param name="keyPair">Key pair.</param>
		public RSA (KeyPair keyPair) {
			this.keyPair = keyPair;
		}

		/// <summary>
		/// Encrypt the specified str.
		/// </summary>
		/// <param name="str">String.</param>
		public string Encrypt (string str) {
			return RSA.Encrypt (str, keyPair);
		}

		/// <summary>
		/// Encrypt the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public byte[] Encrypt (byte[] data) {
			return RSA.Encrypt (data, keyPair);
		}

		/// <summary>
		/// Decrypt the specified str.
		/// </summary>
		/// <param name="str">String.</param>
		public string Decrypt (string str) {
			return RSA.Decrypt (str, keyPair);
		}

		/// <summary>
		/// Decrypt the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public byte[] Decrypt (byte[] data) {
			return RSA.Decrypt (data, keyPair);
		}

		/// <summary>
		/// Generates a key pair.
		/// </summary>
		/// <returns>The key pair.</returns>
		public static KeyPair GenerateKeyPair () {
			return RSABase.GenerateKeyPair ();
		}

		/// <summary>
		/// Encrypt the specified str using the specified publicKey.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="publicKey">Public key.</param>
		public static string Encrypt (string str, string publicKey) {
			return RSABase.Encrypt (str, publicKey, true);
		}

		/// <summary>
		/// Encrypt the specified data using the specified publicKey.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="publicKey">Public key.</param>
		public static byte[] Encrypt (byte[] data, string publicKey) {
			return RSABase.EncryptRaw (data, publicKey, true);
		}

		/// <summary>
		/// Encrypt the specified str using the specified keys.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="keys">Keys.</param>
		public static string Encrypt (string str, KeyPair keys) {
			return Encrypt (str, keys.PublicKey);
		}

		/// <summary>
		/// Encrypt the specified data using the specified keys.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="keys">Keys.</param>
		public static byte[] Encrypt (byte[] data, KeyPair keys) {
			return Encrypt (data, keys.PublicKey);
		}

		/// <summary>
		/// Decrypt the specified str using the specified privateKey.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="privateKey">Private key.</param>
		public static string Decrypt (string str, string privateKey) {
			return RSABase.Decrypt (str, privateKey, true);
		}

		/// <summary>
		/// Decrypt the specified data uing the specified privateKey.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="privateKey">Private key.</param>
		public static byte[] Decrypt (byte[] data, string privateKey) {
			return RSABase.DecryptRaw (data, privateKey, true);
		}

		/// <summary>
		/// Decrypt the specified str using the specified keys.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="keys">Keys.</param>
		public static string Decrypt (string str, KeyPair keys) {
			if (!keys.HasPrivateKey)
				throw new Exception ("The KeyPair doesn't contain a private key.");
			return Decrypt (str, keys.PrivateKey);
		}

		/// <summary>
		/// Decrypt the specified data using the specified keys.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="keys">Keys.</param>
		public static byte[] Decrypt (byte[] data, KeyPair keys) {
			if (!keys.HasPrivateKey)
				throw new Exception ("This KeyPair doesn't contain a private key.");
			return Decrypt (data, keys.PrivateKey);
		}
	}
}

