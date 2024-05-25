﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XProxy.Services;

[assembly: AssemblyVersion("1.0.5")]

namespace XProxy
{
    class Program
    {
        public class Options
        {
            [Option('p', "path", Required = false)]
            public string Path { get; set; }
        }

        static async Task Main(string[] args) => await RunApplication(BuildApplication(args));
        
        static HostApplicationBuilder BuildApplication(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                ConfigService.MainDirectory = o.Path.Trim();
            });

            if (ConfigService.MainDirectory == null)
                ConfigService.MainDirectory = Environment.CurrentDirectory;

            Logger.Ansi = new AnsiVtConsole.NetCore.AnsiVtConsole();

            var builder = Host.CreateApplicationBuilder();

            builder.Logging.SetMinimumLevel(LogLevel.None);

            SetupServices(builder.Services);

            return builder;
        }

        static void SetupServices(IServiceCollection services)
        {
            services.AddSingleton<ConfigService>(); 
            services.AddHostedService<ProxyService>();
            services.AddHostedService<PublicKeyService>();
            services.AddHostedService<ListService>();
            services.AddHostedService<ClientsUpdaterService>();
            services.AddSingleton<PluginsService>();
            services.AddHostedService<CommandsService>();
        }

        static async Task RunApplication(HostApplicationBuilder app)
        {
            IHost host = app.Build();

            await host.RunAsync();
        }
    }
}