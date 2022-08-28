namespace Stalky106
{
	using System;
	using System.Collections.Generic;
	using Exiled.API.Features;
	using MEC;
	using UnityEngine;
	using Random = UnityEngine.Random;
	using System.Linq;
	using Exiled.API.Features.Roles;
	using Extensions;
    using Exiled.CustomRoles.API;

    public class Methods
	{
		private readonly StalkyPlugin plugin;
		public Methods(StalkyPlugin plugin)
		{
			this.plugin = plugin;
		}

		private float disableFor;
		private float stalky106LastTime;
		private float stalkyAvailable;
		public readonly string[] defaultRoleNames =
		{
			"<color=#F00>SCP-173</color>", "<color=#FF8E00>Class D</color>", "Spectator",
			"<color=#F00>SCP-106</color>", "<color=#0096FF>NTF Scientist</color>", "<color=#F00>SCP-049</color>",
			"<color=#FFFF7CFF>Scientist</color>", "SCP-079", "<color=#008f1e>Chaos Insurgent</color>",
			"<color=#f00>SCP-096</color>", "<color=#f00>Zombie</color>",
			"<color=#0096FF>NTF Lieutenant</color>", "<color=#0096FF>NTF Commander</color>",
			"<color=#0096FF>NTF Cadet</color>",
			"Tutorial", "<color=#59636f>Facility Guard</color>",
			"<color=#f00>SCP-939-53</color>", "<color=#f00>SCP-939-89</color>"
		};

		private static readonly RoleType[] alwaysIgnore = { RoleType.None, RoleType.Spectator, RoleType.Scp079 };
		private static readonly Vector3 pocketDimension = new Vector3(0f, -1998f, 0f);

		public float StalkyCooldown
		{
			get => stalkyAvailable - Time.time;
			set
			{
				stalkyAvailable = Time.time + value;
				if (plugin.Config.Preferences.AnnounceReady)
					plugin.Coroutines.Add(Timing.CallDelayed(value, () => plugin.Coroutines.Add(Timing.RunCoroutine(AnnounceGlobalCooldown()))));
			}
		}

		public bool Stalk(Player player)
		{
			if (disableFor > Time.time)
				return false;

			float timeDifference = Time.time - stalky106LastTime;
			float cdAux = StalkyCooldown;
			if (timeDifference > 6f)
			{
				stalky106LastTime = Time.time;
				if (cdAux < 0)
				{
					player.ClearBroadcasts();
					player.Broadcast(6, plugin.Translation.DoubleClick);
				}

				return true;
			}

			player.ClearBroadcasts();
			if (cdAux > 0)
			{
				stalky106LastTime = Time.time;
				int i = 0;
				for (; i < 5 && cdAux > i; i++) 
					player.Broadcast(1, plugin.Translation.Cooldown_Message.Replace("$time", (cdAux - i).ToString("00")));
					
				disableFor = Time.time + i + 1;
				return true;
			}

			disableFor = Time.time + 4;
			plugin.Coroutines.Add(Timing.RunCoroutine(StalkCoroutine(player)));
			return false;
		}

		private IEnumerator<float> StalkCoroutine(Player player)
		{
			List<Player> list = new List<Player>();
			if (!player.Role.Is(out Scp106Role scp106Script))
				yield break;

			foreach (Player plausibleTarget in Player.List)
			{
				if (plausibleTarget.GetCustomRoles().Any(x => x.Name is "SCP-035") || plausibleTarget.SessionVariables.ContainsKey("IsNPC"))
                    continue;

				if (!alwaysIgnore.Contains(plausibleTarget.Role)
					&& !plugin.Config.Preferences.IgnoreRoles.Contains(plausibleTarget.Role)
					&& !plugin.Config.Preferences.IgnoreTeams.Contains(plausibleTarget.Role.Team))
				{
					if (plugin.Config.Preferences.SameZoneOnly)
					{
						if (plausibleTarget.CurrentRoom.Zone == player.CurrentRoom.Zone
							|| plausibleTarget.CurrentRoom.Zone is Exiled.API.Enums.ZoneType.Unspecified)
						{
							list.Add(plausibleTarget);
						}
					}
					else
						list.Add(plausibleTarget);
				}
			}

			if (list.IsEmpty())
			{
				player.Broadcast(4, plugin.Translation.NoTargetsLeft);
				yield break;
			}

			yield return Timing.WaitForOneFrame;

			Player target = FindTarget(list, scp106Script.Script.teleportPlacementMask, out Vector3 portalPosition);
			if (target == default || (Vector3.Distance(portalPosition, pocketDimension) < 40f))
			{
				player.Broadcast(4, plugin.Translation.NoTargetsLeft);
				yield break;
			}

			if (portalPosition.Equals(Vector3.zero))
			{
				player.Broadcast(4, plugin.Translation.Error);
				yield break;
			}

			yield return Timing.WaitForOneFrame;

			plugin.Coroutines.Add(Timing.RunCoroutine(PortalProcedure(scp106Script, portalPosition - Vector3.up)));
			StalkyCooldown = plugin.Config.Preferences.Cooldown;
			stalky106LastTime = Time.time;
			disableFor = Time.time + 10f;
			if (!plugin.Translation.RoleDisplayNames.TryGetValue(target.Role, out string className))
				className = defaultRoleNames[(int)target.Role.Type];

			player.Broadcast(6, plugin.Translation.StalkMessage.ReplaceAfterToken('$',
									new Tuple<string, object>[] {
										new Tuple<string, object>("player", target.Nickname),
										new Tuple<string, object>("class", className),
										new Tuple<string, object>("cd", plugin.Config.Preferences.Cooldown)}));
		}

		public IEnumerator<float> AnnounceGlobalCooldown()
		{
			while (stalkyAvailable > Time.time)
				yield return Timing.WaitForSeconds(0.15F);

			if (!plugin.Config.Preferences.AnnounceReady) 
				yield break;

			foreach (Player player in Player.List)
			{
				if (player.Role.Type is RoleType.Scp106)
					player.Broadcast(6, plugin.Translation.StalkReady);
			}
		}

		private Player FindTarget(List<Player> validPlayerList, LayerMask teleportPlacementMask, out Vector3 portalPosition)
		{
			Player target;
			stalky106LastTime = Time.time;

			do
			{
				int index = Random.Range(0, validPlayerList.Count);
				target = validPlayerList[index];
				Physics.Raycast(new Ray(target.Position, -Vector3.up), out RaycastHit raycastHit, 10f, teleportPlacementMask);
				portalPosition = raycastHit.point;
				validPlayerList.RemoveAt(index);
			} 
			while ((portalPosition.Equals(Vector3.zero) || Vector3.Distance(portalPosition, pocketDimension) < 40f) && validPlayerList.Count > 0);

			return target;
		}

		public IEnumerator<float> PortalProcedure(Scp106Role script, Vector3 pos)
		{
			script.PortalPosition = pos;

			// Game Base code waits 0.3 seconds to change the portal pos
			yield return Timing.WaitForSeconds(0.3f);

			if (plugin.Config.Preferences.AutoTp)
			{
				yield return Timing.WaitForSeconds(plugin.Config.Preferences.AutoDelay);

				do
				{
					script.UsePortal();
					yield return Timing.WaitForOneFrame;
				}
				while (!script.Script.goingViaThePortal && plugin.Config.Preferences.ForceAutoTp);
			}
		}
	}
}
