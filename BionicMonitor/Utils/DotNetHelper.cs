using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Utils {
  public static class DotNetHelper {
    public static int RunDotNet(string cmd) {
      var watcher = Process.Start(DotNetExe.FullPathOrDefault(), cmd);
      watcher?.WaitForExit();
      return watcher?.ExitCode ?? 1;
    }
  }
}