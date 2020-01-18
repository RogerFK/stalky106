using Harmony;
using UnityEngine;

namespace stalky106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), "CallCmdMakePortal")]
    class StalkyCreatePortalPatch
    {
        public static bool Prefix(Scp106PlayerScript __instance)
        {
            if (!__instance._interactRateLimit.CanExecute(true))
            {
                return false;
            }
            if (!__instance.GetComponent<FallDamage>().isGrounded)
            {
                return false;
            }
            Debug.DrawRay(__instance.transform.position, -__instance.transform.up, Color.red, 10f);
            RaycastHit raycastHit;
            if (__instance.iAm106 && !__instance.goingViaThePortal && Physics.Raycast(new Ray(__instance.transform.position, -__instance.transform.up), out raycastHit, 10f, __instance.teleportPlacementMask))
            {
                //only thing that's modified, below here
                if (StalkyMethods.disableFor > Time.time)
                {
                    __instance.SetPortalPosition(raycastHit.point - Vector3.up);
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
                    __instance.SetPortalPosition(raycastHit.point - Vector3.up);
                    return false;
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
                        return false;
                    }
                    StalkyMethods.disableFor = Time.time + 4;
                    MEC.Timing.RunCoroutine(StalkyMethods.StalkCoroutine(__instance, bc), MEC.Segment.Update, "StalkCoroutine");
                }
            }
            return false;
        }
    }
}
