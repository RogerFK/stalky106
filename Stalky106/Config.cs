using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Stalky106
{
	public class Config : IConfig
	{
		[Description("# # Configurations and translations related to Stalky-106 will be here. # # #")]
		public bool IsEnabled { get; set; } = true;
		public StalkyPreferences Preferences { get; set; } = new StalkyPreferences();
	}
}
