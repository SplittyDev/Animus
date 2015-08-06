using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace libanimus.Native
{
	public static partial class Native
	{
		[DllImport ("gdi32.dll")]
		public static extern bool BitBlt (IntPtr hdcDest, int xDest, int yDest,
			int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

		[DllImport ("user32.dll")]
		public static extern bool ReleaseDC (IntPtr hWnd, IntPtr hDc);

		[DllImport ("gdi32.dll")]
		public static extern IntPtr DeleteDC (IntPtr hDc);

		[DllImport ("gdi32.dll")]
		public static extern IntPtr DeleteObject (IntPtr hDc);

		[DllImport ("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap (IntPtr hdc, int nWidth, int nHeight);

		[DllImport ("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC (IntPtr hdc);

		[DllImport ("gdi32.dll")]
		public static extern IntPtr SelectObject (IntPtr hdc, IntPtr bmp);

		[DllImport ("user32.dll")]
		public static extern IntPtr GetDesktopWindow ();

		[DllImport ("user32.dll")]
		public static extern IntPtr GetWindowDC (IntPtr ptr);
	}
}

