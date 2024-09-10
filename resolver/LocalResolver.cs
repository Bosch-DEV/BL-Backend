namespace LauncherBackEnd.resolver;
public class LocalResolver : DefaultResolver {
    public LocalResolver() : base(Main.GetInstance().Settings.lResv, "local", false) {}
}
