using LauncherBackEnd.net;
using LauncherBackEnd.resolver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LauncherBackEnd.game;

public class GameHandler {

    private readonly Dictionary<string, Game> loadedGames;
    private static GameHandler instance;

    private GameHandler() {
        instance = this;
        loadedGames = [];
    }

    public static GameHandler GetInstance() => instance ?? new GameHandler();

    public async Task<Dictionary<string, Game>> ReloadGames() {
        loadedGames.Clear();
        foreach (var resolver in ResolverManager.GetInstance().GetResolvers()) {
            if (!resolver.isValid) continue;
            foreach (DefaultResolver.GameResolverData item in resolver.resolverData.Values) {
                JObject res = await NetHandler.GetInstance().ScrapeWebsiteAsyncJS(item.resvL);
                Main.WriteLine(res.ToString());
                RegisterGame(new Game("tests", "TEstDisplay", "Description very nice", "https://github.com/Bosch-DEV/content/bg.png", "/content/bg.png", "0.1.0", "https://github.com/Bosch-DEV/exec.exe", "/execute/Crosshair Overlay.exe", "Crosshair Overlay.exe"));
            }
        }
        return loadedGames;
    }

    public bool IsGameRegistered(string id) {
        id = CleanID(id);
        return loadedGames.ContainsKey(id);
    }

    public Game? GetGame(string id) {
        id = CleanID(id);
        if(loadedGames.ContainsKey(id)) {
            return loadedGames[id];
        }
        return null;
    }

    public bool UnRegisterGame(string id) {
        id = CleanID(id);
        if (IsGameRegistered(id)) return false;
        loadedGames.Remove(id);
        return true;
    }

    public Game? RegisterGame(Game game, [Optional] string id) {
        id = CleanID(id ?? game.GetInternalName());
        if (loadedGames.ContainsKey(id)) {
            Main.WriteLine($"Couldn't register {id}, Already Registered", textColor: ConsoleColor.Red);
            return loadedGames[id];
        }
        loadedGames.Add(id, game);
        return game;

    }

    private string CleanID(string id) {
        if (id == null) return "";
        return id.Replace(" ", "");
    }

}
