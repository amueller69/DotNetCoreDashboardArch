using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using DotNetCoreDashboardArch.Services;
using DotNetCoreDashboardArch.Middleware;
using DotNetCoreDashboardArch.Services.Repositories;

namespace DotNetCoreDashboardArch
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _loggerFactory = loggerFactory;
            if (env.IsDevelopment())
            {
                _loggerFactory.AddDebug();
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.Cookie.Name = "Session";
            });
            services.AddSingleton<ICacheDependency, CacheDependency>(_ => new CacheDependency(Configuration.GetConnectionString("DefaultConnection"), _loggerFactory));
            services.AddSingleton<ICacheProvider, CacheProvider>(_ => new CacheProvider(_loggerFactory));
            services.AddTransient<IDBContext, DBContext>(_ => new DBContext(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IHomeRepository, HomeRepository>(_ => new HomeRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IReportsRepository, ReportsRepository>(_ => new ReportsRepository(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            _loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseErrorHandlerMiddleware();
            }
            else
            {
                app.UseErrorHandlerMiddleware();
                app.UseExceptionHandler();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "scoped",
                    template: "{controller=Home}/{action=Main}/{scope:regex(^[\\d]?[A-Za-z]{{3,8}})}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Main}/");
            });
        }
    }
}
