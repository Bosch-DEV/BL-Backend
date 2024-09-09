namespace LauncherBackEnd.config;

public class SettingHandler : Config<Settings> {
    public SettingHandler() : base("settings.conf") {

    }

    protected override Settings GenerateDefault() {
        return new Settings();
    }
}
