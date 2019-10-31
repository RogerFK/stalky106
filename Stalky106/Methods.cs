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
	static internal class Methods
	{
		internal static void MovePortal(Scp106PlayerScript auxScp106Component, Vector3 pos, bool pocket)
		{
            MEC.Timing.RunCoroutine(PortalProcedure(auxScp106Component, pos, pocket), MEC.Segment.FixedUpdate);
		}

        private static IEnumerator<float> PortalProcedure(Scp106PlayerScript auxScp106Component, Vector3 pos, bool pocket)
        {
            yield return MEC.Timing.WaitForSeconds(0.1f);
            auxScp106Component.NetworkportalPosition = pos;
            Animator anim = auxScp106Component.portalPrefab.GetComponent<Animator>();
            // I don't know what this does but imma include it so it works like like the game does
            anim.SetBool("activated", false);
            auxScp106Component.portalPrefab.transform.position = pos; 
            if (Stalky106.Instance.autoTp || pocket)
            {
                yield return MEC.Timing.WaitForSeconds(Stalky106.Instance.autoDelay);
                auxScp106Component.CallCmdUsePortal();
            }
            yield return MEC.Timing.WaitForSeconds(1f);
            anim.SetBool("activated", true);
          
        }
		
		internal static IEnumerator<float> DelayBroadcasts(Player player)
		{
			yield return MEC.Timing.WaitForSeconds(0.2f);
			if (player.TeamRole.Role == Role.SCP_106)
			{
				if (Stalky106.Instance.stalk)  { player.PersonalBroadcast(7, Stalky106.Instance.stalkBroadcast, false); }
				if (Stalky106.Instance.pocket) { player.PersonalBroadcast(7, Stalky106.Instance.pocketBroadcast, false); player.SendConsoleMessage(Stalky106.Instance.consolePocket, "white"); }
			}
		}

        internal static void AnnounceCooldown(float cd)
        {
            MEC.Timing.RunCoroutine(CdMethod(cd), MEC.Segment.FixedUpdate);
            
        }

        private static IEnumerator<float> CdMethod(float cd)
        {
            Stalky106.Instance.Info("Put .stalk on cooldown for " + cd + " seconds.");
            yield return MEC.Timing.WaitForSeconds(cd);
            foreach (Player larry in PluginManager.Manager.Server.GetPlayers(Role.SCP_106))
            {
                larry.PersonalBroadcast(3, Stalky106.Instance.newStalkReady, false);
            }
            Stalky106.Instance.Info("Cooldown for .stalk ended");
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
    }
}

