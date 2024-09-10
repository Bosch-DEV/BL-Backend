using LauncherBackEnd.net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LauncherBackEnd.resolver;
public abstract class DefaultResolver {

    JObject? content;

    public string id { get; }
    public bool isValid = true;
    public bool isNet { get; }
    public string path { get; }

    public Dictionary<string, GameResolverData> resolverData;

    public DefaultResolver(string path, string id, bool net = false) {
        this.path = path.Replace("%ENV_PROGDATA%", Environment.SpecialFolder.ApplicationData.ToString());
        this.id = id;
        this.isNet = net;
        resolverData = [];
        Main.WriteLine(this.ToString() + " INIT");
        if(!net && !File.Exists(path)) {
            File.WriteAllText(this.path, "");
        }
    }

    private async Task<bool> LoadContent() {
        Main.WriteLine(this.ToString() + " LOAD CONTENT");
        if(isNet) {
            Main.WriteLine(this.ToString() + " NET");
            content = await NetHandler.GetInstance().ScrapeWebsiteAsyncJS(path);
            return isValid = true;
        } else {
            try {
                Main.WriteLine(this.ToString() + " NONET");
                content = (JObject) JsonConvert.DeserializeObject(path);
                return isValid = true;
            } catch(Exception ex) {
                Main.WriteLine($"Failed to read resolver at: {path}", ConsoleColor.Red);
                Main.WriteLine(ex.Message, ConsoleColor.Red);
                return isValid = false;
            }
        }
    }

    public async Task<bool> LoadGames() {
        _ = await LoadContent();
        if (!isValid) {
            return false;
        }
        foreach (var item in content) {
            JObject obj = item.Value.ToObject<JObject>();
            resolverData.Add(item.Key, new GameResolverData(item.Key, obj.GetValue("exec").ToString(), obj.GetValue("data").ToString()));
        }
        return true;
    }

    //TODO: Fix these names
    public class GameResolverData(string id, string execL, string resvL) {

        public string id { get; } = id;
        public string execL { get; } = execL;
        public string resvL { get; } = resvL;

        public override string ToString() {
            return $"ID: {id}, X: {execL}, Y: {resvL}";
        }
    }

}
