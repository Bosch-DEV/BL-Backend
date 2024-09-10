using System.Diagnostics;

namespace LauncherBackEnd.game;
public class Game(string internalName, string displayName, string description, string backgroundImageURL, string backgroundImagePath, string version, string binaryURL, string binaryPath, string binaryName) {

    public string internalName = internalName, displayName = displayName, description = description, backgroundImageURL = backgroundImageURL, backgroundImagePath = backgroundImagePath, version = version, binaryURL = binaryURL, binaryPath = binaryPath, binaryName = binaryName;

    public string? GetInternalName() => internalName;
    public string? GetDisplayName() => displayName;
    public string? GetDescription() => description;
    public string? GetBackgroundImageURL() => backgroundImageURL;
    public string? GetBackgroundImagePath() => backgroundImagePath;
    public string? GetVersion() => version;
    public string? GetBinaryURL() => binaryURL;
    public string? GetBinaryPath() => binaryPath;

    public Game Start() {
        LaunchCMD();
        return this;
    }

    private void LaunchCMD() {
        ProcessStartInfo info = new() {
            CreateNoWindow = false,
            UseShellExecute = false,
            FileName = Environment.SpecialFolder.ApplicationData.ToString() + $"/BattleLauncher/Games/{this.internalName}/" + binaryPath,
            WindowStyle = ProcessWindowStyle.Maximized
        };
        try {
            new Thread(() => {
                using (Process exePr = Process.Start(info)) {
                    exePr.WaitForExit();
                }
            }).Start();
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }


}

