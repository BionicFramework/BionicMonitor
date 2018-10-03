using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BionicMonitorService.Utils {
  public static class BrowserUtils {
    public static Process OpenUrl(string url) {
      try {
        return Process.Start(url);
      }
      catch {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
          url = url.Replace("&", "^&");
          return Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
          return Process.Start("xdg-open", url);
        }
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
          return Process.Start("open", url);
        }
      }

      return null;
    }
  }
}