using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.InteropServices;
using LauncherBackEnd.config;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Security;

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
        try {
            JObject obj = JObject.Parse(t);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return obj;
        } catch (JsonReaderException ex) {
            Main.WriteLine($"Failed to read scraped content: {t}", ConsoleColor.Red);
            Main.WriteLine($"{ex.Message}", ConsoleColor.Red);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return null;
        }
    }

    public async Task<bool> DownLoadAsync(string uri, string outDir) {
        Settings settings = Main.GetInstance().Settings;
        Main.WriteLine($"URI: {uri}");
        if (settings.uP) {
            Console.WriteLine("Enter password");
            string pass = GetPass();
            try {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await (await new HttpClient(handler: new HttpClientHandler {
                    Proxy = new WebProxy {
                        Address = new Uri(settings.pURL),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(
                        userName: settings.pN.Equals("%ENV_NAME%", StringComparison.CurrentCultureIgnoreCase) ? Environment.UserName : settings.pN,
                        password: pass
                    )
                    }
                }, disposeHandler: true).GetStreamAsync(uri)).CopyToAsync(new FileStream(outDir, FileMode.OpenOrCreate));
                return true;
            } catch (HttpRequestException ex) {
                HandleException(ex, settings);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return await DownLoadAsync(uri, outDir);
            } catch (InvalidOperationException ex) {
                HandleException(ex, settings);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return await DownLoadAsync(uri, outDir);
            }
        } else {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            await (await new HttpClient().GetStreamAsync(uri)).CopyToAsync(new FileStream(outDir, FileMode.OpenOrCreate));
            return true;
        }
    }

    public async Task<string> ScrapeWebSiteAsync(string uri) {
        Settings settings = Main.GetInstance().Settings;
        Main.WriteLine($"URI: {uri}");
        if (settings.uP) {
            Console.WriteLine("Enter password");
            string pass = GetPass();
            try {
                GC.Collect();
                GC.WaitForPendingFinalizers();
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
            } catch (HttpRequestException ex) {
                HandleException(ex, settings);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return await ScrapeWebSiteAsync(uri);
            } catch (InvalidOperationException ex) {
                HandleException(ex, settings);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return await ScrapeWebSiteAsync(uri);
            }
        } else {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return await (await new HttpClient().GetAsync(uri)).Content.ReadAsStringAsync();
        }
    }

    /* Work in Progress with Password Cache for session
    public async Task<string> ScrapeWebSiteAsync(string uri) {
        Settings settings = Main.GetInstance().Settings;
        Main.WriteLine($"URI: {uri}");

        if (settings.uP) {
            SecureString pass;
            if (Main.GetInstance().pass == null || Main.GetInstance().lastReqFail) {
                Console.WriteLine("Enter password");
                pass = GetPass();
                string response = Main.GetValue("Allow Caching Password in RAM as secure String for future Requests? y/n: ");
                Main.WriteBlank();

                if (response.ToLower() == "y") {
                    Main.GetInstance().pass = pass;
                    Main.GetInstance().lastReqFail = false;
                }
            } else {
                pass = Main.GetInstance().pass;
            }

            try {
                IntPtr ptr = IntPtr.Zero;
                NetworkCredential cred;
                try {
                    ptr = Marshal.SecureStringToGlobalAllocUnicode(pass);
                    string password = Marshal.PtrToStringUni(ptr);

                    cred = new NetworkCredential(
                        userName: settings.pN.Equals("%ENV_NAME%", StringComparison.CurrentCultureIgnoreCase) ? Environment.UserName : settings.pN,
                        password: password
                    );

                    pass.Dispose(); // Clear the SecureString after creating NetworkCredential
                } finally {
                    if (ptr != IntPtr.Zero) {
                        Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                    }
                }

                WebProxy prox = new WebProxy {
                    Address = new Uri(settings.pURL),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = cred
                };

                using (HttpClientHandler handler = new HttpClientHandler {
                    Proxy = prox,
                }) {
                    using (HttpClient client = new HttpClient(handler, true)) {
                        string val = await (await client.GetAsync(uri)).Content.ReadAsStringAsync();
                        return val;
                    }
                }
            } catch (HttpRequestException ex) {
                HandleException(ex, settings);
                return await ScrapeWebSiteAsync(uri);
            } catch (InvalidOperationException ex) {
                HandleException(ex, settings);
                return await ScrapeWebSiteAsync(uri);
            }
        } else {
            return await (await new HttpClient().GetAsync(uri)).Content.ReadAsStringAsync();
        }
    }
    */

    private void HandleException(Exception ex, Settings settings) {
        Main.WriteLine($"[DEBUG] Failed to send Proxy Data to server: {settings.pURL}", ConsoleColor.Red);
        Main.WriteLine($"[DEBUG] Failed with payload USER: {(settings.pN.Equals("%ENV_NAME%", StringComparison.CurrentCultureIgnoreCase) ? Environment.UserName : settings.pN)} | Pass: *********", ConsoleColor.Red);
        Main.WriteLine($"[DEBUG] {ex.Message}", ConsoleColor.Red);
        Main.WriteLine($"[DEBUG] {ex.StackTrace}", ConsoleColor.Red);
        Main.WriteLine("Invalid Password, try again", ConsoleColor.Red);
        Main.GetInstance().lastReqFail = true;
    }

    /* Work in Progress with Password Cache for session
    private SecureString GetPass() {
        SecureString secureString = new();
        ConsoleKey key;
        do {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && secureString.Length > 0) {
                Console.Write("\b \b");
                secureString.RemoveAt(secureString.Length - 1);
            } else if (!char.IsControl(keyInfo.KeyChar)) {
                Console.Write("*");
                secureString.AppendChar(keyInfo.KeyChar);
            }
        } while (key != ConsoleKey.Enter);
        Console.WriteLine("");
        return secureString;
    }
    */

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
