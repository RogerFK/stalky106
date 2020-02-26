using Harmony;

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

			return StalkyMethods.Stalk(__instance);
		}
	}
}
