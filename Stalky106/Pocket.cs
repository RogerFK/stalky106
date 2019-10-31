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
	internal class PocketHandler : IEventHandlerRoundStart, IEventHandlerCallCommand, IEventHandlerSetRole, IEventHandlerPocketDimensionDie,
        IEventHandlerPocketDimensionEnter, IEventHandlerPlayerHurt
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

		public PocketHandler(Stalky106 plugin)
		{
			this.plugin = plugin;
		}
		private float currentCd;

		private bool IsInPocketDimension(Vector position) => Vector.Distance(position, pocketDimension) < 50f;
		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			if (ev.Command.StartsWith("pocket"))
			{
				if (!plugin.pocket || !plugin.enable)
				{
					ev.ReturnMessage = "The pocket command is not enabled in this server!";
					return;
				}

				if(ev.Player.TeamRole.Role != Role.SCP_106)
				{
					ev.ReturnMessage = plugin.notscp106;
					return;
				}

				if (IsInPocketDimension(ev.Player.GetPosition()))
				{
					ev.ReturnMessage = plugin.alreadyInPocket;
					ev.Player.PersonalBroadcast(3, plugin.alreadyInPocket, false);
					return;
				}
				ev.Player.PersonalBroadcast(5, plugin.gettingOut, false);
                lastPos = ev.Player.GetPosition();
				ev.Player.Teleport(pocketDimension);
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
	}
}

