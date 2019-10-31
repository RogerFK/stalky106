using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.Piping;
using UnityEngine;
using ServerMod2.API;
using System;

namespace stalky106
{
	internal class EventHandlers : IEventHandlerRoundStart, IEventHandlerCallCommand, IEventHandlerSetRole, IEventHandlerPocketDimensionDie,
        IEventHandlerPocketDimensionEnter, IEventHandlerPlayerHurt, IEventHandler106CreatePortal
	{
		// It will ALWAYS ignore spectators and unconnected players.
		private readonly int[] alwaysIgnore = new int[] { -1, 5, 7 };
		public readonly string[] defaultRoleNames = new string[] { "0:SCP-173", "1:Class D", "3:SCP-106", "4:NTF Scientist", "5:SCP-049", "6:Scientist",
			"8:Chaos Insurgent", "9:SCP-096", "10:Zombie","11:NTF Lieutenant", "12:NTF Commander", "13:NTF Cadet", "14:Tutorial", "15:Facility Guard",
			"16:SCP-939-53", "17:SCP-939-89" };
		private readonly Stalky106 plugin;
		private static Vector lastPos;

		/// Pocket dimension location (constant) ///
		public readonly static Vector pocketDimension = Vector.Down * 1997f;

		public EventHandlers(Stalky106 plugin)
		{
			this.plugin = plugin;
		}
		private float currentCd;
		
		private bool IsInPocketDimension(Vector position) => Vector.Distance(position, pocketDimension) < 50f;
		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			if (ev.Command.StartsWith("stalk"))
			{
				if (!plugin.enable)
				{
					return;
				}

				if (!plugin.stalk)
				{
					ev.ReturnMessage = "Stalk is not enabled in this server!";
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
					if(!(ev.Player.GetGameObject() as GameObject).GetComponent<FallDamage>().isGrounded)
					{
						ev.ReturnMessage = plugin.onGround;
						return;
					}
					if (auxScp106Component != null)
					{
						List<Player> possibleTargets = PluginManager.Manager.Server.GetPlayers()
							.Where(p => !plugin.ignoreRoles.Contains((int)p.TeamRole.Role) && !plugin.ignoreTeams.Contains((int)p.TeamRole.Team) && !alwaysIgnore.Contains((int)p.TeamRole.Team)).ToList();
						if (possibleTargets.Count < 1)
						{
							ev.ReturnMessage = plugin.noTargetsLeft;
							ev.Player.PersonalClearBroadcasts();
							ev.Player.PersonalBroadcast(3, plugin.noTargetsLeft, false);
							return;
						}
						RaycastHit raycastHit;
						Player victim;
						do
						{
							int rng = UnityEngine.Random.Range(0, possibleTargets.Count);
							victim = possibleTargets.ElementAt(rng);
							Physics.Raycast(new Ray(victim.GetPosition().ToVector3(), -Vector3.up), out raycastHit, 10f, auxScp106Component.teleportPlacementMask);
							if (Vector.Distance(victim.GetPosition(), new Vector(0, -1998, 0)) < 30f)
							{
								victim = null;
							}
							possibleTargets.RemoveAt(rng);
						} while ((raycastHit.point.Equals(Vector3.zero) && possibleTargets.Count > 0));
						if(victim == null)
						{
							ev.ReturnMessage = plugin.noTargetsLeft;
							ev.Player.PersonalClearBroadcasts();
							ev.Player.PersonalBroadcast(3, plugin.noTargetsLeft, false);
							return;
						}
                        Methods.MovePortal(auxScp106Component, raycastHit.point - Vector3.up, false);
						currentCd = PluginManager.Manager.Server.Round.Duration + plugin.cooldown;
						if (plugin.announceReady) Methods.AnnounceCooldown(plugin.cooldown);
						if(plugin.parsedRoleDict.ContainsKey((int)victim.TeamRole.Role))
						{
							ev.ReturnMessage = Stalky106.Instance.newStalkMessage.Replace("$player", victim.Name).Replace("$class", plugin.parsedRoleDict[(int)victim.TeamRole.Role]).Replace("$cd", plugin.cooldown.ToString());
						}
						else
						{
							ev.ReturnMessage = Stalky106.Instance.newStalkMessage.Replace("$player", victim.Name).Replace("$class", defaultRoleNames[(int)victim.TeamRole.Role]).Replace("$cd", plugin.cooldown.ToString());
						}
						ev.Player.PersonalClearBroadcasts();
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
			if(plugin.announceReady)
			{
				AnnounceCooldown(plugin.initialCooldown);
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if(ev.Role == Role.SCP_106)
			{
				MEC.Timing.RunCoroutine(DelayBroadcasts(ev.Player), 1);
			}
		}
		private IEnumerator<float> DelayBroadcasts(Player player)
		{
			yield return MEC.Timing.WaitForSeconds(0.2f);
			if (player.TeamRole.Role == Role.SCP_106)
			{
				if (plugin.stalk) { player.PersonalBroadcast(7, plugin.firstBroadcast, false); player.SendConsoleMessage(plugin.consoleInfo, "white"); }
				if (plugin.pocket) { player.PersonalBroadcast(7, plugin.secondBroadcast, false); player.SendConsoleMessage(plugin.consolePocket, "white"); }
			}
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (ev.Player.TeamRole.Role == Role.SCP_106)
			{
				ev.Die = false;
				if (lastPos != null)
				{
					ev.Player.Teleport(lastPos);
					lastPos = null;
				}
				else
				{
					Scp106PlayerScript component = ((GameObject)ev.Player.GetGameObject()).GetComponent<Scp106PlayerScript>();
					if (component != null)
					{
						Vector3 portalPosition = component.portalPosition;
						if (portalPosition != null)
						{
							if (!portalPosition.Equals(Vector3.zero))
							{
								ev.Player.Teleport(new Vector(portalPosition.x, portalPosition.y, portalPosition.z));
							}
						}
					}
					else
					{
						ev.Player.Teleport(PluginManager.Manager.Server.Map.GetRandomSpawnPoint(Role.SCP_106));
					}
				}
			}
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if(IsInPocketDimension(ev.Attacker.GetPosition()))
			{
				ev.TargetPosition = ev.LastPosition;
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (IsInPocketDimension(ev.Attacker.GetPosition()) && ev.Attacker.TeamRole.Role == Role.SCP_106)
			{
				if(!plugin.pocketDamage) ev.Damage = 0;
			}
		}

        public void On106CreatePortal(Player106CreatePortalEvent ev)
        {
            throw new NotImplementedException();
        }
    }
}

