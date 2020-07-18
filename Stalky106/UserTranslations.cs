using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Stalky106
{
	[Serializable]
	public class UserTranslations
	{
		[Description("The names to be displayed when stalking (example, if ClassD is D-BOI, it will say D-BOI instead.")]
		public Dictionary<RoleType, string> RoleDisplayNames { set; get; } = new Dictionary<RoleType, string>()
		{
			{ RoleType.Scp173, "<color=#F00>SCP-173</color>" },
			{ RoleType.ClassD, "<color=#FF8E00>Class D</color>" },
			{ RoleType.Spectator, "Spectator" },
			{ RoleType.Scp106, "<color=#F00>SCP-106</color>" },
			{ RoleType.NtfScientist, "<color=#0096FF>NTF Scientist</color>" },
			{ RoleType.Scp049, "<color=#F00>SCP-049</color>" },
			{ RoleType.Scp096, "<color=#f00>SCP-096</color>" },
			{ RoleType.Scp0492, "<color=#f00>Zombie</color>" },
			{ RoleType.NtfLieutenant, "<color=#0096FF>NTF Lieutenant</color>" },
			{ RoleType.NtfCommander, "<color=#0096FF>NTF Commander</color>" },
			{ RoleType.NtfCadet, "<color=#0096FF>NTF Cadet</color>" },
			{ RoleType.Tutorial, "Tutorial" },
			{ RoleType.FacilityGuard, "<color=#59636f>Facility Guard</color>" },
			{ RoleType.Scp93953, "<color=#f00>SCP-939-53</color>" },
			{ RoleType.Scp93989, "<color=#f00>SCP-939-89</color>" }
		};

		public string WelcomeBroadcast { set; get; } = @"<size=80><color=#0020ed><b>Stalk</b></color></size>\nIn this server, you can <color=#0020ed><b>stalk</b></color> humans by double-clicking the portal creation button in the <b>[TAB]</b> menu.";
		public string StalkReady { set; get; } = @"\n<b><color=#0020ed><b>Stalk</b></color> is <color=#00e861>ready</color></b>.\n<size=30>Double-click your portal creating tool to use it.</size>";
		public string DoubleClick { set; get; } = @"\nClick the portal creation tool again to <color=#ff0955><b>Stalk</b></color> a random player.";
		public string StalkMessage { set; get; } = @"\n<i>You will <color=#0020ed><b>stalk</b></color> <b>$player</b>, who is a $class</i>\n<size=30><color=#FFFFFF66>Cooldown: $cd</color></size>";
		public string NoTargetsLeft { set; get; } = @"\nNo targets found.";
		public string ConsoleInfo { set; get; } = @"Stalky106 enables additional functionality to SCP-106 by giving him the ability to place a portal to a random player, bringing him closer to the lore";
		public string Cooldown_Message { set; get; } = @"\nYou have to wait $time seconds to use <color=#0020ed><b>Stalk</b></color>.";
		public string OnGround { set; get; } = @"\nYou have to be on the ground to <color=#c9002c><b>stalk</b></color> people.";
		public string Error { set; get; } = @"\nAn error ocurred. Please, try it again.";
	}
}
