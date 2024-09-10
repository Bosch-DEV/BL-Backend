using LauncherBackEnd.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherBackEnd.resolver;
public abstract class DefaultResolver {

    JObject? content;
    public bool isValid = true;
    public bool isNet { get; }
    public string path { get; }

    public Dictionary<string, GameResolverData> resolverData;

    public DefaultResolver(string path, bool net = false) {
        this.path = path.Replace("%ENV_PROGDATA%", Environment.SpecialFolder.ApplicationData.ToString());
        this.isNet = net;
        LoadGames();
    }

    private async Task<bool> LoadContent() {
        if(isNet) {
            content = await NetHandler.GetInstance().ScrapeWebsiteAsyncJS(path);
            return isValid = true;
        } else {
            try {
                content = (JObject) JsonConvert.DeserializeObject(path);
                return isValid = true;
            } catch(Exception ex) {
                Main.WriteLine($"Failed to read resolver at: {path}", ConsoleColor.Red);
                Main.WriteLine(ex.Message, ConsoleColor.Red);
                return isValid = false;
            }
        }
    }

    private async void LoadGames() {
        _ = await LoadContent();
        if (!isValid) return;
        foreach (var item in content) {
            Main.WriteLine(item.ToString(), ConsoleColor.Yellow);
        }
    }

    //TODO: Fix these names
    public class GameResolverData(string id, string x, string y) {

        public string id { get; } = id;
        public string x { get; } = x;
        public string y { get; } = y;
    }

}
