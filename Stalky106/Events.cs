using EXILED;
using System;
using System.Collections.Generic;

namespace Stalky106
{
	internal class EventHandlers
	{
		internal void OnRoundStart()
		{
		try {
			if (StalkyMethods.ForceDisable) return;
			}
			catch (Exception ex) {
				Log.Error("ForceDisable threw: " + ex.Message);
			}
			try
			{
				StalkyMethods.StalkyCooldown = StalkyConfigs.initialCooldown;
			}
			catch (Exception ex)
			{
				Log.Error("StalkyCooldown/InitialCooldown threw: " + ex.Message);
			}
		}

		internal void OnSetClass(SetClassEvent ev)
		{
			if (StalkyMethods.ForceDisable) return;

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
					ev.Sender.RaReply($"Stalky106#Stalk {(StalkyMethods.ForceDisable ? "was already" : string.Empty)} disabled.",
						!StalkyMethods.ForceDisable, false, string.Empty);

					StalkyMethods.ForceDisable = true;
					ev.Allow = false;
					break;

				case "STALK ENABLE":
					ev.Sender.RaReply($"Stalky106#Stalk {(!StalkyMethods.ForceDisable ? "was already" : string.Empty)} enabled.",
						StalkyMethods.ForceDisable, false, string.Empty);

					StalkyMethods.ForceDisable = false;
					ev.Allow = false;
					break;

				default:
					ev.Sender.RaReply("Stalky106#Command unrecognized.", false, false, string.Empty);
					ev.Allow = false;
					return;
			}
		}

		internal void OnCreatePortal(Scp106CreatedPortalEvent ev)
		{
			try
			{
				if (ev.Player != null)
				{
					ev.Allow = StalkyMethods.Stalk(ev.Player.GetComponent<Scp106PlayerScript>());
				}
			}
			catch (Exception ex)
			{
				if(StalkyConfigs.throwOnError)
				{
					EXILED.Log.Error("Error in Stalky106!");
					throw;
				}
				else 
				{
					Log.Error("Error in Stalky106: " + ex);
				}
			}
		}
	}
}

