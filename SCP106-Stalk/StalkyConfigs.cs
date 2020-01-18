using EXILED;
using System;
using System.Collections.Generic;
using System.IO;

namespace stalky106
{
	internal static class StalkyConfigs
	{
		private static readonly string translationPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "translations");
		private static readonly string stalkyTranslationPath = Path.Combine(translationPath, "stalky_translations.txt");
		internal static float cooldownCfg;

		internal static float initialCooldown;

		// By default, it ignores SCPs, Chaos Insurgents and the TUTORIAL class
		internal static List<int> ignoreTeams;

		// By default, it ignores SCP-106 and SCP-079
		internal static List<int> ignoreRoles;

		internal static bool announceReady;

		internal static bool autoTp;

		internal static float autoDelay;

		internal static Dictionary<int, string> parsedRoleNames;

		internal static string stalkBroadcast = "<size=80><color=#0020ed><b>Stalk</b></color></size>\nIn this server, you can <color=#0020ed><b>stalk</b></color> humans by double-clicking the portal creation button in the <b>[TAB]</b> menu.";
		internal static string newStalkReady = "\n<b><color=#0020ed><b>Stalk</b></color> is <color=#00e861>ready</color></b>.\n<size=30>Double-click your portal creating tool to use it.</size>";
		internal static string doubleClick = "\nClick the portal creation tool again to <color=#ff0955><b>Stalk</b></color> a random player.";
		internal static string newStalkMessage = "\n<i>You will <color=#0020ed><b>stalk</b></color> <b>$player</b>, who is a $class</i>\n<size=30><color=#FFFFFF66>Cooldown: $cd</color></size>";
		internal static string noTargetsLeft = "\nNo targets found.";
		internal static string consoleInfo = "Stalky106 enables additional functionality to SCP-106 by giving him the ability to place a portal to a random player, bringing him closer to the lore";
		internal static string cooldownmsg = "\nYou have to wait $time seconds to use <color=#0020ed><b>Stalk</b></color>.";
		internal static string onGround = "\nYou have to be on the ground to <color=#c9002c><b>stalk</b></color> people.";
		internal static string error = "\nAn error ocurred. Please, try it again.";

		internal static void ReloadConfigs()
		{
			// Configs
			cooldownCfg = Plugin.Config.GetFloat("stalky_cooldown", 40f);
			initialCooldown = Plugin.Config.GetFloat("stalky_initial_cooldown", 80f);
			ignoreTeams = Plugin.Config.GetIntList("stalky_ignore_teams");
			if (ignoreTeams == null || ignoreTeams.Count == 0)
			{
				ignoreTeams = new List<int>() { (int)Team.SCP, (int)Team.CHI, (int)Team.TUT };
			}
			ignoreRoles = Plugin.Config.GetIntList("stalky_ignore_roles");
			if (ignoreRoles == null || ignoreRoles.Count == 0)
			{
				ignoreRoles = new List<int>() { 3, 7 };
			}
			announceReady = Plugin.Config.GetBool("stalky_announce_ready", true);
			autoTp = Plugin.Config.GetBool("stalky_auto_tp", true);
			autoDelay = Plugin.Config.GetFloat("stalky_auto_delay", 0.2f);
			var temp = Plugin.Config.GetStringDictionary("stalky_role_names");
			if (temp != null)
			{
				parsedRoleNames = new Dictionary<int, string>(temp.Count);
				foreach (var field in temp)
				{
					if (int.TryParse(field.Key, out int result))
					{
						try { parsedRoleNames.Add(result, field.Value); }
						catch { Plugin.Error($"Duped role {field.Key}"); }
					}
					else
					{
						Plugin.Error($"Unknown key for line {field.Key}:{field.Value}.");
					}
				}
			}
			else
			{
				parsedRoleNames = new Dictionary<int, string>(0);
			}

			// Translations

			if (File.Exists(translationPath))
			{
				var lines = File.ReadAllLines(translationPath);
				foreach (string line in lines)
				{
					var splitted = line.Split(':');
					string key = splitted[0];
					string value = line.Substring(key.Length + 1).Trim();
					// Known keys, doing a switch massively saves time, didn't need to be modular anyways
					switch (key)
					{
						case "stalk_broadcast":
							stalkBroadcast = value;
							break;
						case "new_stalk_ready":
							newStalkReady = value;
							break;
						case "double_click":
							doubleClick = value;
							break;
						case "new_stalk_message":
							newStalkMessage = value;
							break;
						case "no_targets_left":
							noTargetsLeft = value;
							break;
						case "cooldownmsg":
							cooldownmsg = value;
							break;
						case "on_ground":
							onGround = value;
							break;
						case "error":
							error = value;
							break;
						case "console_info":
							consoleInfo = value;
							break;
						default:
							Plugin.Error($"Unknown translation: {key}");
							break;
					}
				}
			}
			else
			{
				if (!Directory.Exists(translationPath))
				{
					Directory.CreateDirectory(translationPath);
				}
				// Create and write all the text of the default file contents if the file is not found
				File.WriteAllText(stalkyTranslationPath, defaultFileContents);
			}
		}
		#region File Contents
		private static readonly string defaultFileContents = @"stalkBroadcast: <size=80><color=#0020ed><b>Stalk</b></color></size>\nIn this server, you can <color=#0020ed><b>stalk</b></color> humans by double-clicking the portal creation button in the <b>[TAB]</b> menu." + Environment.NewLine +
						@"new_stalk_ready: \n<b><color=#0020ed><b>Stalk</b></color> is <color=#f2245f>ready</color></b>\n<size=30>Double-click your portal creating tool to use it.</size>" + Environment.NewLine +
						@"double_click: \nClick the portal creation tool again to <color=#ff0955><b>Stalk</b></color> a random player." + Environment.NewLine +
						@"console_info: Stalky106 enables additional functionality to SCP-106 by giving him the ability to place a portal to a random player, bringing him closer to the lore." + Environment.NewLine +
						@"new_stalk_message: \n<i>You will <color=#0020ed><b>stalk</b></color> <b>$player</b>, who is a $class</i>\n<size=30><color=#FFFFFF66>Cooldown: $cd seconds</color></size>" + Environment.NewLine +
						@"no_targets_left: \nNo targets found." + Environment.NewLine +
						@"cooldownmsg: \nYou have to wait $time seconds to use <color=#0020ed><b>Stalk</b></color>." + Environment.NewLine +
						@"on_ground: \nYou have to be on the ground to <color=#c9002c><b>stalk</b></color> people.";
		#endregion
	}
}
