using System.Collections.Generic;
using Smod2;
using Smod2.API;
using UnityEngine;

namespace stalky106
{
	static internal class Methods
	{
		internal static void MovePortal(Scp106PlayerScript auxScp106Component, Vector3 pos, bool pocket)
		{
            MEC.Timing.RunCoroutine(PortalProcedure(auxScp106Component, pos, pocket), MEC.Segment.FixedUpdate);
		}

        private static IEnumerator<float> PortalProcedure(Scp106PlayerScript auxScp106Component, Vector3 pos, bool pocket)
        {
            yield return 0f; // Wait one frame
            Scp106PlayerScript local106Component = PlayerManager.localPlayer.GetComponent<Scp106PlayerScript>();
            local106Component.NetworkportalPosition = pos;
            Animator anim = local106Component.portalPrefab.GetComponent<Animator>();
            // I don't know what this does but imma include it so it works like like the game does
            anim.SetBool("activated", false);
            local106Component.portalPrefab.transform.position = pos;
            if (Stalky106.Instance.autoTp || pocket)
            {
                MEC.Timing.RunCoroutine(ForceTeleportLarry(auxScp106Component), 1);
            }
            yield return MEC.Timing.WaitForSeconds(1f);
            anim.SetBool("activated", true);
          
        }
		
        private static IEnumerator<float> ForceTeleportLarry(Scp106PlayerScript auxScp106Component)
        {
            yield return MEC.Timing.WaitForSeconds(Stalky106.Instance.autoDelay);
            // Try to do the portal sequence every frame
            do
            {
                auxScp106Component.CallCmdUsePortal();
                yield return 0f; // 0f == 1 frame for MEC
            } while (!auxScp106Component.goingViaThePortal);
        }
		internal static IEnumerator<float> DelayBroadcasts(Player player)
		{
			yield return MEC.Timing.WaitForSeconds(0.2f);
			if (player.TeamRole.Role == Role.SCP_106)
			{
				if (Stalky106.Instance.stalk)  { player.PersonalBroadcast(10, Stalky106.Instance.stalkBroadcast, false); }
				if (Stalky106.Instance.pocket) { player.PersonalBroadcast(10, Stalky106.Instance.pocketBroadcast, false); player.SendConsoleMessage(Stalky106.Instance.consolePocket, "white"); }
			}
		}

        internal static void AnnounceCooldown(float cd)
        {
            MEC.Timing.RunCoroutine(CdMethod(cd), MEC.Segment.FixedUpdate);
            
        }

        private static IEnumerator<float> CdMethod(float cd)
        {
            yield return MEC.Timing.WaitForSeconds(cd);
            foreach (Player larry in PluginManager.Manager.Server.GetPlayers(Role.SCP_106))
            {
                larry.PersonalClearBroadcasts();
                larry.PersonalBroadcast(4, Stalky106.Instance.newStalkReady, false);
            }
        }
        internal static void DelayBroadcast(this Player player, uint time, string message, bool monospaced = false)
        {
            MEC.Timing.RunCoroutine(_DelayBc(player, time, message, monospaced), MEC.Segment.FixedUpdate);
        }
        private static IEnumerator<float> _DelayBc(Player player, uint time, string message, bool monospaced = false)
        {
            yield return MEC.Timing.WaitForSeconds(0.1f);
            player.PersonalBroadcast(time, message, monospaced);
        }
        internal static bool InPocketDimension(this Player player) => Vector.Distance(player.GetPosition(), PocketHandler.pocketDimension) < 50f;
    }
}

