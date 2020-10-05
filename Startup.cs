using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorDesktopDemo.Data;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace BlazorDesktopDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)

        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            if(HybridSupport.IsElectronActive){
                ElectronBootstrap();
            }         
            
        }
        void ElectronBootstrap(){
         Task.Run(async () => await Electron.WindowManager.CreateWindowAsync());

         var menu = new MenuItem[] 
         {
            new MenuItem 
            { 
                    Label = "Edit", Type = MenuType.submenu, Submenu = new MenuItem[] 
                    {
                            new MenuItem { Label = "Undo", Accelerator = "CmdOrCtrl+Z", Role = MenuRole.undo },
                             new MenuItem { Label = "Redo", Accelerator = "Shift+CmdOrCtrl+Z", Role = MenuRole.redo },
                             new MenuItem { Type = MenuType.separator },
                         new MenuItem { Label = "Cut", Accelerator = "CmdOrCtrl+X", Role = MenuRole.cut },
                         new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy },
                         new MenuItem { Label = "Paste", Accelerator = "CmdOrCtrl+V", Role = MenuRole.paste },
                         new MenuItem { Label = "Select All", Accelerator = "CmdOrCtrl+A", Role = MenuRole.selectall }
                    }
            },
            new MenuItem { Label = "Ansicht", Type = MenuType.submenu, Submenu = new MenuItem[] 
            {
                 new MenuItem
                    {
            Label = "Reload",
            Accelerator = "CmdOrCtrl+R",
            Click = () =>
            {
                // on reload, start fresh and close any old
                // open secondary windows
                Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow => {
                    if(browserWindow.Id != 1)
                    {
                        browserWindow.Close();
                    }
                    else
                    {
                        browserWindow.Reload();
                    }
                });
            }
        },
        new MenuItem
        {
            Label = "Toggle Full Screen",
            Accelerator = "CmdOrCtrl+F",
            Click = async () =>
            {
                bool isFullScreen = await Electron.WindowManager.BrowserWindows.First().IsFullScreenAsync();
                Electron.WindowManager.BrowserWindows.First().SetFullScreen(!isFullScreen);
            }
        },
        new MenuItem
        {
            Label = "Open Developer Tools",
            Accelerator = "CmdOrCtrl+I",
            Click = () => Electron.WindowManager.BrowserWindows.First().WebContents.OpenDevTools()
        },
        
    }
    },
    new MenuItem { Label = "Window", Role = MenuRole.window, Type = MenuType.submenu, Submenu = new MenuItem[] {
            new MenuItem { Label = "Minimize", Accelerator = "CmdOrCtrl+M", Role = MenuRole.minimize },
            new MenuItem { Label = "Close", Accelerator = "CmdOrCtrl+W", Role = MenuRole.close },
            new MenuItem
             {
            Label = "Beenden",
            Accelerator = "CmdOrCtrl+X",
            Click = () => Electron.App.Exit(0)
            }
    }
    }
    
};

Electron.Menu.SetApplicationMenu(menu);
        }
    }
    

    
}
