using System;

namespace libanimus {
	[Serializable]
	public class ConnectionFailedException : Exception {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ConnectionFailedException"/> class
		/// </summary>
		public ConnectionFailedException () {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ConnectionFailedException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public ConnectionFailedException (string message) : base (message) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ConnectionFailedException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public ConnectionFailedException (string message, Exception inner) : base (message, inner) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ConnectionFailedException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected ConnectionFailedException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context) {
		}
	}
}

