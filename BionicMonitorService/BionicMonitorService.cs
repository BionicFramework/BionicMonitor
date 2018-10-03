using System.IO;
using BionicMonitorService.Hubs;
using BionicMonitorService.Options;
using BionicMonitorService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace BionicMonitorService {
  public static class BMSInitializer {
    private static IWatcherService _watcherService;

    public static void AddBionicMonitorService(this IServiceCollection services) =>
      services.AddSingleton<IWatcherService, WatcherService>();

    public static IApplicationBuilder UseBionicMonitor(
      this IApplicationBuilder app,
      IApplicationLifetime applicationLifetime
    ) {
      var rootPath = Path.Combine(Directory.GetCurrentDirectory(), BionicMonitorOptions.DestinationRootDir);

      app.UseStaticFiles(new StaticFileOptions {
        FileProvider = new PhysicalFileProvider(rootPath),
        ContentTypeProvider = CreateContentTypeProvider(),
        OnPrepareResponse = SetCacheHeaders
      });

      app.MapWhen(IsNotFrameworkOrBionicDir, childAppBuilder => {
        var destinationDir = Path.Combine(Directory.GetCurrentDirectory(), BionicMonitorOptions.DestinationRootDir);
        var staticFileOptions = new StaticFileOptions {
          FileProvider = new PhysicalFileProvider(destinationDir),
          OnPrepareResponse = SetCacheHeaders
        };
        childAppBuilder.UseSpa(spa => spa.Options.DefaultPageStaticFileOptions = staticFileOptions);
      });

      var distPath = Path.Combine(Directory.GetCurrentDirectory(), "bin/Debug/netstandard2.0/dist");
      if (Directory.Exists(distPath)) {
        app.UseStaticFiles(new StaticFileOptions {
          FileProvider = new PhysicalFileProvider(distPath),
          ContentTypeProvider = CreateContentTypeProvider(),
          OnPrepareResponse = SetCacheHeaders
        });
      }

      app.UseSignalR(routes => routes.MapHub<ReloadHub>("/reloadHub"));

      app.UseMvc(routes => routes.MapRoute(name: "scripts", template: "{controller=Scripts}"));

      _watcherService = app.ApplicationServices.GetService<IWatcherService>();
      applicationLifetime.ApplicationStarted.Register(ApplicationStarted);

      return app;
    }

    private static void ApplicationStarted() {
      _watcherService.Init();
      _watcherService.OpenPage($"http://{BionicMonitorOptions.HostOrIp}:{BionicMonitorOptions.Port}");
    }

    private static IContentTypeProvider CreateContentTypeProvider() =>
      new FileExtensionContentTypeProvider {
        Mappings = {
          {
            ".dll",
            "application/octet-stream"
          }, {
            ".mem",
            "application/octet-stream"
          }, {
            ".wasm",
            "application/wasm"
          }
        }
      };

    private static void SetCacheHeaders(StaticFileResponseContext ctx) {
      var typedHeaders = ctx.Context.Response.GetTypedHeaders();
      if (typedHeaders.CacheControl != null) return;
      typedHeaders.CacheControl = new CacheControlHeaderValue {
        NoCache = true
      };
    }

    private static bool IsNotFrameworkOrBionicDir(HttpContext context) {
      return !(context.Request.Path.StartsWithSegments((PathString) "/_framework")
               || context.Request.Path.StartsWithSegments((PathString) "/_content")
               || context.Request.Path.StartsWithSegments((PathString) "/reloadHub")
               || context.Request.Path.StartsWithSegments((PathString) "/bionic"));
    }
  }
}