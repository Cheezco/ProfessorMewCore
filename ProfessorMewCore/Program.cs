using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using ProfessorMewCore.Services;
using Discord.Addons.Interactive;

namespace ProfessorMewCore
{
    class Program
    {
        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = (Discord.GatewayIntents)0b_0011_0111_1000_0011,
                AlwaysDownloadUsers = true,
                ExclusiveBulkDelete = true,
            };

            using(var services = ConfigureServices(config))
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                services.GetRequiredService<LoggingService>().Initialize();
                await services.GetRequiredService<UserPrivilegeService>().InitializeAsync();
                await services.GetRequiredService<GuildService>().InitializeAsync();

                await client.LoginAsync(Discord.TokenType.Bot, System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Tkn.txt"));
                await client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private static ServiceProvider ConfigureServices(DiscordSocketConfig config)
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(config))
                .AddSingleton<InteractiveService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<LoggingService>()
                .AddSingleton<ServerProtectionService>()
                .AddSingleton<UserPrivilegeService>()
                .AddSingleton<GuildService>()
                .BuildServiceProvider();
        }
    }
}
