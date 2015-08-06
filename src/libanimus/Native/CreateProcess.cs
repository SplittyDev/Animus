using System;
using System.Runtime.InteropServices;

namespace libanimus.Native
{
	public static partial class Native
	{
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
		/// PROCESS INFORMATION.
		/// </summary>
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public uint dwProcessId;
			public uint dwThreadId;
		}

		/// <summary>
		/// STARTUP INFO.
		/// </summary>
		public struct STARTUPINFO
		{
			public uint cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		/// <summary>
		/// SECURITY ATTRIBUTES.
		/// </summary>
		public struct SECURITY_ATTRIBUTES
		{
			public int length;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}
	}
}

