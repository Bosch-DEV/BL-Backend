using LauncherBackEnd.config;
using LauncherBackEnd.net;

namespace LauncherBackEnd;

public class Main {

    private static Main? instance;
    private readonly SettingHandler settingHandler;
    public Settings Settings { get; }

    public bool isRunningProxy;

    internal Main() {
        instance = this;
        settingHandler = new SettingHandler();
        Settings = settingHandler.HandleReadWrite();
        NetHandler.ProxyReturn proxyresult = NetHandler.GetInstance().CheckProxy();
        isRunningProxy = proxyresult.UsingProxy;
        if (isRunningProxy && !Settings.uP) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("!!!        THIS SYSTEM IS USING A SYSTEM WIDE PROXY        !!!");
            Console.WriteLine("!!! YOU HAVE DISABLED THE USE PROXY OPTION IN YOUR CONFIG  !!!");
            Console.WriteLine("!!! THIS APPLICATION, WILL NOT BE ABLE TO CACHE NEW GAMES  !!!");
            Console.WriteLine("!!! OR DOWNLOAD ANYTHING! PLEASE BE SURE TO ADD YOUR PROXY !!!");
            Console.WriteLine("!!!           SETTINGS IN THE CONFIGURATION FILE           !!!");
            Console.WriteLine("!!!                      CURRENT PROXY                     !!!");
            Console.WriteLine($"!!!    {proxyresult.Proxy}");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.ForegroundColor = ConsoleColor.White;
        }
        HandleAsyncTasks();
    }

    public async void HandleAsyncTasks() {
        Console.WriteLine(await NetHandler.GetInstance().ScrapeMasterResolver());
    }

    public static Main GetInstance() => instance ?? new Main();
}
