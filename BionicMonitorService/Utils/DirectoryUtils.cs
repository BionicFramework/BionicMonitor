using System.IO;

namespace BionicMonitorService.Utils {
  public static class DirectoryUtils {
    public static void Copy(string sourceDirectory, string targetDirectory) =>
      CopyAll(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
    
    private static void CopyAll(DirectoryInfo source, DirectoryInfo target) {
      Directory.CreateDirectory(target.FullName);

      foreach (var info in source.GetFiles()) {
        info.CopyTo(Path.Combine(target.FullName, info.Name), true);
      }

      foreach (var subDir in source.GetDirectories()) {
        CopyAll(subDir, target.CreateSubdirectory(subDir.Name));
      }
    }
  }
}