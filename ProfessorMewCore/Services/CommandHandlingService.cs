using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;
using ProfessorMewCore.Exceptions;
using ProfessorMewData.Contexts;
using ProfessorMewData.Models.Guild;
using ProfessorMewData.Interfaces.Guild;
using ProfessorMewData.Extensions.Guild;

namespace ProfessorMewCore.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private List<SavedGuild> _guilds;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                _guilds = await context.Guilds.ToListAsync();
            }
        }

        public async Task UpdateAsync()
        {
            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                _guilds = await context.Guilds.ToListAsync();
            }
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage message) return;
            if (message.Source != MessageSource.User) return;

            //await CheckProtection(rawMessage);

            var argPos = 0;

            if (!message.HasStringPrefix(GetPrefix(rawMessage.Channel), ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified || result.IsSuccess) return;

            if (result is ExecuteResult executeResult && executeResult.Exception is ProfessorMewException exception)
            {
                await context.Channel.SendMessageAsync(exception.Message);
            }
            else
            {
                await context.Channel.SendMessageAsync($"Error: ```{result}```");
            }
        }

        private string GetPrefix(ISocketMessageChannel channel)
        {
            string prefix = "!";
            if (channel is SocketGuildChannel guildChannel)
            {
                var guild = _guilds.GetGuild(guildChannel.Guild.Id);
                if (guild is not null)
                {
                    prefix = guild.Prefix;
                }
            }

            return prefix;
        }

        private async Task CheckProtection(SocketMessage rawMessage)
        {
            bool Isspam = _services.GetRequiredService<ServerProtectionService>().CheckForSpam(rawMessage.Author, rawMessage.Content);

            if (Isspam && rawMessage.Channel is SocketGuildChannel guildChannel)
            {
                if (_guilds.Exists(x => x.DiscordID == guildChannel.Guild.Id) && rawMessage.Author is SocketGuildUser guildUser)
                {
                    var guild = _guilds.Find(x => x.DiscordID == guildChannel.Guild.Id);
                    var rank = new List<IRank>(guild.Ranks).Find(x => x.Default);
                    var role = new List<IRole>(guildChannel.Guild.Roles).Find(x => x.Id == rank.DiscordID);
                    await _services.GetRequiredService<ServerProtectionService>().MuteAsync(guildUser, role);
                }
                
                return;
            }
        }
    }
}
