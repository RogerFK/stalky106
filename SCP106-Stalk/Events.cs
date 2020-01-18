using EXILED;
using System.Collections.Generic;
using UnityEngine;

namespace stalky106
{
	internal class EventHandlers
	{
		public EventHandlers()
		{
		}

		internal void OnRoundStart()
		{
			StalkyMethods.StalkyCooldown = StalkyConfigs.initialCooldown;
		}

		internal void OnSetClass(SetClassEvent ev)
		{
			if (ev.Role == RoleType.Scp106)
			{
				MEC.Timing.RunCoroutine(DelayBroadcast(ev.Player), MEC.Segment.Update, "DelayBroadcast");
			}
		}

		private IEnumerator<float> DelayBroadcast(ReferenceHub player)
		{
			yield return MEC.Timing.WaitForSeconds(0.5f);
			// Shut up. I don't know if it may change or... something.
			if (player.GetComponent<CharacterClassManager>().CurClass == RoleType.Scp106)
			{
				player.GetComponent<Broadcast>().TargetAddElement(player.scp079PlayerScript.connectionToClient, StalkyConfigs.stalkBroadcast, 10U, false);
			}
		}
	}
}

