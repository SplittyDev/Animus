using System;
using System.Runtime.InteropServices;

namespace libanimus.Native
{
	public static partial class Native
	{
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

