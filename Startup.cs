using Funq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Desktop;

namespace win32
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseServiceStack(new AppHost {
                AppSettings = new NetCoreAppSettings(Configuration),
            });
        }

        public class Hello : IReturn<Hello>
        {
            public string Name { get; set; }
        }
        public class MyServices : Service
        {
            public object Any(Hello request) => request;
        }

        public class AppHost : AppHostBase
        {
            public AppHost() 
                : base(nameof(win32), typeof(MyServices).Assembly) {}
            
            public override void Configure(Container container)
            {
                SetConfig(new HostConfig {
                    DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), HostingEnvironment.IsDevelopment()),
                });
                
                if (Config.DebugMode)
                {
                    Plugins.Add(new HotReloadFeature {
                        VirtualFiles = VirtualFiles, //Monitor all folders for changes including /src & /wwwroot
                    });
                }
                
                Plugins.Add(new SharpPagesFeature {
                    EnableSpaFallback = true,
                });
                
                Plugins.Add(new DesktopFeature {
                    AppName = "win32",
                    AccessRole = RoleNames.AllowAnon,
                });
            }
        }
    }
}