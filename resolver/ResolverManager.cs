namespace LauncherBackEnd.resolver;
public class ResolverManager {

    private Dictionary<string, DefaultResolver> registeredResolvers;

    private static ResolverManager instance;

    public bool hasLoaded;
    public bool failedLoad;

    private ResolverManager() {
        instance = this;
        registeredResolvers = [];
    }

    public static ResolverManager GetInstance() => instance ?? new ResolverManager();

    public async Task<bool> RegisterAll() {
        RegisterResolver(new LocalResolver());
        RegisterResolver(new RemoteResolver());
        foreach(var res in GetResolvers()) {
            await res.LoadGames();
        }
        return true;
    }

    public bool RegisterResolver(DefaultResolver resolver) {
        if (IsRegistered(resolver.id)) return false;
        if (!resolver.isValid) return false;
        registeredResolvers.Add(resolver.id, resolver);
        return true;
    }

    public bool UnregisterResolver(DefaultResolver resolver) {
        if (!IsRegistered(resolver.id)) return false;
        registeredResolvers.Remove(resolver.id);
        return true;
    }

    public List<DefaultResolver> GetResolvers() {
        return [.. registeredResolvers.Values];
    }

    public DefaultResolver? GetResolver(string id) {
        if (!IsRegistered(id)) return null;
        return registeredResolvers[id];
    }

    public bool IsRegistered(string id) {
        return registeredResolvers.ContainsKey(id);
    }
}
