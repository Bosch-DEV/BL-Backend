namespace LauncherBackEnd.config;

public class Settings {

    /* Overwrite Server File
     * true = Overwrites server File, not retaining anything from before
     * false = Attempts to append to existing Server File
     */
    public bool owSF { get; set; } = true;

    /* Use proxy
     * true = uses the proxy URL, the local account name, and prompts for Password
     * false = connects normally, without passing proxy information
     */
    public bool uP { get; set; } = true;

    /* Proxy Username
     * The username that is passed to the proxy if enabled
     * Use %ENV_NAME% to use the user that is currently logged in
     */
    public string pN { get; set; } = "USERNAME";

    /* Proxy URL
     * The URL the Username and password get Passed to, if enabled
     */
    public string pURL { get; set; } = "http://your.proxy.server:8080";

    /* The Main Resolver URL
     * This resolver Contains the Games, and where there resolver.resv + .exe is located and is pulled from 
     */
    public string resvURL { get; set; } = "https://raw.githubusercontent.com/Bosch-DEV/Battle-Launcher/master/resolver.resv";

    /* Fallback Resolver URL
     * Incase main resolver where to go down, this resolver, is running with a less updated version, but is far more secure and uptime should be 99.99%
     * If this resolver also goes down, either you dont have internet, or theres a mass DDOS attack on multiple networks.
     */
    public string fbResvURL { get; set; } = "https://img.bedless.dev/Bosch-DEV/resolver.resv";

    /* Local Resolver
     * Incase you want to add your own Games, You can add to your own Local Resolver.
     * You can also get games approved for the main resolver, but that might take a bit, so it's recommeded to add it to your local while you wait
     */
    public string lResv { get; set; } = "C:/Users/%user_name%/ProgramData/resolver.resv";
}
