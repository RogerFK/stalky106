using EXILED;
using System.Collections.Generic;
using UnityEngine;

namespace stalky106
{
	internal class EventHandlers
	{
		public bool roundActuallyStarted = false;
		internal void OnRoundStart()
		{
			roundActuallyStarted = true;
			StalkyMethods.StalkyCooldown = StalkyConfigs.initialCooldown;
		}

		internal void OnSetClass(SetClassEvent ev)
		{
			if (!roundActuallyStarted || ev.Player == PlayerManager.localPlayer) return;
			
			if (ev.Role == RoleType.Scp106)
			{
				MEC.Timing.RunCoroutine(DelayBroadcast(ev.Player), MEC.Segment.FixedUpdate, "DelayBroadcast");
			}
		}

		private IEnumerator<float> DelayBroadcast(ReferenceHub player)
		{
			yield return MEC.Timing.WaitForSeconds(0.5f);
			if (player.characterClassManager.CurClass == RoleType.Scp106)
			{
				player.GetComponent<Broadcast>().TargetAddElement(player.scp079PlayerScript.connectionToClient, StalkyConfigs.stalkBroadcast, 10U, false);
			}
		}
	}
}

