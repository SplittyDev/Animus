using System;
using System.Runtime.InteropServices;

namespace libanimus
{
	public static partial class Native
	{
		/// <summary>
		/// Checks if a file exists, supports alternative NTFS streams.
		/// </summary>
		/// <returns><c>true</c>, if the file exists, <c>false</c> otherwise.</returns>
		/// <param name="pszPath">Psz path.</param>
		[DllImport ("shlwapi.dll", EntryPoint = "PathFileExistsW",  SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool PathFileExists ([MarshalAs (UnmanagedType.LPTStr)] string pszPath);
	}
}

