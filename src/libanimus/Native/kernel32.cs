using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace libanimus.Native
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

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <returns><c>true</c>, if file w was deleted, <c>false</c> otherwise.</returns>
		/// <param name="lpFileName">Lp file name.</param>
		[DllImport ("kernel32.dll", SetLastError = true)]
		[return: MarshalAs (UnmanagedType.Bool)]
		public static extern bool DeleteFileW ([MarshalAs (UnmanagedType.LPWStr)] string lpFileName);

		/// <summary>
		/// Creates a process.
		/// </summary>
		/// <returns><c>true</c>, if process was created, <c>false</c> otherwise.</returns>
		/// <param name="lpApplicationName">Lp application name.</param>
		/// <param name="lpCommandLine">Lp command line.</param>
		/// <param name="lpProcessAttributes">Lp process attributes.</param>
		/// <param name="lpThreadAttributes">Lp thread attributes.</param>
		/// <param name="bInheritHandles">If set to <c>true</c> b inherit handles.</param>
		/// <param name="dwCreationFlags">Dw creation flags.</param>
		/// <param name="lpEnvironment">Lp environment.</param>
		/// <param name="lpCurrentDirectory">Lp current directory.</param>
		/// <param name="lpStartupInfo">Lp startup info.</param>
		/// <param name="lpProcessInformation">Lp process information.</param>
		[DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool CreateProcess(
			string lpApplicationName,
			string lpCommandLine,
			ref SECURITY_ATTRIBUTES lpProcessAttributes, 
			ref SECURITY_ATTRIBUTES lpThreadAttributes,
			bool bInheritHandles, 
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo, 
			out PROCESS_INFORMATION lpProcessInformation);

		/// <summary>
		/// Creates a process.
		/// </summary>
		/// <returns><c>true</c>, if process was created, <c>false</c> otherwise.</returns>
		/// <param name="lpApplicationName">Lp application name.</param>
		/// <param name="lpCommandLine">Lp command line.</param>
		/// <param name="lpProcessAttributes">Lp process attributes.</param>
		/// <param name="lpThreadAttributes">Lp thread attributes.</param>
		/// <param name="bInheritHandles">If set to <c>true</c> b inherit handles.</param>
		/// <param name="dwCreationFlags">Dw creation flags.</param>
		/// <param name="lpEnvironment">Lp environment.</param>
		/// <param name="lpCurrentDirectory">Lp current directory.</param>
		/// <param name="lpStartupInfo">Lp startup info.</param>
		/// <param name="lpProcessInformation">Lp process information.</param>
		[DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool CreateProcess(
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes, 
			IntPtr lpThreadAttributes,
			bool bInheritHandles, 
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo, 
			out PROCESS_INFORMATION lpProcessInformation);

		/// <summary>
		/// Gets the module handle.
		/// </summary>
		/// <returns>The module handle.</returns>
		/// <param name="lpModuleName">Lp module name.</param>
		[DllImport ("kernel32.dll")]
		public static extern IntPtr GetModuleHandle (string lpModuleName);
	}
}

