using System;
using libanimus.Actions;

namespace libanimus
{
	public static class ActionPoolExtensions
	{
		public static void Register<TAction> (this ActionPool dummy) where TAction : HostAction, new() {
			var action = new TAction ();
			ActionPool.Instance.RegisterAction (action);
		}
	}
}

