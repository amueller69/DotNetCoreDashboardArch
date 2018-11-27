using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Web;

namespace DotNetCoreDashboardArch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                BuildHost(args).Run();
            }
            catch (Exception e)
            {
                logger.Error(e, "Terminating application");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHost BuildHost(string[] args)
        {
            IWebHost host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
            return host;
        }

    }
}
