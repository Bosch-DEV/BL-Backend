using System;
using System.Collections.Generic;
using System.Linq;
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

    public Dictionary<string, Game> ReloadGames() {
        return loadedGames;
    }

    public bool IsGameRegistered(string id) {
        return loadedGames.ContainsKey(id);
    }

    public Game? GetGame(string id) {
        if(loadedGames.ContainsKey(id)) {
            return loadedGames[id];
        }
        return null;
    }

    public bool UnRegisterGame(string id) {
        if (IsGameRegistered(id)) return false;
        loadedGames.Remove(id);
        return true;
    }

    public Game? RegisterGame(string id) {
        if (loadedGames.ContainsKey(id)) {
            Main.WriteLine($"Couldn't register {id}, Already Registered", textColor: ConsoleColor.Red);
            return loadedGames[id];
        }
        return null;

    }

}
