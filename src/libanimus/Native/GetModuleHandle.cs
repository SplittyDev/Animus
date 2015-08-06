using System;
using System.Runtime.InteropServices;

namespace libanimus.Native
{
	public static partial class Native
	{
		[DllImport ("kernel32.dll")]
		public static extern IntPtr GetModuleHandle (string lpModuleName);
	}
}

