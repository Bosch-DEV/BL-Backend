namespace LauncherBackEnd;

public class Runner {

    private static bool running = true;
    private static Thread? keepAliveThread = null;

    private static void Yes() {
        while (running) {
            _ = 1;
        }
    }

    public static void KeepAliveMultiThreaded() {
        keepAliveThread = new Thread(() => {
            Thread.CurrentThread.IsBackground = false;
            Yes();
        });
        keepAliveThread.Start();
    }

    public static void StopRunner() {
        running = false;
        keepAliveThread?.Join();
    }
}