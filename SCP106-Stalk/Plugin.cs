using System.Collections.Generic;
using Smod2;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Lang;

namespace stalky106
{
	[PluginDetails(
		author = "RogerFK",
		name = "Stalky 106",
		description = "Enables a stalk command for SCP-106 to teleport below some random victim, so it's more close to the original lore",
		id = "rogerfk.scp106stalk",
		version = "1.0",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0,
		configPrefix = "stalky",
		langFile = "stalky_phrases"
		)]

	public class Stalky106 : Plugin
	{
		public override void OnDisable()
		{
			this.Info("Larry won't ever stalk you again at night.");
		}

		public override void OnEnable()
		{
			this.Info("Prepare to face Larry...");
		}
		public Dictionary<int, string> parsedRoleDict = new Dictionary<int, string>();
		[ConfigOption]
		public readonly bool enable = true;

		[ConfigOption]
		public readonly int cooldown = 30;

		[ConfigOption]
		public readonly int initialCooldown = 120;

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
		public readonly string[] roleNames = new string[] { "0:SCP-173", "1:Class D", "3:SCP-106", "4:NTF Scientist", "5:SCP-049", "6:Scientist",
			"8:Chaos Insurgent","9:SCP-096", "10:Zombie","11:NTF Lieutenant", "12:NTF Commander", "13:NTF Cadet", "14:Tutorial", "15:Facility Guard",
			"16:SCP-939-53", "17:SCP-939-89" };

		[LangOption]
		public readonly string firstbroadcast = @"In this server, you can stalk people by putting '.stalk' in the console.\nPress ` to open the console.";

		[LangOption]
		public readonly string stalkready = @"<color=#0F0>Your '.stalk' command is ready.</color>";

		[LangOption]
		public readonly string consoleinfo = @"Stalky106 enables additional functionality to SCP-106 by giving him the ability to place a portal to a random player, bringing him closer to the lore. Additionaly, you can use 'cmdbind g .stalk' (for example) to bind it to a key to not have to open the console every time you do it";

		[LangOption]
		public readonly string hauntmessage = @"<i>You will stalk <b>$player</b>, who is a $class</i>";

		[LangOption]
		public readonly string notscp106 = "You are not SCP-106!";

		[LangOption]
		public readonly string notargetsleft = "There's no targets left for you to stalk.";

		[LangOption]
		public readonly string error = "An error ocurred. Send this command again";

		[LangOption]
		public readonly string cooldownmsg = "You have to wait $time seconds to use this command";

		public override void Register()
		{
			AddEventHandlers(new EventHandlers(this));

			foreach (string key in roleNames)
			{
				string[] configInputs = key.Split(':');
				if (configInputs.Length == 2)
				{
					if (!int.TryParse(configInputs[0], out int rolenumber))
					{
						Error(configInputs[0] + " isn't a number, when it should (error caught in \'stalky_role_names\')");
						continue;
					}
					if (parsedRoleDict.ContainsKey(rolenumber))
					{
						Error("You tried to add " + configInputs[0] + " two times for the \'stalky_role_names\' config option. Just took the first one into account.");
						continue;
					}
					parsedRoleDict.Add(rolenumber, configInputs[1]);
				}
				else
				{
					this.Error(key + " isn't a valid option for \'stalky_role_names\'.");
				}
			}
			Info("Successfully loaded the configs");
		}
	}
}
