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
	internal class PocketHandler : IEventHandlerCallCommand, IEventHandlerPocketDimensionDie,
        IEventHandlerPocketDimensionEnter, IEventHandlerPlayerHurt
	{
		private readonly Stalky106 plugin;
		private static Vector lastPos;

		/// Pocket dimension location (constant) ///
		public readonly static Vector pocketDimension = Vector.Down * 1997f;

		public PocketHandler(Stalky106 plugin)
		{
			this.plugin = plugin;
		}

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

				if (ev.Player.InPocketDimension())
				{
					ev.ReturnMessage = plugin.alreadyInPocket;
					ev.Player.PersonalBroadcast(3, plugin.alreadyInPocket, false);
					return;
				}
				ev.Player.PersonalBroadcast(5, plugin.gettingOut, false);
                lastPos = ev.Player.GetPosition();
                plugin.Info($"{lastPos.x},{lastPos.y},{lastPos.z}");
                Scp106PlayerScript s106cmp = ((GameObject)ev.Player.GetGameObject()).GetComponent<Scp106PlayerScript>();
                RaycastHit raycastHit;
                Physics.Raycast(new Ray(pocketDimension.ToVector3(), -Vector3.up), out raycastHit, 50f, s106cmp.teleportPlacementMask);
                Methods.MovePortal(s106cmp, raycastHit.point - Vector3.up, true);
                ev.ReturnMessage = "I'm on me moms car, broom broom";
            }
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (ev.Player.TeamRole.Role == Role.SCP_106)
			{
				ev.Die = false;
                Scp106PlayerScript s106cmp = ((GameObject)ev.Player.GetGameObject()).GetComponent<Scp106PlayerScript>();

                if (lastPos != null && !lastPos.Equals(Vector.Zero) && s106cmp != null)
				{
                    // Move the player back
                    ev.Player.Teleport(pocketDimension);

                    // Spawn a portal in his last position and TP him to that
                    RaycastHit raycastHit;
                    plugin.Info($"ahora {lastPos.x},{lastPos.y},{lastPos.z}");
                    Physics.Raycast(new Ray(lastPos.ToVector3(), -Vector3.up), out raycastHit, 50f, s106cmp.teleportPlacementMask);
                    plugin.Info($" raycas {raycastHit.point.x},{raycastHit.point.y},{raycastHit.point.z}");
                    Methods.MovePortal(s106cmp, raycastHit.point - Vector3.up, true);
				}
				else
				{
					if (s106cmp != null)
					{
						Vector3 portalPosition = s106cmp.portalPosition;
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
			if(ev.Attacker.InPocketDimension())
			{
				ev.TargetPosition = ev.LastPosition;
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (ev.Attacker.InPocketDimension() && ev.Attacker.TeamRole.Role == Role.SCP_106)
			{
				if(!plugin.pocketDamage) ev.Damage = 0;
			}
		}
    }
}

