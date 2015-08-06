using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace libanimus.Networking
{
	public class Updater
	{
		readonly static object syncRoot = new object ();
		static Updater instance;

		public static Updater Instance {
			get {
				if (instance == null)
					lock (syncRoot)
						if (instance == null)
							instance = new Updater ();
				return instance;
			}
		}

		public event EventHandler<UpdateAvailableEventArgs> UpdateAvailable;
		public event EventHandler NoUpdateAvailable;
		public event EventHandler UpdatePrepared;

		WebClient wc;
		string location;

		public Updater () {
			wc = new WebClient ();
			wc.Proxy = new WebProxy ();
			NoUpdateAvailable += (sender, e) => { };
			UpdateAvailable += (sender, e) => { };
			UpdatePrepared += UpdatePreparedCallback;
		}

		public void StartupCheck () {
			if (Upgrading ())
				ApplyCoreUpgrade ();
			if (CleanupNeeded ())
				ApplyCleanup ();
		}

		public void CheckUpdate (string uri) {
			wc.DownloadStringCompleted += CheckUpdateCallback;
			wc.DownloadStringAsync (new Uri (string.Format ("{0}/version.txt", uri)));
		}

		public void DownloadUpdate (string uri) {
			location = string.Format ("{0}.exe", Path.GetTempFileName ());
			wc.DownloadFileCompleted += DownloadUpdateCallback;
			wc.DownloadFileAsync (new Uri (string.Format ("{0}/animus.exe", uri)), location);
		}

		void CheckUpdateCallback (object sender, DownloadStringCompletedEventArgs e) {
			wc.DownloadStringCompleted -= CheckUpdateCallback;
			var newVersion = Version.Parse (e.Result);
			var curVersion = Assembly.GetExecutingAssembly ().GetName ().Version;
			var diff = newVersion.CompareTo (curVersion);
			if (diff > 0)
				UpdateAvailable (this, new UpdateAvailableEventArgs (curVersion, newVersion));
			else
				NoUpdateAvailable (this, EventArgs.Empty);
		}

		void DownloadUpdateCallback (object sender, AsyncCompletedEventArgs e) {
			UpdatePrepared (this, EventArgs.Empty);
		}

		void UpdatePreparedCallback (object sender, EventArgs e) {
			var psi = new ProcessStartInfo {
				Arguments = "--apply-upgrade",
				CreateNoWindow = true,
				FileName = location,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = Environment.CurrentDirectory,
			};
			NetworkManager.Instance.Notify ("Going down for core upgrade. brb!");
			Process.Start (psi);
			Process.GetCurrentProcess ().Kill ();
		}

		void ApplyCoreUpgrade () {
			File.Delete ("animus.exe");
			location = Assembly.GetExecutingAssembly ().Location;
			File.Copy (location, "animus.exe");
			var psi = new ProcessStartInfo {
				Arguments = string.Format ("--cleanup \"{0}\"", location),				CreateNoWindow = true,
				FileName = "animus.exe",
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = Environment.CurrentDirectory,
			};
			NetworkManager.Instance.Notify ("Going down for after-update cleanup. brb!");
			Process.Start (psi);
			Process.GetCurrentProcess ().Kill ();
		}

		void ApplyCleanup () {
			File.Delete (location);
			var version = Assembly.GetExecutingAssembly ().GetName ().Version;
			NetworkManager.Instance.Notify ("Upgrade complete. New version: {0}", version);
		}

		bool Upgrading () {
			var args = Environment.GetCommandLineArgs ().Skip (1).ToArray ();
			if (args.Length > 0) {
				if (args.First () == "--apply-upgrade") {
					location = "animus.exe";
					return true;
				}
			}
			return false;
		}

		bool CleanupNeeded () {
			var args = Environment.GetCommandLineArgs ().Skip (1).ToArray ();
			if (args.Length > 1) {
				if (args.First () == "--cleanup") {
					location = args.Skip (1).First ();
					return true;
				}
			}
			return false;
		}
	}
}

