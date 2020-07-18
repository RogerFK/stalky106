using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace Stalky106
{
	public class StalkyMethods
	{
		// Times
		public float disableFor;
		public float stalky106LastTime;
		private float stalkyCd;

		public float StalkyCooldown
		{
			set
			{
				stalkyCd = Time.time + value;
				if (plugin.Config.Preferences.AnnounceReady)
				{
					plugin.coroutines.Add(MEC.Timing.RunCoroutine(this.AnnounceGlobalCooldown(value), MEC.Segment.Update));
				}
			}
			get => stalkyCd - Time.time;
		}

		public readonly string[] defaultRoleNames = new string[]
		  { "<color=#F00>SCP-173</color>", "<color=#FF8E00>Class D</color>", "Spectator",
			"<color=#F00>SCP-106</color>", "<color=#0096FF>NTF Scientist</color>", "<color=#F00>SCP-049</color>",
			"<color=#FFFF7CFF>Scientist</color>", "SCP-079", "<color=#008f1e>Chaos Insurgent</color>",
			"<color=#f00>SCP-096</color>", "<color=#f00>Zombie</color>",
			"<color=#0096FF>NTF Lieutenant</color>", "<color=#0096FF>NTF Commander</color>",
			"<color=#0096FF>NTF Cadet</color>",
			"Tutorial", "<color=#59636f>Facility Guard</color>",
			"<color=#f00>SCP-939-53</color>", "<color=#f00>SCP-939-89</color>" };

		// It will ALWAYS ignore spectators and unconnected players.
		private static readonly RoleType[] alwaysIgnore = new RoleType[] { RoleType.None, RoleType.Spectator, RoleType.Scp079 };
		private readonly StalkyPlugin plugin;
		public StalkyMethods(StalkyPlugin plugin)
		{
			this.plugin = plugin;
		}
		internal bool Stalk(Player player)
		{
			// If Stalky is disabled by force, don't even create a portal for the guy
			// Avoids 1-frame trick to (probably unintentionally) "cancel" the Stalk.
			if (disableFor > Time.time)
			{
				return false;
			}

			float timeDifference = Time.time - stalky106LastTime;
			float cdAux = StalkyCooldown;
			if (timeDifference > 6f)
			{
				stalky106LastTime = Time.time;
				if (cdAux < 0)
				{
					player.ClearBroadcasts();
					player.Broadcast(6, plugin.Config.Translations.DoubleClick);
				}
				return true;
			}
			else
			{
				player.ClearBroadcasts();
				if (cdAux > 0)
				{
					stalky106LastTime = Time.time;
					int i = 0;
					for (; i < 5 && cdAux > i; i++) player.Broadcast(1, plugin.Config.Translations.Cooldown_Message.Replace("$time", (cdAux - i).ToString("00")));
					disableFor = Time.time + i + 1;
					return true;
				}
				disableFor = Time.time + 4;
				plugin.coroutines.Add(MEC.Timing.RunCoroutine(this.StalkCoroutine(player), MEC.Segment.Update));
				return false;
			}
		}

		public IEnumerator<float> StalkCoroutine(Player player)
		{
			List<Player> list = new List<Player>();
			Scp106PlayerScript scp106Script = player.GameObject.GetComponent<Scp106PlayerScript>();

			// We can't (or shouldn't) do it in one iteration, as we "need" to pick a random one.
			// If we did do it in one iteration, we would have to "go back" at some point,
			// meaning we aren't really saving any CPU time at all.

			foreach (Player plausibleTarget in Player.List)
			{
				if (!alwaysIgnore.Contains(plausibleTarget.Role)
					&& !plugin.Config.Preferences.IgnoreRoles.Contains(plausibleTarget.Role) // noooo my nanoseconds
					&& !plugin.Config.Preferences.IgnoreTeams.Contains(plausibleTarget.Team))
				{
					list.Add(plausibleTarget);
				}
			}
			if (list.IsEmpty())
			{
				player.Broadcast(4, plugin.Config.Translations.NoTargetsLeft);
				yield break;
			}

			// Wait one frame after computing the players
			yield return MEC.Timing.WaitForOneFrame;

			Player target = this.FindTarget(list, scp106Script.teleportPlacementMask, out Vector3 portalPosition);

			if (target == null || (Vector3.Distance(portalPosition, StalkyPlugin.pocketDimension) < 40f))
			{
				player.Broadcast(4, plugin.Config.Translations.NoTargetsLeft);
				yield break;
			}
			if (portalPosition.Equals(Vector3.zero))
			{
				player.Broadcast(4, plugin.Config.Translations.Error);
				yield break;
			}

			// Wait another frame after the while loops that goes over players.
			// Only useful for +100 player servers and the potatest server in this case, 
			// but it might help with poorly implemented logging systems. And bruh it's 2 frames.
			yield return MEC.Timing.WaitForOneFrame;

			plugin.coroutines.Add(MEC.Timing.RunCoroutine(this.PortalProcedure(scp106Script, portalPosition - Vector3.up), MEC.Segment.Update));

			StalkyCooldown = plugin.Config.Preferences.Cooldown;
			stalky106LastTime = Time.time;
			disableFor = Time.time + 10f;
			if (!plugin.Config.Translations.RoleDisplayNames.TryGetValue(target.Role, out string className))
			{
				className = defaultRoleNames[(int)target.Role];
			}

			player.Broadcast(6, plugin.Config.Translations.StalkMessage
								.Replace("$player", target.Nickname).Replace("$class", className)
								.Replace("$cd", plugin.Config.Preferences.Cooldown.ToString()));
		}

		private Player FindTarget(List<Player> validPlayerList, LayerMask teleportPlacementMask, out Vector3 portalPosition)
		{
			Player target = null;
			stalky106LastTime = Time.time;

			do
			{
				int index = UnityEngine.Random.Range(0, validPlayerList.Count);
				target = validPlayerList[index];
				Physics.Raycast(new Ray(target.GameObject.transform.position, -Vector3.up), out RaycastHit raycastHit, 10f, teleportPlacementMask);

				// If the raycast fails, the point will be (0, 0, 0), basically Vector3.zero
				portalPosition = raycastHit.point;
				validPlayerList.RemoveAt(index);
			} while ((portalPosition.Equals(Vector3.zero) || Vector3.Distance(portalPosition, StalkyPlugin.pocketDimension) < 40f) && validPlayerList.Count > 0);

			return target;
		}

		public IEnumerator<float> PortalProcedure(Scp106PlayerScript script, Vector3 pos)
		{
			script.NetworkportalPosition = pos;
			if (plugin.Config.Preferences.AutoTp)
			{
				yield return MEC.Timing.WaitForSeconds(plugin.Config.Preferences.AutoDelay);
				// Prevents you from avoiding the auto-tp by jumping.
				// Bug: frame-perfect jumps will move SCP-106 on the server, but the client
				// will be able to move. Hence, why the config: force_auto_tp
				do
				{
					script.CallCmdUsePortal();
					yield return MEC.Timing.WaitForOneFrame;
				}
				while (!script.goingViaThePortal && plugin.Config.Preferences.ForceAutoTp);
			}
		}
		private IEnumerator<float> AnnounceGlobalCooldown(float duration)
		{
			yield return MEC.Timing.WaitForSeconds(duration);

			if (!plugin.Config.IsEnabled) yield break;

			foreach (Player player in Player.List)
			{
				if (player.Role == RoleType.Scp106)
				{
					player.Broadcast(6, plugin.Config.Translations.StalkReady);
				}
			}
		}
	}
}
