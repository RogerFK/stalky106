namespace Stalky106.Configs
{
	using System.ComponentModel;

	public class StalkyPreferences
	{
		public bool AnnounceReady { get; set; } = true;

		public float Cooldown { set; get; } = 40f;

		public float InitialCooldown { set; get; } = 80f;

		[Description("Should SCP-106 automatically teleport when stalking, or should he teleport manually?")]
		public bool AutoTp { set; get; } = true;

		[Description("Delay for the auto-teleportation. Ignored if auto_tp is false.")]
		public float AutoDelay { set; get; } = 0.2f;

		[Description("Forces SCP-106 to be teleported. If he's jumping, it will \"wait\" until he's on the ground to teleport him. Frame-perfect trick allows SCP-106 to move while teleporting, set this to false if players abuse it.")]
		public bool ForceAutoTp { get; set; } = true;

		[Description("ignore_teams and ignore_roles will ignore said teams and roles when searching for a player to stalk")]
		public Team[] IgnoreTeams { set; get; } = { Team.SCP, Team.CHI, Team.TUT };

		public RoleType[] IgnoreRoles { set; get; } =  { RoleType.Scp106, RoleType.Scp079 };
		
		[Description("Changes behaviour of stalk by only allowing to stalk players in the same zone as Larry")]
		public bool SameZoneOnly { get; set; } = false;

		[Description("The minimum amount of targetable players to be able to stalk.")]
		public int MinimumAliveTargets { get; set; } = 0;

		[Description("The minimum amount of alive players (any role) to be able to stalk.")]
		public int MinimumAlivePlayers { get; set; } = 0;

		[Description("The minimum amount of players connected in the server for Stalky to work.")]
		public int MinimumPlayers { get; set; } = 0;
	}
}
