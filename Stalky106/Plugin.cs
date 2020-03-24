using EXILED;
using System.Collections.Generic;

namespace Stalky106
{
	public class Stalky106 : Plugin
	{
		private EventHandlers events;
		internal readonly static UnityEngine.Vector3 pocketDimension = new UnityEngine.Vector3(0f, -1998f, 0f);
		public static UnityEngine.Vector3 PocketDimension
		{
			get
			{
				return pocketDimension;
			}
		}
		public static int harmonyCounter;
		public const string Version = "V1.3";
		public bool enabled;
		public static List<MEC.CoroutineHandle> Coroutines { set; get; }
		public override void OnDisable()
		{
			if (!enabled) return;
			if (Coroutines != null) MEC.Timing.KillCoroutines(Coroutines);
			Events.RoundStartEvent -= events.OnRoundStart;
			Events.SetClassEvent -= events.OnSetClass;
			Events.RemoteAdminCommandEvent -= events.RACommand;
			Events.Scp106CreatedPortalEvent -= events.OnCreatePortal;
			Log.Info("Larry won't ever stalk you again at night...");
		}

		public override void OnEnable()
		{
			enabled = Config.GetBool("stalky_enable", true);
			if (!enabled)
			{
				Log.Error("Stalky106 is disabled via configs. It will not be loaded.");
				return;
			}
			Log.Info("Prepare to face Larry...");
			events = new EventHandlers();
			StalkyConfigs.ReloadConfigs();
			Events.RoundStartEvent += events.OnRoundStart;
			Events.SetClassEvent += events.OnSetClass;
			Events.RemoteAdminCommandEvent += events.RACommand;
			Events.Scp106CreatedPortalEvent += events.OnCreatePortal;
		}

		public override string getName => "Stalky106-[TAB]";

		public override void OnReload()
		{
		}
	}
}
