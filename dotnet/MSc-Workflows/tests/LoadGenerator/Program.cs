﻿using System.Threading;
using System.Threading.Tasks;
using LoadGenerator;
using Microsoft.Extensions.Configuration;

namespace ManualTestingProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            await new Worker(config).ExecuteAsync(CancellationToken.None);
        }
    }
}