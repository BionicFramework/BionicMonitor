using System.IO;
using Newtonsoft.Json;

namespace BionicMonitorService.Options {
  public class BionicMonitorOptions {
    public static string[] args;
    public static bool UseBionic { get; set; }
    public static bool IsSignalRProvided { get; set; }
    public static string HostOrIp { get; set; } = "localhost";
    public static string Port { get; set; } = "5000";
    public static string SecurePort { get; set; } = "5001";
    public static string DestinationRootDir { get; set; } = "wwwroot";
    public static string SourceRootDir { get; set; } = "wwwroot";

    public static void LoadOptions() {
      var jsonFile = "biomon.json";
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), jsonFile);
      if (!File.Exists(fullPath)) {
        jsonFile = $".bionic/{jsonFile}";
        fullPath = Path.Combine(Directory.GetCurrentDirectory(), jsonFile);
        if (!File.Exists(fullPath)) {
          return;
        }
      }

      using (var s = new StreamReader(fullPath)) {
        var json = s.ReadToEnd();
        dynamic item = JsonConvert.DeserializeObject(json);
        IfItemExists(() => UseBionic = item.useBionic ?? UseBionic);
        IfItemExists(() => IsSignalRProvided = item.isSignalRProvided ?? IsSignalRProvided);
        IfItemExists(() => HostOrIp = item.hostOrIp ?? HostOrIp);
        IfItemExists(() => Port = item.port ?? Port);
        IfItemExists(() => SecurePort = item.securePort ?? SecurePort);
        IfItemExists(() => DestinationRootDir = item.destinationRootDir ?? DestinationRootDir);
        IfItemExists(() => SourceRootDir = item.sourceRootDir ?? SourceRootDir);
      }
    }

    private static bool IfItemExists<T>(GetValue<T> getValue) {
      try {
        getValue();
        return true;
      }
      catch {
        return false;
      }
    }

    private delegate T GetValue<out T>();
  }
}