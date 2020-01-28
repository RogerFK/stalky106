using EXILED;
using Harmony;
using System;
using System.Collections.Generic;

namespace stalky106
{
	internal class EventHandlers
	{
		internal void OnRoundStart()
		{
			if (StalkyCreatePortalPatch.ForceDisable) return;

			StalkyMethods.StalkyCooldown = StalkyConfigs.initialCooldown;
		}

		internal void OnSetClass(SetClassEvent ev)
		{
			if (StalkyCreatePortalPatch.ForceDisable) return;

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

		internal void RACommand(ref RACommandEvent ev)
		{
			// safety first
			if (string.IsNullOrWhiteSpace(ev.Command)) return;

			string upperedCommand = ev.Command.ToUpperInvariant();

			if (!upperedCommand.StartsWith("STALK")) return;

			switch (upperedCommand)
			{
				case "STALK DISABLE":
					ev.Sender.RaReply($"Stalky106#Stalk {(StalkyCreatePortalPatch.ForceDisable ? "was already" : string.Empty)} disabled.",
						!StalkyCreatePortalPatch.ForceDisable, false, string.Empty);

					StalkyCreatePortalPatch.ForceDisable = true;
					break;

				case "STALK ENABLE":
					ev.Sender.RaReply($"Stalky106#Stalk {(!StalkyCreatePortalPatch.ForceDisable ? "was already" : string.Empty)} enabled.",
						StalkyCreatePortalPatch.ForceDisable, false, string.Empty);

					StalkyCreatePortalPatch.ForceDisable = false;
					break;

				default:
					ev.Sender.RaReply("Stalky106#Command unrecognized.", false, false, string.Empty);
					return;
			}
		}
	}
}

