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
    internal class StalkyEvents : IEventHandlerRoundStart, IEventHandlerSetRole, IEventHandler106CreatePortal
    {
        // It will ALWAYS ignore spectators and unconnected players.
        private readonly int[] alwaysIgnore = new int[] { -1, 5, 7 };
        public readonly string[] defaultRoleNames = new string[] { "0:SCP-173", "1:Class D", "3:SCP-106", "4:NTF Scientist", "5:SCP-049", "6:Scientist",
            "8:Chaos Insurgent", "9:SCP-096", "10:Zombie","11:NTF Lieutenant", "12:NTF Commander", "13:NTF Cadet", "14:Tutorial", "15:Facility Guard",
            "16:SCP-939-53", "17:SCP-939-89" };
        private readonly Stalky106 plugin;

        /// Pocket dimension location (constant) ///
        public readonly static Vector pocketDimension = Vector.Down * 1997f;

        public StalkyEvents(Stalky106 plugin)
        {
            this.plugin = plugin;
        }
        private float currentCd;

        public void OnRoundStart(RoundStartEvent ev)
        {
            currentCd = plugin.initialCooldown;
            if (plugin.announceReady)
            {
                Methods.AnnounceCooldown(plugin.initialCooldown);
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if (ev.Role == Role.SCP_106)
            {
                MEC.Timing.RunCoroutine(Methods.DelayBroadcasts(ev.Player), 1);
            }
        }

        private long triggerTick = 0;

        public void On106CreatePortal(Player106CreatePortalEvent ev)
        {
            if (!plugin.enable || !plugin.stalk || ev.Player.GetGameObject().Equals(PlayerManager.localPlayer))
            {
                return;
            }
            // https://tickstodatetime.azurewebsites.net/ taken as reference for ticks.
            // This basically compares the current time to the tick the guy triggered the portal creation tool,
            // and if the difference between his last portal creation (triggerTick) and the current one (DateTime.Now.Ticks)
            // is bigger than the seconds that you set the threshold to, then prompt him and refresh the latest portal creation tick
            long timeDifference = DateTime.Now.Ticks - triggerTick;

            if (timeDifference < 2000000) { return; }
            ev.Player.PersonalClearBroadcasts();
            int cdAux = (int)((currentCd > 0 ? currentCd : 1) - PluginManager.Manager.Server.Round.Duration);
            if (timeDifference > 10000000 * plugin.threshold)
            {
                triggerTick = DateTime.Now.Ticks;
                if(cdAux > 0) ev.Player.PersonalBroadcast((uint)plugin.threshold, plugin.doubleClick.Replace("\n", Environment.NewLine), false);
            }
            else
            {
                if (cdAux > 0)
                {
                    triggerTick = DateTime.Now.AddSeconds(Math.Min(5, Math.Max(cdAux - 0.5, 0))).Ticks;
                    for (int i = 0; i < 5 && cdAux > i; i++) ev.Player.PersonalBroadcast(1, plugin.cooldownmsg.Replace("$time", (cdAux - i).ToString()), false);
                    return;
                }
                triggerTick = DateTime.Now.AddSeconds(10).Ticks;
                MEC.Timing.RunCoroutine(StalkCoroutine(ev), 0);
            }
        }
        private IEnumerator<float> StalkCoroutine(Player106CreatePortalEvent ev)
        {
            yield return MEC.Timing.WaitForOneFrame;
            Scp106PlayerScript auxScp106Component = (ev.Player.GetGameObject() as GameObject).GetComponent<Scp106PlayerScript>();
            if (!(ev.Player.GetGameObject() as GameObject).GetComponent<FallDamage>().isGrounded)
            {
                ev.Player.PersonalBroadcast(3, plugin.onGround, false);
                yield break;
            }
            if (auxScp106Component != null)
            {
                List<Player> possibleTargets = PluginManager.Manager.Server.GetPlayers(p => !plugin.ignoreRoles.Contains((int)p.TeamRole.Role) && !plugin.ignoreTeams.Contains((int)p.TeamRole.Team) && !alwaysIgnore.Contains((int)p.TeamRole.Team));
                if (possibleTargets.Count < 1)
                {
                    ev.Player.PersonalBroadcast(3, plugin.noTargetsLeft, false);
                    yield break;
                }
                RaycastHit raycastHit;
                Player victim;
                int rng;
                do
                {
                    rng = UnityEngine.Random.Range(0, possibleTargets.Count);
                    victim = possibleTargets[rng];
                    Physics.Raycast(new Ray(victim.GetPosition().ToVector3(), -Vector3.up), out raycastHit, 10f, auxScp106Component.teleportPlacementMask);
                    // If the victim is in the pocket dimension
                    if (Vector.Distance(victim.GetPosition(), new Vector(0, -1998, 0)) < 40f)
                    {
                        victim = null;
                    }
                    possibleTargets.RemoveAt(rng);
                } while ((raycastHit.point.Equals(Vector3.zero) && possibleTargets.Count > 0));
                if (victim == null)
                {
                    ev.Player.PersonalBroadcast(3, plugin.noTargetsLeft, false);
                    yield break;
                }
                if (raycastHit.point.Equals(Vector3.zero))
                {
                    ev.Player.PersonalBroadcast(4, plugin.error, false);
                    yield break;
                }
                Methods.MovePortal(auxScp106Component, raycastHit.point - Vector3.up, false);
                currentCd = PluginManager.Manager.Server.Round.Duration + plugin.cooldown;
                if (plugin.announceReady) Methods.AnnounceCooldown(plugin.cooldown);
                string bcMessage;
                if (plugin.parsedRoleDict.ContainsKey((int)victim.TeamRole.Role))
                {
                    bcMessage = Stalky106.Instance.newStalkMessage.Replace("$player", victim.Name).Replace("$class", plugin.parsedRoleDict[(int)victim.TeamRole.Role]).Replace("$cd", plugin.cooldown.ToString());
                }
                else
                {
                    bcMessage = Stalky106.Instance.newStalkMessage.Replace("$player", victim.Name).Replace("$class", defaultRoleNames[(int)victim.TeamRole.Role]).Replace("$cd", plugin.cooldown.ToString());
                }
                ev.Player.PersonalBroadcast(5, bcMessage, false);
            }
            yield break;
        }
    }
}