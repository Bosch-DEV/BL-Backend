using LauncherBackEnd.net;
using LauncherBackEnd.resolver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                if(res == null) {
                    Main.WriteLine($"Failed to pull from: {item.resvL}", ConsoleColor.Red);
                    continue;
                }
                if(!res.ContainsKey("internal_name") ||
                    !res.ContainsKey("display_name") ||
                    !res.ContainsKey("version") ||
                    !res.ContainsKey("description") ||
                    !res.ContainsKey("execPath") ||
                    !res.ContainsKey("background_img") ||
                    !res.ContainsKey("startupFlags")) {
                    Main.WriteLine($"Invalid Structure, Missing Keys. Report this to the Developer {item.resvL}", ConsoleColor.Red);
                    continue;
                }
                _ = RegisterGame(new Game(
                    res.GetValue("internal_name").ToString(),
                    res.GetValue("display_name").ToString(),
                    res.GetValue("description").ToString(),
                    res.GetValue("background_img").ToString(),
                    res.GetValue("version").ToString(),
                    res.GetValue("execPath").ToString(),
                    res.GetValue("startupFlags").ToString()
                    ));
            }
        }
        return loadedGames;
    }

    public async Task<bool> DownloadGames() {
        foreach (var resolver in ResolverManager.GetInstance().GetResolvers()) {
            if (!resolver.isValid) continue;
            foreach (DefaultResolver.GameResolverData item in resolver.resolverData.Values) {
                try {
                    if (GetGame(item.id) == null) continue;
                    GetGame(item.id).Download(item.execL);
                } catch (Exception ex) {
                    Main.WriteLine($"{ex.Message}");
                }
            }
        }
        return true;
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

    public List<Game> GetRegisteredGames() {
        return loadedGames.Values.ToList();
    }

    public Dictionary<string, Game> GetAllRegisteredGames() {
        return loadedGames; 
    }

    private string CleanID(string id) {
        if (id == null) return "";
        return id.Replace(" ", "");
    }

}
