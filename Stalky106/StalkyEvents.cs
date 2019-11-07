using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;
using ServerMod2.API;
using System;

namespace stalky106
{
    internal class StalkyEvents : IEventHandlerRoundStart, IEventHandlerSetRole, IEventHandler106CreatePortal
    {
        // It will ALWAYS ignore spectators and unconnected players.
        private readonly int[] alwaysIgnore = new int[] { -1, 5, 7 };
        public readonly string[] defaultRoleNames = new string[]
        {   "0:<color=#F00>SCP-173</color>", "1:<color=#FF8E00>Class D</color>",
            "3:<color=#F00>SCP-106</color>", "4:<color=#0096FF>NTF Scientist</color>", "5:<color=#F00>SCP-049</color>",
            "6:<color=#FFFF7CFF>Scientist</color>", "8:<color=#008f1e>Chaos Insurgent</color>",
            "9:<color=#f00>SCP-096</color>", "10:<color=#f00>Zombie</color>",
            "11:<color=#0096FF>NTF Lieutenant</color>", "12:<color=#0096FF>NTF Commander</color>", "13:<color=#0096FF>NTF Cadet</color>",
            "14:Tutorial", "15:<color=#59636f>Facility Guard</color>",
            "16:<color=#f00>SCP-939-53</color>", "17:<color=#f00>SCP-939-89</color>" };
        private readonly Stalky106 plugin;

        /// Pocket dimension location (constant) ///
        public readonly static Vector pocketDimension = Vector.Down * 1997f;

        public StalkyEvents(Stalky106 plugin)
        {
            this.plugin = plugin;
        }
        private int currentCd;

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
        private long disableFor = 0;

        public void On106CreatePortal(Player106CreatePortalEvent ev)
        {
            if (!plugin.enable || !plugin.stalk || ev.Player.GetGameObject().Equals(PlayerManager.localPlayer))
            {
                return;
            }
            // https://tickstodatetime.azurewebsites.net/ taken as reference for ticks.
            // This basically compares the current time to the tick the guy triggered the portal creation tool,
            // and if the difference between his last portal creation (triggerTick) and the current one (DateTime.Now.Ticks)
            // is bigger than the seconds that you set the threshold to, then prompt him and refresh the latest portal creation tick.
            // Same goes for the "disableFor" variable, which avoids massive lag and spamming the guy/console with tons of broadcasts
            
            if (disableFor > DateTime.Now.Ticks) return;
            
            long timeDifference = DateTime.Now.Ticks - triggerTick;

            if (timeDifference < 2000000) return;

            int cdAux = currentCd - PluginManager.Manager.Server.Round.Duration;
            if (timeDifference > 10000000 * plugin.threshold)
            {
                triggerTick = DateTime.Now.Ticks;
                if (cdAux < 0)
                {
                    ev.Player.PersonalClearBroadcasts();
                    ev.Player.PersonalBroadcast((uint)plugin.threshold, plugin.doubleClick.Replace("\n", Environment.NewLine), false);
                }
            }
            else
            {
                ev.Player.PersonalClearBroadcasts();
                if (cdAux > 0)
                {
                    triggerTick = DateTime.Now.AddSeconds(Math.Min(5, Math.Max(cdAux - 0.5, 0))).Ticks;
                    int i = 0;
                    for (; i < 5 && cdAux > i; i++) ev.Player.PersonalBroadcast(1, plugin.cooldownmsg.Replace("$time", (cdAux - i).ToString()), false);
                    disableFor = DateTime.Now.AddSeconds(i + 1).Ticks;
                    return;
                }
                MEC.Timing.RunCoroutine(StalkCoroutine(ev), 0);
            }
        }
        private IEnumerator<float> StalkCoroutine(Player106CreatePortalEvent ev)
        {
            yield return MEC.Timing.WaitForOneFrame;
            Scp106PlayerScript auxScp106Component = (ev.Player.GetGameObject() as GameObject).GetComponent<Scp106PlayerScript>();
            if (auxScp106Component != null)
            {
                List<Player> possibleTargets = PluginManager.Manager.Server.GetPlayers(p => !plugin.ignoreRoles.Contains((int)p.TeamRole.Role) && !plugin.ignoreTeams.Contains((int)p.TeamRole.Team) && !alwaysIgnore.Contains((int)p.TeamRole.Team));
                if (possibleTargets.Count < 1)
                {
                    ev.Player.PersonalBroadcast(3, plugin.noTargetsLeft, false);
                    yield break;
                }
                RaycastHit raycastHit;
                Player victim = null;
                int rng;
                // Before any "error" that might ocurr, register the current time so there's no "fake" cooldown
                triggerTick = DateTime.Now.Ticks;
                do
                {
                    rng = UnityEngine.Random.Range(0, possibleTargets.Count);
                    victim = possibleTargets[rng];
                    Physics.Raycast(new Ray(victim.GetPosition().ToVector3(), -Vector3.up), out raycastHit, 10f, auxScp106Component.teleportPlacementMask);
                    // If the victim is in the pocket dimension
                    if (Vector.Distance(victim.GetPosition(), new Vector(0, -1998, 0)) < 40f)
                    {
                        victim = null;
                        raycastHit.point = Vector3.zero;
                    }
                    possibleTargets.RemoveAt(rng);
                } while (raycastHit.point.Equals(Vector3.zero) && possibleTargets.Count > 0);
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
                triggerTick = DateTime.Now.AddSeconds(10).Ticks;
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