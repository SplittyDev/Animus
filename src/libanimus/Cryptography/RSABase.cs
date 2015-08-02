using System;
using System.Security.Cryptography;
using System.Text;

namespace libanimus
{
	/// <summary>
	/// RSA encryption provider base.
	/// </summary>
	static class RSABase
	{
		/// <summary>
		/// Generates a key pair.
		/// </summary>
		/// <returns>The key pair.</returns>
		public static KeyPair GenerateKeyPair () {
			var rsa = new RSACryptoServiceProvider ();
			var privateXml = rsa.ToXmlString (true);
			var publicXml = rsa.ToXmlString (false);
			return new KeyPair (publicXml, privateXml);
		}

		/// <summary>
		/// Encrypts the specified data.
		/// </summary>
		/// <returns>The encrypted data.</returns>
		/// <param name="data">Data.</param>
		/// <param name="publicKey">Public key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static byte[] EncryptRaw (byte[] data, string publicKey, bool fOAEP) {
			var rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (publicKey);
			return rsa.Encrypt (data, fOAEP);
		}

		/// <summary>
		/// Encrypts the specified string.
		/// </summary>
		/// <returns>The encrypted string.</returns>
		/// <param name="str">String.</param>
		/// <param name="publicKey">Public key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static byte[] EncryptRaw (string str, string publicKey, bool fOAEP) {
			return EncryptRaw (Encoding.ASCII.GetBytes (str), publicKey, fOAEP);
		}

		/// <summary>
		/// Encrypt the specified data using the specified publicKey.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="publicKey">Public key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static string Encrypt (byte[] data, string publicKey, bool fOAEP) {
			return Encoding.ASCII.GetString (EncryptRaw (data, publicKey, fOAEP));
		}

		/// <summary>
		/// Encrypt the specified str using the specified publicKey.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="publicKey">Public key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static string Encrypt (string str, string publicKey, bool fOAEP) {
			return Encoding.ASCII.GetString (EncryptRaw (str, publicKey, fOAEP));
		}

		/// <summary>
		/// Decrypts the specified data.
		/// </summary>
		/// <returns>The decrypted data.</returns>
		/// <param name="data">Data.</param>
		/// <param name="privateKey">Private key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static byte[] DecryptRaw (byte[] data, string privateKey, bool fOAEP) {
			var rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (privateKey);
			return rsa.Decrypt (data, fOAEP);
		}

		/// <summary>
		/// Decrypts the specified string.
		/// </summary>
		/// <returns>The decrypted string.</returns>
		/// <param name="str">String.</param>
		/// <param name="privateKey">Private key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static byte[] DecryptRaw (string str, string privateKey, bool fOAEP) {
			return DecryptRaw (Encoding.ASCII.GetBytes (str), privateKey, fOAEP);
		}

		/// <summary>
		/// Decrypt the specified data using the specified privateKey.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="privateKey">Private key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static string Decrypt (byte[] data, string privateKey, bool fOAEP) {
			return Encoding.ASCII.GetString (DecryptRaw (data, privateKey, fOAEP));
		}

		/// <summary>
		/// Decrypt the specified str using the specified privateKey.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="privateKey">Private key.</param>
		/// <param name="fOAEP">If set to <c>true</c> use fOAEP.</param>
		public static string Decrypt (string str, string privateKey, bool fOAEP) {
			return Encoding.ASCII.GetString (DecryptRaw (str, privateKey, fOAEP));
		}
	}
}

