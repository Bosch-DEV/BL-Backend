using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace LauncherBackEnd.net;

public class NetHandler {

    private static NetHandler? instance;

    private NetHandler() {
        instance = this;
    }

    public static NetHandler GetInstance() {
        return instance ?? new NetHandler();
    }

    public async Task<JObject> ScrapeMasterResolver() {
        return await ScrapeWebsiteAsyncJS(Main.GetInstance().Settings.resvURL);
    }

    public async Task<JObject> ScrapeWebsiteAsyncJS(string uri) {
        var t = await ScrapeWebSiteAsync(uri);
        return JObject.Parse(t);
    }

    public async Task<string> ScrapeWebSiteAsync(string uri) {
        Settings settings = Main.GetInstance().Settings;
        if (settings.uP) {
            Console.WriteLine("Enter password");
            string pass = GetPass();
            return await (await new HttpClient(handler: new HttpClientHandler {
                Proxy = new WebProxy {
                    Address = new Uri(settings.pURL),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                    userName: settings.pN.Equals("%ENV_NAME%", StringComparison.CurrentCultureIgnoreCase) ? Environment.UserName : settings.pN,
                    password: pass
                )
                }
            }, disposeHandler: true).GetAsync(uri)).Content.ReadAsStringAsync();
        } else {
            return await (await new HttpClient().GetAsync(uri)).Content.ReadAsStringAsync();
        }
    }

    private string GetPass() {
        var pass = string.Empty;
        ConsoleKey key;
        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0) {
                Console.Write("\b \b");
                pass = pass[0..^1];
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        Console.WriteLine("");
        return pass;
    }

    public ProxyReturn CheckProxy() {
        IWebProxy defaultProxy = WebRequest.DefaultWebProxy;
        ProxyReturn proxyReturn;
        if (defaultProxy != null) {
            Uri proxyUri = defaultProxy.GetProxy(new Uri("http://www.example.com"));
            if (proxyUri != null) {
                proxyReturn = new ProxyReturn(true, proxyUri.ToString());
            } else {
                proxyReturn = new ProxyReturn(false, "No system-wide proxy is being used.");
            }
        } else {
            proxyReturn = new ProxyReturn(false, "No proxy settings found.");
        }
        return proxyReturn == null ? new ProxyReturn(false) : proxyReturn;
    }

    public class ProxyReturn(bool usingProxy, [Optional] string proxy) {

        public bool UsingProxy { get; } = usingProxy;
        public string Proxy { get; } = proxy == null ? "404" : proxy;
    }
}
