using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;

namespace Stalky106
{
	internal class EventHandlers
	{
		private readonly StalkyPlugin plugin;
		public EventHandlers(StalkyPlugin plugin)
		{
			this.plugin = plugin;
		}

		internal void OnRoundStart()
		{
			if (!plugin.Config.IsEnabled) return;

			// should never be null but you never know
			if (plugin.Methods != null)
			{
				plugin.Methods.StalkyCooldown = plugin.Config.Preferences.InitialCooldown;
			}
		}

		internal void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (!plugin.Config.IsEnabled || ev.Player.GameObject == PlayerManager.localPlayer) return;

			if (ev.NewRole == RoleType.Scp106)
			{
				plugin.AddCoroutine(MEC.Timing.CallDelayed(0.5f, () =>
					{
						if (ev.Player.Role == RoleType.Scp106)
						{
							ev.Player.Broadcast(10, plugin.Config.Translations.WelcomeBroadcast);
							ev.Player.SendConsoleMessage(plugin.Config.Translations.ConsoleInfo, "white");
						}
					}));
			}
		}

		internal void OnCreatePortal(CreatingPortalEventArgs ev) => ev.IsAllowed = plugin.Methods.Stalk(ev.Player);
	}
}

