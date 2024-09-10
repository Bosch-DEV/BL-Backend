using LauncherBackEnd.net;
using System.Diagnostics;
using System.IO.Compression;

namespace LauncherBackEnd.game;
public class Game(string internalName, string displayName, string description, string backgroundImagePath, string version, string execPath, string flags) {

    public string internalName = internalName, displayName = displayName, description = description, backgroundImagePath = backgroundImagePath, version = version, execPath = execPath, flags = flags;

    public string? GetInternalName() => internalName;
    public string? GetDisplayName() => displayName;
    public string? GetDescription() => description;
    public string? GetBackgroundImagePath() => backgroundImagePath;
    public string? GetVersion() => version;
    public string? GetExecPath() => execPath;

    public string? GetFlags() => flags;

    private string dir = $"{Environment.SpecialFolder.ApplicationData.ToString()}/BattleLauncher/Games/{internalName}/";
    private string dirN = $"{Environment.SpecialFolder.ApplicationData.ToString()}/BattleLauncher/Games/{internalName}{(execPath.StartsWith("/") ? execPath : "/" + execPath)}";


    public override string ToString() {
        return $"internalName: {internalName}, displayName: {displayName}, description: {description}, backgroundImagePath: {backgroundImagePath}, version: {version}, execPath: {execPath}, flags: {flags}";
    }

    public async Task<bool> Download(string webURI) {
        Main.WriteLine("Downloading: " + webURI);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        if (await NetHandler.GetInstance().DownLoadAsync(webURI, $"{dir + this.internalName}.zip") == false) return false;
        ZipFile.ExtractToDirectory($"{dir + this.internalName}.zip", dir, true);
        File.Delete($"{dir + this.internalName}.zip");
        return true;
    }

    public Game Start() {
        LaunchCMD();
        return this;
    }

    private void LaunchCMD() {
        ProcessStartInfo info = new() {
            CreateNoWindow = false,
            UseShellExecute = false,
            FileName = $"{Environment.SpecialFolder.ApplicationData.ToString()}/BattleLauncher/Games/{this.internalName}{(execPath.StartsWith("/") ? execPath : "/" + execPath)}",
            WindowStyle = ProcessWindowStyle.Maximized,
            Arguments = flags
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

