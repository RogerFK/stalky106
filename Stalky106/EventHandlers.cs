using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using NorthwoodLib.Pools;

namespace Stalky106
{
	public class EventHandlers
	{
		private readonly StalkyPlugin plugin;

		public EventHandlers(StalkyPlugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundStart()
		{
			plugin.Methods.StalkyCooldown = plugin.Config.Preferences.InitialCooldown;
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (ev.NewRole == RoleType.Scp106)
			{
				plugin.Coroutines.Add(Timing.CallDelayed(0.5f, () =>
				{
					if (ev.Player.Role == RoleType.Scp106)
					{
						ev.Player.Broadcast(10, plugin.Translation.WelcomeBroadcast);
						ev.Player.SendConsoleMessage(plugin.Translation.ConsoleInfo, "white");
					}
				}));
			}
		}

		public void OnCreatePortal(CreatingPortalEventArgs ev) 
		{
			var list = ListPool<Player>.Shared.Rent(Player.List);
			int playerCount = list.Count;
			if (plugin.Config.Preferences.MinimumPlayers > playerCount)
			{
				ev.Player.Broadcast(8, plugin.Translation.MinPlayers.Replace("$count", plugin.Config.Preferences.MinimumPlayers.ToString()));
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
				ev.Player.Broadcast(8, plugin.Translation.MinAlive.Replace("$count", plugin.Config.Preferences.MinimumAlivePlayers.ToString()));
				plugin.Methods.StalkyCooldown = 10f;
				return;
			}

			if (plugin.Config.Preferences.MinimumAliveTargets > targetCount)
			{
				ev.Player.Broadcast(8, plugin.Translation.MinTargetsAlive.Replace("$count", plugin.Config.Preferences.MinimumAliveTargets.ToString()));
				plugin.Methods.StalkyCooldown = 10f;
				return;
			}

			ListPool<Player>.Shared.Return(list);
			ev.IsAllowed = plugin.Methods.Stalk(ev.Player);
		}

		public void OnRestartingRound()
        {
			Timing.KillCoroutines(plugin.Coroutines.ToArray());
			plugin.Coroutines.Clear();
		}
	}
}

