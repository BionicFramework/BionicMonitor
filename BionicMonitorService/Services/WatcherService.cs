using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BionicMonitorService.Options;
using BionicMonitorService.Utils;
using Microsoft.AspNetCore.SignalR.Client;

namespace BionicMonitorService.Services {
  public class WatcherService : IWatcherService {
    private static readonly Regex SignalRRegEx =
      new Regex(
        @"<script WARNING=""Injected by Bionic Monitor"" src=""https:\/\/unpkg\.com\/@aspnet\/signalr@\d?\.\d?\.\d?\/dist\/browser\/signalr\.min\.js""><\/script>",
        RegexOptions.Compiled);
    private static readonly Regex ReloaderRegEx= new Regex(
        @"<script WARNING=""Injected by Bionic Monitor"" src=""bionic/reloader.js""></script>",
        RegexOptions.Compiled);

    private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>(); 
    private HubConnection _connection;
    private Task _reloader;
    
    public async Task Init() {
      if (BionicMonitorOptions.UseBionic) {
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), BionicMonitorOptions.DestinationRootDir);
        if (!Directory.Exists(rootPath)) {
          Directory.CreateDirectory(rootPath);
        }
      }
      InitWatchers();
      await InitSignalR();
    }

    public void OpenPage(string url) {
      var browser = BrowserUtils.OpenUrl(url);
      browser?.WaitForExit();
    }

    private async Task Reload() {
      _reloader = null;
      await _connection.InvokeAsync("SendMessage", "reload");
    }

    private async Task InitSignalR() {
      _connection = new HubConnectionBuilder()
        .WithUrl($"http://{BionicMonitorOptions.HostOrIp}:{BionicMonitorOptions.Port}/reloadHub")
        .Build();

      try {
        await _connection.StartAsync();
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }

    private void InitWatchers() {
      InitWatcher("bin/Debug/netstandard2.0/dist");
      InitWatcher(BionicMonitorOptions.DestinationRootDir);
      var path = Path.Combine(Directory.GetCurrentDirectory(), $"{BionicMonitorOptions.DestinationRootDir}/index.html");
      InjectReloader(path);
    }

    private void InitWatcher(string relativePath) {
      var path = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
      if (!Directory.Exists(path)) return;
      
      var watcher = new FileSystemWatcher {
        Path = path,
        NotifyFilter = NotifyFilters.LastWrite,
        Filter = "*.*",
        EnableRaisingEvents = true,
        IncludeSubdirectories = true
      };
      watcher.Changed += OnChanged;

      _watchers.Add(watcher);
    }

    private void OnChanged(object source, FileSystemEventArgs e) {
      if (e.FullPath.EndsWith("___jb_tmp___")) return;
      if (e.FullPath.EndsWith(".html")) InjectReloader(e.FullPath);
      if (_reloader == null) {
        _reloader = Task.Delay(1000).ContinueWith(t => Reload());
      }
    }

    private void InjectReloader(string path) {
      Console.WriteLine($"Injecting in {path}");
      if (!File.Exists(path)) return;
      Console.WriteLine($"#Injecting in {path}");
      try {
        var modified = false;
        var all = File.ReadAllText(path);

        var matches = SignalRRegEx.Matches(all);
        if (matches.Count == 0 && !BionicMonitorOptions.IsSignalRProvided) {
          modified = true;
          all = all.Replace(
            "</head>",
            "<script WARNING=\"Injected by Bionic Monitor\" src=\"https://unpkg.com/@aspnet/signalr@1.0.3/dist/browser/signalr.min.js\"></script>\n</head>"
          );
        }

        matches = ReloaderRegEx.Matches(all);
        if (matches.Count == 0) {
          modified = true;
          all = all.Replace(
            "</head>",
            "<script WARNING=\"Injected by Bionic Monitor\" src=\"bionic/reloader.js\"></script>\n</head>"
          );
        }

        if (modified) {
          using (var file = new StreamWriter(File.Create(path))) {
            file.Write(all);
          }
        }
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }
  }
}