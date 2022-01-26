using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using System;
using Handlers = Exiled.Events.Handlers;

namespace Stalky106
{
	public class StalkyPlugin : Plugin<Config, Translations>
	{
		public override string Author { get; } = "RogerFK && Raul125";
		public override string Name { get; } = "Stalky106";
		public override string Prefix { get; } = "ST106";
		public override Version RequiredExiledVersion { get; } = new Version(4, 2, 3);
		public const string VersionStr = "3.3.0";
		public override Version Version { get; } = new Version(VersionStr);
        public EventHandlers EventHandlers { get; private set; }
		public Methods Methods { get; private set; }

		public readonly List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public static readonly Vector3 pocketDimension = new Vector3(0f, -1998f, 0f);

		public override void OnEnabled()
		{
			Methods = new Methods(this);
			EventHandlers = new EventHandlers(this);

			Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
			Handlers.Server.RestartingRound += EventHandlers.OnRestartingRound;
			Handlers.Player.ChangingRole += EventHandlers.OnSetClass;
			Handlers.Scp106.CreatingPortal += EventHandlers.OnCreatePortal;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
			Handlers.Server.RestartingRound -= EventHandlers.OnRestartingRound;
			Handlers.Player.ChangingRole -= EventHandlers.OnSetClass;
			Handlers.Scp106.CreatingPortal -= EventHandlers.OnCreatePortal;

			EventHandlers = null;
			Methods = null;

			base.OnDisabled();
		}
	}
}
