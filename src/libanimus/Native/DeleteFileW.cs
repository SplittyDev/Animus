using System;
using System.Runtime.InteropServices;

namespace libanimus
{
	public static partial class Native
	{
		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <returns><c>true</c>, if file w was deleted, <c>false</c> otherwise.</returns>
		/// <param name="lpFileName">Lp file name.</param>
		[DllImport ("kernel32.dll", SetLastError = true)]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool DeleteFileW ([MarshalAs (UnmanagedType.LPWStr)] string lpFileName);
	}
}

