using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace stalky106
{
	[PluginDetails(
		author = "RogerFK",
		name = "Stalky 106 v2",
		description = "Enables a stalk mechanic for SCP-106 to teleport below some random victim, so it's closer to the original lore",
		id = "rogerfk.scp106stalk",
		version = "2.0",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0,
		configPrefix = "stalky",
		langFile = "stalky_phrases"
		)]

	public class Stalky106 : Plugin
	{
        public static Stalky106 Instance;
        public override void OnDisable()
		{
			this.Info("Disabled Stalky106 v2.0.");
		}

		public override void OnEnable()
		{
			this.Info("Prepare to face Larry...");
		}
		public Dictionary<int, string> parsedRoleDict = new Dictionary<int, string>();


        [ConfigOption]
		public readonly bool enable = true;

        [ConfigOption]
        public readonly bool debug = true;

        [ConfigOption]
		public readonly bool stalk = true;

        [ConfigOption]
        public readonly int threshold = 5;

        [ConfigOption]
		public readonly bool pocket = true;

		[ConfigOption]
		public readonly bool pocketDamage = false;

		[ConfigOption]
		public readonly float cooldown = 30;

		[ConfigOption]
		public readonly float initialCooldown = 80;

		[ConfigOption]
		// By default, it ignores SCPs, Chaos Insurgents and the TUTORIAL class
		public readonly int[] ignoreTeams = new int[] { 0, 2, 6 };

		[ConfigOption]
		// By default, it ignores SCP-106 and SCP-079
		public readonly int[] ignoreRoles = new int[] { 3, 7 };

		[ConfigOption]
		public readonly bool announceReady = true;

		[ConfigOption]
		public readonly bool autoTp = true;

		[ConfigOption]
		public readonly float autoDelay = 0.05f;

		[ConfigOption]
		public readonly string[] roleNames = new string[] { "0:SCP-173", "1:Class D", "3:SCP-106", "4:NTF Scientist", "5:SCP-049", "6:Scientist",
			"8:Chaos Insurgent","9:SCP-096", "10:Zombie","11:NTF Lieutenant", "12:NTF Commander", "13:NTF Cadet", "14:Tutorial", "15:Facility Guard",
			"16:SCP-939-53", "17:SCP-939-89" };

		[LangOption]
		public readonly string stalkBroadcast = @"<size=80><color=#0020ed><b>Stalk</b></color></size>\nIn this server, you can <color=#0020ed><b>stalk</b></color> humans by double-clicking the portal creation button in the <b>[TAB]</b> menu.";

		[LangOption]
		public readonly string pocketBroadcast = @"You can also write or bind <b>'.pocket'</b> to visit the pocket dimension.\nPress <b>[`]</b> or <b>[~]</b> to open the console for more info.";

		[LangOption]
		public readonly string gettingOut = @"To get out of the Pocket Dimension, use your portal or go through a door to teleport yourself to the portal itself";

		[LangOption]
		public readonly string newStalkReady = @"\n<b><color=#0020ed><b>Stalk</b></color> is <color=#00e861>ready</color></b>.\n<size=30>Double-click your portal creating tool to use it.</size>";

        [LangOption]
        public readonly string doubleClick = @"\nClick the portal creation tool again to <color=#ff0955><b>Stalk</b></color> a random player.";

        [LangOption]
		public readonly string consolePocket = @"In this server, you can use <b>.pocket</b> to visit the pocket dimension. Additionaly, you can use 'cmdbind p .pocket' (for example) to bind it to a key to not have to open the console every time you want to do it.";

		[LangOption]
		public readonly string alreadyInPocket = @"\n<color=#B00>You're already in the <b>Pocket Dimension</b>.</color>";

		[LangOption]
		public readonly string newStalkMessage = @"\n<i>You will <color=#0020ed><b>stalk</b></color> <b>$player</b>, who is a $class</i>\n<size=30><color=#FFFFFF66>Cooldown: $cd</color></size>";

		[LangOption]
		public readonly string notscp106 = "You are not SCP-106!";

		[LangOption]
		public readonly string noTargetsLeft = "No targets found.";

		[LangOption]
		public readonly string error = "An error ocurred. Please, try it again.";

		[LangOption]
		public readonly string cooldownmsg = @"\nYou have to wait $time seconds to use <color=#0020ed><b>Stalk</b></color>.";

		[LangOption]
		public readonly string onGround = @"\nYou have to be on the ground to <color=#c9002c><b>stalk</b></color> people.";

		public override void Register()
		{
            Instance = this;
            AddEventHandlers(new StalkyEvents(this));
            AddEventHandlers(new PocketHandler(this));
            RefreshRoleNames();
		}
        public static void RefreshRoleNames()
        {
            foreach (string key in Instance.roleNames)
            {
                string[] configInputs = key.Split(':');
                if (configInputs.Length == 2)
                {
                    if (!int.TryParse(configInputs[0], out int rolenumber))
                    {
                        Instance.Error(configInputs[0] + " isn't a number, when it should (error caught in \'stalky_role_names\')");
                        continue;
                    }
                    if (Instance.parsedRoleDict.ContainsKey(rolenumber))
                    {
                        Instance.Error("You tried to add " + configInputs[0] + " two times for the \'stalky_role_names\' config option. Just took the first one into account.");
                        continue;
                    }
                    Instance.parsedRoleDict.Add(rolenumber, configInputs[1]);
                }
                else
                {
                    Instance.Error(key + " isn't a valid option for \'stalky_role_names\'.");
                }
            }
            Instance.Info("Successfully loaded the configs");
        }
	}
}
