using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace LauncherBackEnd.config;

public abstract class Config<T>(string configName, [Optional] string configPath) where T : class, new() {
    protected string path = configPath ?? Environment.SpecialFolder.ApplicationData + "/BattleLauncher/";
    protected string name = configName;

    protected abstract T GenerateDefault();

    public T HandleReadWrite() {
        T configData;

        if (File.Exists(Path.Combine(path, name))) {
            string jsonContent = File.ReadAllText(Path.Combine(path, name));
            try {
                configData = JsonConvert.DeserializeObject<T>(jsonContent);
            } catch (JsonException) {
                Console.WriteLine("Error reading JSON, generating default configuration.");
                configData = GenerateDefault();
            }
        } else {
            configData = GenerateDefault();
        }

        WriteConfig(configData);
        return configData;
    }

    protected void WriteConfig(T configData) {
        string jsonText = JsonConvert.SerializeObject(configData, Formatting.Indented);
        File.WriteAllText(Path.Combine(path, name), jsonText);
        Console.WriteLine(jsonText);
    }
}
