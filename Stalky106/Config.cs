namespace Stalky106
{
	using Exiled.API.Interfaces;
	using Configs;

	public class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		public StalkyPreferences Preferences { get; set; } = new StalkyPreferences();
	}
}
