using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace stalky106
{
    public static class StalkyMethods
    {
        public static float disableFor;
        public static float stalky106LastTime;
        private static float stalkyCd;
		public static float StalkyCooldown
        {
            set
            {
                stalkyCd = Time.time + value;
                if (StalkyConfigs.announceReady)
                {
                    Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(StalkyCooldownAnnounce(value), MEC.Segment.Update));
                }
            }
            get
            {
                return stalkyCd - Time.time;
            }
        }
        public static readonly string[] defaultRoleNames = new string[]
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
        public static IEnumerator<float> StalkCoroutine(Scp106PlayerScript script, Broadcast bc)
        {
            List<ReferenceHub> list = new List<ReferenceHub>();
            foreach (ReferenceHub rh in EXILED.Plugin.GetHubs())
            {
                Role role = rh.characterClassManager.Classes.SafeGet(rh.characterClassManager.CurClass);
                if (!alwaysIgnore.Contains(role.roleId)
                    && !StalkyConfigs.ignoreRoles.Contains((int)role.roleId)
                    && !StalkyConfigs.ignoreTeams.Contains((int)EXILED.Plugin.GetTeam(role.roleId)))
                {
                    list.Add(rh);
                }
            }
            if (list.IsEmpty())
            {
                bc.TargetAddElement(script.connectionToClient, StalkyConfigs.noTargetsLeft, 4U, false);
                yield break;
            }

            // Wait one frame after computing the players
            yield return MEC.Timing.WaitForOneFrame;

            stalky106LastTime = Time.time;
            ReferenceHub target;
            Vector3 portalPosition;
            do
            {
                int index = Random.Range(0, list.Count);
                target = list[index];
                Physics.Raycast(new Ray(target.transform.position, -Vector3.up), out RaycastHit raycastHit, 10f, script.teleportPlacementMask);
                // If the raycast isn't succesful, the point will be (0, 0, 0), basically Vector3.zero
                portalPosition = raycastHit.point;
                list.RemoveAt(index);
            }
            while ((portalPosition.Equals(Vector3.zero) || Vector3.Distance(portalPosition, Stalky106.pocketDimension) < 40f) && list.Count > 0);
            if (target == null || (Vector3.Distance(portalPosition, Stalky106.pocketDimension) < 40f))
            {
                bc.TargetAddElement(script.connectionToClient, StalkyConfigs.noTargetsLeft, 4U, false);
                yield break;
            }
            if (portalPosition.Equals(Vector3.zero))
            {
                bc.TargetAddElement(script.connectionToClient, StalkyConfigs.error, 4U, false);
                yield break;
            }

            // Wait another frame after the while loops that goes over players.
            // Only useful for +100 player servers and the potatest server in this case, but it goes to show how to do these.
            yield return MEC.Timing.WaitForOneFrame;
            Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(PortalProcedure(script, portalPosition - Vector3.up), MEC.Segment.Update));

            stalkyCd = Time.time + StalkyConfigs.cooldownCfg;
            Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(StalkyCooldownAnnounce(60f), MEC.Segment.Update));
            stalky106LastTime = Time.time;
            disableFor = Time.time + 10f;
            if (!StalkyConfigs.parsedRoleNames.TryGetValue((int)target.characterClassManager.CurClass, out string className))
            {
                className = defaultRoleNames[(int)target.characterClassManager.CurClass];
            }
            string data = StalkyConfigs.newStalkMessage.Replace("$player", target.nicknameSync.Network_myNickSync).Replace("$class", className).Replace("$cd", StalkyConfigs.cooldownCfg.ToString());
            bc.TargetAddElement(script.connectionToClient, data, 6U, false);
        }

        public static IEnumerator<float> PortalProcedure(Scp106PlayerScript script, Vector3 pos)
        {
            yield return MEC.Timing.WaitForOneFrame;
            Scp106PlayerScript component = PlayerManager.localPlayer.GetComponent<Scp106PlayerScript>();
            component.NetworkportalPosition = pos;
            Animator anim = component.portalPrefab.GetComponent<Animator>();
            anim.SetBool("activated", false);
            component.portalPrefab.transform.position = pos;
            if (StalkyConfigs.autoTp) Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(ForceTeleportLarry(script), MEC.Segment.FixedUpdate));
            yield return MEC.Timing.WaitForSeconds(1f);
            anim.SetBool("activated", true);
        }
        private static IEnumerator<float> ForceTeleportLarry(Scp106PlayerScript script)
        {
            yield return MEC.Timing.WaitForSeconds(StalkyConfigs.autoDelay);
            do
            {
                script.CallCmdUsePortal();
                yield return MEC.Timing.WaitForOneFrame;
            }
            while (!script.goingViaThePortal);
        }
        private static IEnumerator<float> StalkyCooldownAnnounce(float duration)
        {
            yield return MEC.Timing.WaitForSeconds(duration);

            if (StalkyCreatePortalPatch.ForceDisable) yield break;
            
            Broadcast bc = PlayerManager.localPlayer.GetComponent<Broadcast>();
            foreach (var rh in EXILED.Plugin.GetHubs())
            {
                if (rh.characterClassManager.CurClass == RoleType.Scp106)
                {
                    bc.TargetAddElement(rh.scp079PlayerScript.connectionToClient, StalkyConfigs.newStalkReady, 6U, false);
                }
            }
        }
    }
}
