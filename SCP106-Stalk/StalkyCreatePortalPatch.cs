using Harmony;
using UnityEngine;

namespace stalky106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), "CallCmdMakePortal")]
    class StalkyCreatePortalPatch
    {
        public static bool ForceDisable = false;
        public static bool Prefix(Scp106PlayerScript __instance)
        {
            if (ForceDisable)
            {
                return true;
            }
            if (!__instance.GetComponent<FallDamage>().isGrounded || !__instance.iAm106 || __instance.goingViaThePortal)
            {
                return false;
            }

            // If Stalky is disabled by force, don't even create a portal for the guy
            // Avoids 1-frame trick to (probably unintentionally) "cancel" the Stalk.
            if (StalkyMethods.disableFor > Time.time)
            {
                return false;
            }

            float timeDifference = Time.time - StalkyMethods.stalky106LastTime;
            float cdAux = StalkyMethods.StalkyCooldown;
            Broadcast bc = PlayerManager.localPlayer.GetComponent<Broadcast>();
            if (timeDifference > 6f)
            {
                StalkyMethods.stalky106LastTime = Time.time;
                if (cdAux < 0)
                {
                    bc.TargetClearElements(__instance.connectionToClient);
                    bc.TargetAddElement(__instance.connectionToClient, StalkyConfigs.doubleClick, 6u, false);
                }
                return true;
            }
            else
            {
                bc.TargetClearElements(__instance.connectionToClient);
                if (cdAux > 0)
                {
                    StalkyMethods.stalky106LastTime = Time.time;
                    int i = 0;
                    for (; i < 5 && cdAux > i; i++) bc.TargetAddElement(__instance.connectionToClient, StalkyConfigs.cooldownmsg.Replace("$time", (cdAux - i).ToString("00")), 1u, false);
                    StalkyMethods.disableFor = Time.time + i + 1;
                    return true;
                }
                StalkyMethods.disableFor = Time.time + 4;
                Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(StalkyMethods.StalkCoroutine(__instance, bc), MEC.Segment.Update));
                return false;
            }
        }
    }
}
