using System;
using System.ComponentModel;

namespace Stalky106
{
	[Serializable]
	public class UserPreferences
	{
		public bool AnnounceReady { get; set; } = true;
		public float Cooldown { set; get; } = 40f;
		public float InitialCooldown { set; get; } = 80f;

		[Description("Should SCP-106 automatically teleport when stalking, or should he teleport manually?")]
		public bool AutoTp { set; get; } = true;

		[Description("Delay for the auto-teleportation. Ignored if auto_tp is false.")]
		public float AutoDelay { set; get; } = 0.2f;

		[Description("Forces SCP-106 to be teleported. If he's jumping, it will \"wait\" until he's on the ground to teleport him. Frame-perfect trick allows SCP-106 to move while teleporting, set this to false if players abuse it.")]
		public bool ForceAutoTp { get; set; }

		[Description("ignore_teams and ignore_roles will ignore said teams and roles when searching for a player to stalk")]
		public Team[] IgnoreTeams { set; get; } = new Team[] { Team.SCP, Team.CHI, Team.TUT };
		public RoleType[] IgnoreRoles { set; get; } = new RoleType[] { RoleType.Scp106, RoleType.Scp079 };
	}
}
