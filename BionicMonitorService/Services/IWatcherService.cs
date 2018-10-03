using System.Threading.Tasks;

namespace BionicMonitorService.Services {
  public interface IWatcherService {
    Task Init();
    void OpenPage(string url);
  }
}