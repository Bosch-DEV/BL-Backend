using LauncherBackEnd.config;
using LauncherBackEnd.game;
using LauncherBackEnd.net;
using LauncherBackEnd.resolver;
using System.Security;

namespace LauncherBackEnd;

public class Main {

    private static Main? instance;
    private readonly SettingHandler settingHandler;
    public SecureString pass = null;
    public bool hasPinned = false;
    public bool lastReqFail = true;
    public Settings Settings { get; }

    public bool isRunningProxy;

    public string? ProxyHost { get; }

    internal Main() {
        instance = this;
        settingHandler = new SettingHandler();
        Settings = settingHandler.HandleReadWrite();
        NetHandler.ProxyReturn proxyresult = NetHandler.GetInstance().CheckProxy();
        isRunningProxy = proxyresult.UsingProxy;
        ProxyHost = proxyresult.Proxy;
        HandleProxy();
        HandleAsyncTasks();
    }

    public async Task<bool> HandleAsyncTasks() {
        await ResolverManager.GetInstance().RegisterAll();
        await GameHandler.GetInstance().ReloadGames();
        GameHandler.GetInstance().GetRegisteredGames().ForEach(game => {
            Console.WriteLine(game.ToString());
        } );
        await GameHandler.GetInstance().DownloadGames();
        /*
        try {
            await File.WriteAllTextAsync(Settings.resvPath.Replace("%ENV_PROGDATA%", Environment.SpecialFolder.ApplicationData.ToString()), (await NetHandler.GetInstance().ScrapeMasterResolver()).ToString());
            Console.WriteLine(await NetHandler.GetInstance().ScrapeMasterResolver());
            return true;
        } catch (NullReferenceException ex) {
            Main.WriteLine($"Failed to write to local remote resolver:", ConsoleColor.Red);
            Main.WriteLine($"{ex.Message}", ConsoleColor.Red);
            return false;
        }
        */
        return false;
    }

    public static Main GetInstance() => instance ?? new Main();

    public static string GetValue(string message, bool hideResponse = false, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor textColor = ConsoleColor.White) {
        ConsoleColor tColorO = Console.ForegroundColor;
        ConsoleColor bgColorO = Console.BackgroundColor;
        Console.ForegroundColor = textColor;
        Console.BackgroundColor = backgroundColor;
        Console.Write($"{message}");
        var response = string.Empty;
        ConsoleKey key;
        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && response.Length > 0) {
                Console.Write("\b \b");
                response = response[0..^1];
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                if (hideResponse) {
                    Console.Write("*");
                } else {
                    Console.Write(keyInfo.KeyChar);
                }
                response += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        //Console.WriteLine("");
        Console.ForegroundColor = tColorO;
        Console.BackgroundColor = bgColorO;
        return response;
    }

    private bool HandleProxy() {
        if (isRunningProxy && !Settings.uP) {
            WriteBlock(62, "!", ConsoleColor.Red, addSpaceUnder: true);
            WriteLine("!!!        THIS SYSTEM IS USING A SYSTEM WIDE PROXY        !!!", textColor: ConsoleColor.Red);
            WriteLine("!!! YOU HAVE DISABLED THE USE PROXY OPTION IN YOUR CONFIG  !!!", textColor: ConsoleColor.Red);
            WriteLine("!!! THIS APPLICATION, WILL NOT BE ABLE TO CACHE NEW GAMES  !!!", textColor: ConsoleColor.Red);
            WriteLine("!!! OR DOWNLOAD ANYTHING! PLEASE BE SURE TO ADD YOUR PROXY !!!", textColor: ConsoleColor.Red);
            WriteLine("!!!           SETTINGS IN THE CONFIGURATION FILE           !!!", textColor: ConsoleColor.Red);
            WriteLine("!!!                      CURRENT PROXY                     !!!", textColor: ConsoleColor.Red);
            WriteLine($"!!!    {ProxyHost}", textColor: ConsoleColor.Red);
            WriteBlock(62, "!", ConsoleColor.Red);
            WriteBlank();
            WriteBlank();
            WriteBlock(70, "!", ConsoleColor.Red, addSpaceUnder: true);
            WriteLine("!!!   AUTO-APPLY PROXY SETTINGS TO THE CONFIGURATION AND ENABLE?   !!!", textColor: ConsoleColor.Red);
            Write("!!!                          ", ConsoleColor.Red);
            string value = GetValue("y/N: ", textColor: ConsoleColor.White);
            if (value.Equals("y", StringComparison.CurrentCultureIgnoreCase)) {
                WriteLine("", textColor: ConsoleColor.Red);
                WriteLine("!!!                     Applying Configuration                    !!!", textColor: ConsoleColor.Red);
                WriteBlock(70, "!", ConsoleColor.Red, addSpaceUnder: true);
                Settings.uP = true;
                Settings.pURL = ProxyHost;
                settingHandler.HandleReadWrite();
                settingHandler.ForceWrite(Settings);
                return false;
            } else {
                WriteLine("", textColor: ConsoleColor.Red);
                WriteLine("!!!                     Exiting Running Process                    !!!", textColor: ConsoleColor.Red);
                WriteBlock(70, "!", ConsoleColor.Red, addSpaceUnder: true);
                Runner.StopRunner();
                return false;
            }
        }
        return true;
    }

    public static void WriteBlock(int length, string repeating, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool addSpaceUnder = true) {
        ConsoleColor tColorO = Console.ForegroundColor;
        ConsoleColor bgColorO = Console.BackgroundColor;
        for (int i = 0; i < length; i++) {
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(repeating);
        }
        Console.ForegroundColor = tColorO;
        Console.BackgroundColor = bgColorO;
        if (addSpaceUnder) Console.WriteLine("");
    }

    public static void WriteLine(string text, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool reset = true) {
        ConsoleColor tColorO = Console.ForegroundColor;
        ConsoleColor bgColorO = Console.BackgroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = textColor;
        Console.WriteLine(text);
        if (reset) {
            Console.ForegroundColor = tColorO;
            Console.BackgroundColor = bgColorO;
        }
    }

    public static void Write(string text, ConsoleColor textColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black, bool reset = true) {
        ConsoleColor tColorO = Console.ForegroundColor;
        ConsoleColor bgColorO = Console.BackgroundColor;
        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = textColor;
        Console.Write(text);
        if (reset) {
            Console.ForegroundColor = tColorO;
            Console.BackgroundColor = bgColorO;
        }
    }

    public static void WriteBlank() {
        Console.WriteLine("");
    }
}
