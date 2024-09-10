namespace LauncherBackEnd.game;
public class Game(string internalName, string displayName, string description, string backgroundImageURL, string backgroundImagePath, string version, string binaryURL, string binaryPath) {

    public string internalName = internalName, displayName = displayName, description = description, backgroundImageURL = backgroundImageURL, backgroundImagePath = backgroundImagePath, version = version, binaryURL = binaryURL, binaryPath = binaryPath;

    public string? GetInternalName() => internalName;
    public string? GetDisplayName() => displayName;
    public string? GetDescription() => description;
    public string? GetBackgroundImageURL() => backgroundImageURL;
    public string? GetBackgroundImagePath() => backgroundImagePath;
    public string? GetVersion() => version;
    public string? GetBinaryURL() => binaryURL;
    public string? GetBinaryPath() => binaryPath;

}

