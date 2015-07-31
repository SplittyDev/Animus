using System;
using System.Security.Cryptography;
using System.Text;

namespace libanimus
{
	public static class PublicKeyEncryptionProvider
	{
		public static KeyPair GenerateKeyPair () {
			var rsa = new RSACryptoServiceProvider ();
			var privateXml = rsa.ToXmlString (true);
			var publicXml = rsa.ToXmlString (false);
			return new KeyPair (publicXml, privateXml);
		}

		public static byte[] EncryptRaw (byte[] str, string publicKey, bool fOAEP) {
			var rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (publicKey);
			return rsa.Encrypt (str, fOAEP);
		}

		public static byte[] EncryptRaw (string str, string publicKey, bool fOAEP) {
			return EncryptRaw (Encoding.ASCII.GetBytes (str), publicKey, fOAEP);
		}

		public static string Encrypt (byte[] str, string publicKey, bool fOAEP) {
			return Encoding.ASCII.GetString (EncryptRaw (str, publicKey, fOAEP));
		}

		public static string Encrypt (string str, string publicKey, bool fOAEP) {
			return Encoding.ASCII.GetString (EncryptRaw (str, publicKey, fOAEP));
		}

		public static byte[] DecryptRaw (byte[] str, string privateKey, bool fOAEP) {
			var rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (privateKey);
			return rsa.Decrypt (str, fOAEP);
		}

		public static byte[] DecryptRaw (string str, string privateKey, bool fOAEP) {
			return DecryptRaw (Encoding.ASCII.GetBytes (str), privateKey, fOAEP);
		}

		public static string Decrypt (byte[] str, string privateKey, bool fOAEP) {
			return Encoding.ASCII.GetString (DecryptRaw (str, privateKey, fOAEP));
		}

		public static string Decrypt (string str, string privateKey, bool fOAEP) {
			return Encoding.ASCII.GetString (DecryptRaw (str, privateKey, fOAEP));
		}
	}
}

