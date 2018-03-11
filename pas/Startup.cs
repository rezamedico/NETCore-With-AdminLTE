using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataTables.AspNet.AspNetCore;
using LinqToDB.Data;
using LinqToDB.DataProvider.MySql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using pas.Interfaces;

namespace pas
{
    public class Startup
    {
        public IConfiguration _configuration { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {            
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            this._configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Connection Strings
            DataConnection
             .AddConfiguration(
                 "DefaultConnection",
                 _configuration["Data:DefaultConnection:ConnectionString"],
                 new MySqlDataProvider());

            DataConnection
             .AddConfiguration(
                 "ReadOnlyConnection",
                 _configuration["Data:ReadOnlyConnection:ConnectionString"],
                 new MySqlDataProvider());

            DataConnection.DefaultConfiguration = "DefaultConnection";


            // Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.Cookie.Name = "PasCookies";
                options.LoginPath = "/Login/Index";
                options.LogoutPath = "/Login/Logout";                
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(150);
            });            

            //Add session and set session expired
            services.AddSession(o => { o.IdleTimeout = new TimeSpan(0, 10, 0); });

            //Add Cache
            services.AddDistributedMemoryCache();

            //DataTables
            services.RegisterDataTables();

            //Protection
            services.AddDataProtection();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(sp => { return _configuration; });
            services.AddScoped<IPasSession, PasSessionImplementation>();


            // Add MVC services to the services container.
            services.AddMvc()
            .AddJsonOptions(opt => {
                //This AddJsonOptions method are used for make the json serializer write as is as the object from the controller
                //It's not serialize to camel case. For example: The object contain Foo ==> Good Day, it will become Foo: Good Day
                //Not become foo ==> Good Day
                var resolver = opt.SerializerSettings.ContractResolver;
                if (resolver != null)
                {
                    var res = resolver as DefaultContractResolver;
                    res.NamingStrategy = null;
                }
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //localization
            var options = new RequestLocalizationOptions
            {
                // Set options here to change middleware behavior
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US")

                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US")
                }
            };
            //app.UseRequestLocalization(options, defaultRequestCulture: new RequestCulture("en-US"));
            app.UseRequestLocalization(options);

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(_configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler("/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                //try
                //{
                //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                //        .CreateScope())
                //    {
                //        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                //             .Database.Migrate();
                //    }
                //}
                //catch { }

                //var certFile = _currentEnvirontment.ContentRootPath + "\\sslcert.pfx";//ApplicationEnvirontment.ApplicationBasePath + "\\sslcert.pfx";
                //var signingCertificate = new X509Certificate2(certFile, "&Medico#2017");

                ////app.Use(ChangeContextToHttps);
                //app.UseKestrelHttps(signingCertificate);
            }

            app.UseAuthentication();

            app.UseStatusCodePagesWithRedirects("~/Error/{0}");

            // Add static files to the request pipeline.
            app.UseStaticFiles();       
            app.UseSession();

            //loggerFactory.MinimumLevel = LogLevel.Information;
            //loggerFactory.AddConsole();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    //template: "{controller=Home}/{action=Index}/{id?}");
                //    template: "{controller=Account}/{action=Login}/{id?}");
                routes.MapRoute(
                    name: "areaRouting",
                    template: "{area:exists}/{controller}/{action}/{id?}/{category?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                   name: "wizardstep",
                   template: "{controller=Home}/{action=Index}/{step?}/{id?}");

                //routes.MapRoute(
                //    name: "configRouting",
                //    template: "{area:exists}/{controller=Config}/{action}/{category?}/{id?}");

                routes.MapRoute(
                    name: "errorRouting",
                    template: "Error/{id?}",
                    defaults: new { controller = "Error", action = "Index" });
                //constraints: new Routing.RootRouteConstraint<HomeController>());

            });
        }
    }
}
