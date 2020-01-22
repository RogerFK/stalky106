using EXILED;
using Harmony;
using System.Collections.Generic;

namespace stalky106
{
	internal class EventHandlers
	{
		internal void OnRoundStart()
		{
			StalkyMethods.StalkyCooldown = StalkyConfigs.initialCooldown;
		}

		internal void OnSetClass(SetClassEvent ev)
		{
			if (ev.Player == PlayerManager.localPlayer) return;

			if (ev.Role == RoleType.Scp106)
			{
				Stalky106.Coroutines.Add(MEC.Timing.RunCoroutine(DelayBroadcast(ev.Player), MEC.Segment.FixedUpdate));
			}
		}

		private IEnumerator<float> DelayBroadcast(ReferenceHub player)
		{
			yield return MEC.Timing.WaitForSeconds(0.5f);
			if (player.characterClassManager.CurClass == RoleType.Scp106)
			{
				player.GetComponent<Broadcast>().TargetAddElement(player.scp079PlayerScript.connectionToClient, StalkyConfigs.stalkBroadcast, 10U, false);
				player.GetComponent<GameConsoleTransmission>().SendToClient(player.GetComponent<Mirror.NetworkIdentity>().connectionToClient, StalkyConfigs.consoleInfo, "white");
			}
		}
	}
}

