using System;

namespace libanimus.Cryptography
{
	/// <summary>
	/// A private/public key pair.
	/// </summary>
	public class KeyPair
	{
		/// <summary>
		/// The private key.
		/// </summary>
		public readonly string PrivateKey;

		/// <summary>
		/// The public key.
		/// </summary>
		public readonly string PublicKey;

		/// <summary>
		/// Gets a value indicating whether this instance has a private key.
		/// </summary>
		/// <value><c>true</c> if this instance has a private key; otherwise, <c>false</c>.</value>
		public bool HasPrivateKey {
			get { return !string.IsNullOrEmpty (PrivateKey); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.KeyPair"/> class.
		/// </summary>
		/// <param name="publicKey">Public key.</param>
		public KeyPair (string publicKey) {
			PublicKey = publicKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="libanimus.KeyPair"/> class.
		/// </summary>
		/// <param name="publicKey">Public key.</param>
		/// <param name="privateKey">Private key.</param>
		public KeyPair (string publicKey, string privateKey) : this (publicKey) {
			PrivateKey = privateKey;
		}
	}
}

