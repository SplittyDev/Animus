using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gtk;
using IrcTest;
using libanimus.Networking;

partial class MainWindow: Gtk.Window
{
	ServerModel server_model;
	UserModel user_model;
	IrcClient client;
	ConcurrentQueue<string> textbuf;
	bool updating;

	public MainWindow () : base (Gtk.WindowType.Toplevel) {
		Build ();

		textbuf = new ConcurrentQueue<string> ();
		server_model = new ServerModel ();
		user_model = new UserModel ();
		client = new IrcClient ();

		var col_servers = new TreeViewColumn { Title = "Servers" };
		var server_renderer = new CellRendererText ();
		col_servers.PackStart (server_renderer, true);
		treeview_servers.AppendColumn (col_servers);
		col_servers.AddAttribute (server_renderer, "text", 0);
		treeview_servers.Model = server_model;

		var col_users = new TreeViewColumn { Title = "Users" };
		var user_renderer = new CellRendererText ();
		col_users.PackStart (user_renderer, true);
		treeview_users.AppendColumn (col_users);
		col_users.AddAttribute (user_renderer, "text", 0);
		treeview_users.Model = user_model;

		client.Connected += (hostname, port) => server_model.AppendValues (hostname);

		client.LoggedIn += (sender, e) => {
		};

		client.ChannelJoined += channel => {
			lbl_topic.Text = channel;
			TreeIter iter;
			server_model.GetIter (out iter, new TreePath ("0"));
			server_model.AppendValues (iter, channel);
		};

		client.NamesObtained += names => {
			user_model.Clear ();
			foreach (var name in names)
				user_model.AppendValues (name);
		};

		client.ChannelMessage += async (message, sender) => {
			textbuf.Enqueue (string.Format ("[{0}] {1}\n", sender, message));
			if (updating)
				return;
			updating = true;
			await Task.Factory.StartNew (() => {
				Application.Invoke (delegate {
					var iter = txt_chat.Buffer.EndIter;
					while (textbuf.Count > 0) {
						string value;
						if (textbuf.TryDequeue (out value))
							txt_chat.Buffer.Insert (ref iter, value);
					}
				});
			});
			updating = false;
		};

		client.Connect ("int0x10.com", 6697, true);
		client.LogIn ("spl1tty", "spl1tty", "spl1tty");
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a) {
		Application.Quit ();
		a.RetVal = true;
	}

	protected async void OnEntryChatActivated (object sender, EventArgs e)
	{
		client.Message (entry_chat.Text, "#int0x10");
		textbuf.Enqueue (string.Format ("[{0}] {1}\n", client.Nickname, entry_chat.Text));
		await Task.Factory.StartNew (() => {
			Application.Invoke (delegate {
				var iter = txt_chat.Buffer.EndIter;
				while (textbuf.Count > 0) {
					string value;
					if (textbuf.TryDequeue (out value))
						txt_chat.Buffer.Insert (ref iter, value);
				}
			});
		});
		entry_chat.Text = "";
	}
}
