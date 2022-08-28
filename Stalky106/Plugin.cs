namespace Stalky106
{
	using System.Collections.Generic;
	using Exiled.API.Features;
	using MEC;
	using System;
	using Handlers = Exiled.Events.Handlers;

	public class StalkyPlugin : Plugin<Config, Translations>
	{
		public override string Author { get; } = "RogerFK && Raul125";

		public override string Name { get; } = "Stalky106";

		public override string Prefix { get; } = "ST106";

		public override Version RequiredExiledVersion { get; } = new Version(5, 3, 0);

		public const string VersionStr = "3.3.4";

		public override Version Version { get; } = new Version(VersionStr);

        public EventHandlers EventHandlers { get; private set; }

		public Methods Methods { get; private set; }

		public readonly List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

		public override void OnEnabled()
		{
			Methods = new Methods(this);
			EventHandlers = new EventHandlers(this);

			Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
			Handlers.Server.RestartingRound += EventHandlers.OnRestartingRound;
			Handlers.Player.ChangingRole += EventHandlers.OnChangingRole;
			Handlers.Scp106.CreatingPortal += EventHandlers.OnCreatingPortal;

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
			Handlers.Server.RestartingRound -= EventHandlers.OnRestartingRound;
			Handlers.Player.ChangingRole -= EventHandlers.OnChangingRole;
			Handlers.Scp106.CreatingPortal -= EventHandlers.OnCreatingPortal;

			EventHandlers = null;
			Methods = null;

			base.OnDisabled();
		}
	}
}
