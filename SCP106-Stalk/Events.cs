using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;
using MEC;
using ServerMod2.API;

namespace stalky106
{
	internal class EventHandlers : IEventHandlerRoundStart, IEventHandlerCallCommand, IEventHandlerSetRole//, IEventHandlerPlayerHurt
	{
		// It will ALWAYS ignores spectators and unconnected players.
		private readonly int[] alwaysIgnore = new int[] { -1, 5, 7 };
		public readonly string[] defaultRoleNames = new string[] { "0:SCP-173", "1:Class D", "3:SCP-106", "4:NTF Scientist", "5:SCP-049", "6:Scientist",
			"8:Chaos Insurgent","9:SCP-096", "10:Zombie","11:NTF Lieutenant", "12:NTF Commander", "13:NTF Cadet", "14:Tutorial", "15:Facility Guard",
			"16:SCP-939-53", "17:SCP-939-89" };
		private readonly Stalky106 plugin;
		public EventHandlers(Stalky106 plugin)
		{
			this.plugin = plugin;
		}
		private float currentCd;

		private IEnumerator<float> PortalAnimation(Scp106PlayerScript auxScp106Component)
		{
			Animator anim = auxScp106Component.portalPrefab.GetComponent<Animator>();
			// I don't know what this does but imma include it so it works flawlessly
			anim.SetBool("activated", false);
			yield return 1f;
			auxScp106Component.portalPrefab.transform.position = auxScp106Component.portalPosition;
			anim.SetBool("activated", true);

			if (plugin.autoTp) auxScp106Component.CallCmdUsePortal();
		}
		private void MovePortalThingy(Scp106PlayerScript auxScp106Component, Vector3 pos)
		{
			auxScp106Component.NetworkportalPosition = pos;
			Timing.RunCoroutine(PortalAnimation(auxScp106Component));
		}
		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			if (ev.Command.StartsWith("stalk"))
			{
				if (!plugin.enable)
				{
					return;
				}
				if(ev.Player.TeamRole.Role == Role.SCP_106)
				{
					int cdAux = (int) currentCd - PluginManager.Manager.Server.Round.Duration;
					if (cdAux > 0)
					{
						ev.ReturnMessage = plugin.cooldownmsg.Replace("$time", cdAux.ToString());
						return;
					}
					Scp106PlayerScript auxScp106Component = (ev.Player.GetGameObject() as GameObject).GetComponent<Scp106PlayerScript>();
					if (auxScp106Component != null)
					{
						IEnumerable<Player> possibleTargets = PluginManager.Manager.Server.GetPlayers()
							.Where(p => !plugin.ignoreRoles.Contains((int)p.TeamRole.Role) && !plugin.ignoreTeams.Contains((int)p.TeamRole.Team) && !alwaysIgnore.Contains((int)p.TeamRole.Team));
						if (possibleTargets.Count() < 1)
						{
							ev.ReturnMessage = plugin.notargetsleft;
							return;
						}
						RaycastHit raycastHit;
						Player victim;
						do
						{
							victim = possibleTargets.ElementAt(Random.Range(0, possibleTargets.Count()));
							Physics.Raycast(new Ray(victim.GetPosition().ToVector3(), -Vector3.up), out raycastHit, 10f, auxScp106Component.teleportPlacementMask);
						} while (raycastHit.point.Equals(Vector3.zero));
						MovePortalThingy(auxScp106Component, raycastHit.point - Vector3.up);
						currentCd = PluginManager.Manager.Server.Round.Duration + plugin.cooldown;
						if (plugin.announceReady) Timing.RunCoroutine(AnnounceCooldown(plugin.cooldown));
						if(plugin.parsedRoleDict.ContainsKey((int)victim.TeamRole.Role))
						{
							ev.ReturnMessage = plugin.hauntmessage.Replace("$player", victim.Name).Replace("$class", plugin.parsedRoleDict[(int)victim.TeamRole.Role]);
						}
						else
						{
							ev.ReturnMessage = plugin.hauntmessage.Replace("$player", victim.Name).Replace("$class", defaultRoleNames[(int)victim.TeamRole.Role]);
						}
						ev.Player.PersonalBroadcast(5, ev.ReturnMessage, false);
					}
					else
					{
						ev.ReturnMessage = plugin.error;
					}
				}
				else
				{
					ev.ReturnMessage = plugin.notscp106;
				}
			}
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			currentCd = plugin.initialCooldown;
			if(plugin.announceReady) Timing.RunCoroutine(AnnounceCooldown(plugin.initialCooldown));
		}

		private IEnumerator<float> AnnounceCooldown(float cd)
		{
			plugin.Info("Put .stalk on cooldown for " + cd + " seconds.");
			yield return cd;
			plugin.Info("Cooldown for .stalk ended");
			foreach (Player larry in PluginManager.Manager.Server.GetPlayers(Role.SCP_106))
			{
				larry.PersonalBroadcast(8, plugin.stalkready, false);
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if(ev.Role == Role.SCP_106)
			{
				ev.Player.PersonalBroadcast(8, plugin.firstbroadcast, false);
				ev.Player.SendConsoleMessage(plugin.consoleinfo, "white");
			}
		}

		/*
		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			Scp106PlayerScript auxScp106Component = (ev.Player.GetGameObject() as GameObject).GetComponent<Scp106PlayerScript>();
			if (auxScp106Component.goingViaThePortal && !plugin.attackFromPortal) // attack from portal was a bool in case u're dumbo
			{
				ev.Damage = 0;
				Timing.RunCoroutine(TpBack(ev.Player, ev.Player.GetPosition()));
			}
		}
		private IEnumerator<float> TpBack(Player player, Vector pos)
		{
			yield return 0.01f;
			player.Teleport(pos);
		}
		*/
	}
}

