using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Stalky106
{
	public class PluginConfig : IConfig
	{
		[Description("# # Configurations and translations related to Stalky-106 will be here. # # #")]
		public bool IsEnabled { get; set; } = true;
		public UserPreferences Preferences { get; set; } = new UserPreferences();

		public UserTranslations Translations { get; set; } = new UserTranslations();
	}
}
