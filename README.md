# stalky106
Ever wondered "how can SCP-106 get closer to its lore?" and wished that they just dissapeared at teslas, to make them easier to lose? Well, this plugin doesn't do that, but instead gets him closer to the lore by giving him the ability to stalk random people by placing a portal at their feet!

*Idea by ConnorTheCone and RogerFK*

# Command

To use, open up the console, which varies between keyboard layouts, but it's usually the key below the ESC key or else ; (in the Spanish keyboard layout it's ñ, for example)
The command is `.stalk` and can be bound to a key by putting `cmdbind <key> .stalk`. Only SCP-106 can use this command and, for now, you can't target a specific player *(note: if you really want that feature, make sure to fork the repo, code it yourself and do a Pull Request!)*


# Translations

This plugin uses the default translation method used since Smod 3.4.0. Feel free to change it in the sm_translations folder that's just below your sm_plugins folder, the file is called stalky.txt. You can change any line and even use the [UnityEngine's Rich Text Formatting](https://docs.unity3d.com/Manual/StyledText.html) 


# Configs
| Config Option | Value Type | Default Value | Description |
|:-----------------------:|:----------:|:------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|:----------------------------------------------------------------------------------------------------------------------:|
| stalky_enable | boolean | true | Enables/Disables this plugin |
| stalky_cooldown | int | 30 | The time SCP-106's have to wait before stalking another player |
| stalky_initial_cooldown | int | 120 | The time SCP-106's have to wait before stalking it's first victim |
| stalky_ignore_teams | int List | 0, 2, 6 | [Teams](https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#team) that the "stalk" command will ignore. (Defaults to SCPs, Chaos and Tutorial teams) |
| stalky_ignore_roles | int List | 3, 7 | [Roles](https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#role) that the "stalk" command will ignore. (Defaults to other SCP-106 and SCP-079) |
| stalky_announce_ready | bool | true | Tells all SCP-106's when are they have their stalk command ready *(Note: disabling this will avoid using C# Tasks everytime, which will save resources. If your CPU usage is high, you may want to disable this feature)*|
| stalky_auto_tp | bool | true | Teleports SCP-106 just when the portal is created below their victim's feet. Really OP, but faithful to the main lore. |
| stalky_auto_delay | float | 0.2 | Time to elapse after the portal is completely created |
| stalky_role_names | Dictionary | 0:SCP-173, 1:Class D, 3:SCP-106, 4:NTF Scientist, 5:SCP-049, 6:Scientist, 8:Chaos Insurgent, 9:SCP-096, 10:Zombie, 11:NTF Lieutenant, 12:NTF Commander, 13:NTF Cadet, 14:Tutorial, 15:Facility Guard, 16:SCP-939-53, 17:SCP-939-89 | The role name Dictionary that will get displayed at the top when Larry finds a victim (for example, it would say "teleporting to X guy with role Y"). You can give it colors, style or even funny names. Whatever you feel like, but follow the aforementioned Rich Text guide to do it. |
