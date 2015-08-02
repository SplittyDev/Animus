using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace libanimus
{
	public static partial class Native
	{
		/// <summary>
		/// Creates a file.
		/// </summary>
		/// <returns>The file w.</returns>
		/// <param name="filename">Filename.</param>
		/// <param name="access">Access.</param>
		/// <param name="share">Share.</param>
		/// <param name="securityAttributes">Security attributes.</param>
		/// <param name="creationDisposition">Creation disposition.</param>
		/// <param name="flagsAndAttributes">Flags and attributes.</param>
		/// <param name="templateFile">Template file.</param>
		[DllImport ("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFileW (
			[MarshalAs (UnmanagedType.LPWStr)] string filename,
			[MarshalAs (UnmanagedType.U4)] FileAccess access,
			[MarshalAs (UnmanagedType.U4)] FileShare share,
			IntPtr securityAttributes,
			[MarshalAs (UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs (UnmanagedType.U4)] FileAttributes flagsAndAttributes,
			IntPtr templateFile
		);
	}
}

