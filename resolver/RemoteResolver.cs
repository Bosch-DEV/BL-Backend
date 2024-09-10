namespace LauncherBackEnd.resolver;
public class RemoteResolver : DefaultResolver {
    public RemoteResolver() : base(Main.GetInstance().Settings.resvURL, "remote", true) { }
}
