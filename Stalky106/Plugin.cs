using System.Collections.Generic;

using Exiled.API.Features;

using UnityEngine;

using PlyEvents = Exiled.Events.Handlers.Player;
using SvEvents = Exiled.Events.Handlers.Server;
using ScpEvents = Exiled.Events.Handlers.Scp106;

namespace Stalky106
{
	public class StalkyPlugin : Plugin<PluginConfig>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "More visually appealing naming style")]
		public const string VersionStr = "3.0";
		private readonly List<MEC.CoroutineHandle> coroutines = new List<MEC.CoroutineHandle>();
		public void AddCoroutine(MEC.CoroutineHandle coroutineHandle) => coroutines.Add(coroutineHandle);
		public void NewCoroutine(IEnumerator<float> coroutine, MEC.Segment segment = MEC.Segment.Update) => coroutines.Add(MEC.Timing.RunCoroutine(coroutine, segment));
		public override string Prefix => "ST106";
		public static readonly Vector3 pocketDimension = new Vector3(0f, -1998f, 0f);

		private EventHandlers events;
		public StalkyMethods Methods { private set; get; }

		public override void OnEnabled()
		{
			if (!Config.IsEnabled)
			{
				Log.Info("Stalky106 is disabled via configs. It will not be loaded.");
				return;
			}
			Log.Info("Prepare to face Larry...");
			Methods = new StalkyMethods(this);
			events = new EventHandlers(this);
			SvEvents.RoundStarted += events.OnRoundStart;
			PlyEvents.ChangingRole += events.OnSetClass;
			ScpEvents.CreatingPortal += events.OnCreatePortal;
			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			if (coroutines != null && coroutines.Count > 0) MEC.Timing.KillCoroutines(coroutines.ToArray());
			if (events != null)
			{
				SvEvents.RoundStarted -= events.OnRoundStart;
				PlyEvents.ChangingRole -= events.OnSetClass;
				ScpEvents.CreatingPortal -= events.OnCreatePortal;
				events = null;
			}
			
			Methods = null;

			if (Config.IsEnabled)
			{
				Log.Info("Larry won't ever stalk you again at night...");
				base.OnDisabled();
			}
		}
	}
}
