using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.ExcelApp.WS.FileCreateWorkerService.Models;
using RabbitMQ.ExcelApp.WS.FileCreateWorkerService.Services;
using System;

namespace RabbitMQ.ExcelApp.WS.FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration Configuration = hostContext.Configuration;


                    services.AddDbContext<AdventureWorks2019Context>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
                    });

                    services.AddSingleton<RabbitMQClientService>();
                    services.AddSingleton(sp => new ConnectionFactory()
                    {
                        Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")),
                        DispatchConsumersAsync = true
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
