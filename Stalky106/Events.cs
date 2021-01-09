using System;
using System.Collections.Generic;

using Exiled.API.Features;
using Exiled.Events.EventArgs;

using NorthwoodLib.Pools;
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

		internal void OnCreatePortal(CreatingPortalEventArgs ev) 
		{
			if (!ev.IsAllowed) return;

			/* TODO: Move everything that isn't specifically related to stalking/hunting here */

			var list = ListPool<Player>.Shared.Rent(Player.List);
			int playerCount = list.Count;

			if (plugin.Config.Preferences.MinimumPlayers > playerCount)
			{
				ev.Player.Broadcast(8, plugin.Config.Translations.MinPlayers.Replace("$count", plugin.Config.Preferences.MinimumPlayers.ToString()));
				plugin.Methods.StalkyCooldown = 10f;
				return;
			}

			int aliveCount = 0;
			int targetCount = 0;
			for (int i = 0; i < playerCount; i++) 
			{
				var ply = list[i];
				// if it's a target
				if (ply.IsAlive) 
				{
					aliveCount++;
					if (!plugin.Config.Preferences.IgnoreRoles.Contains(ply.Role) && !plugin.Config.Preferences.IgnoreTeams.Contains(ply.Team))
					{
						targetCount++;
					}
				}
			}

			if (plugin.Config.Preferences.MinimumAlivePlayers > aliveCount) 
			{
				ev.Player.Broadcast(8, plugin.Config.Translations.MinAlive.Replace("$count", plugin.Config.Preferences.MinimumAlivePlayers.ToString()));
				plugin.Methods.StalkyCooldown = 10f;
				return;
			}

			if (plugin.Config.Preferences.MinimumAliveTargets > targetCount)
			{
				ev.Player.Broadcast(8, plugin.Config.Translations.MinTargetsAlive.Replace("$count", plugin.Config.Preferences.MinimumAliveTargets.ToString()));
				plugin.Methods.StalkyCooldown = 10f;
				return;
			}

			ListPool<Player>.Shared.Return(list);
			ev.IsAllowed = plugin.Methods.Stalk(ev.Player);
		}
	}
}

