using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ProfessorMewCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Services
{
    public class LoggingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        public LoggingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
        }

        public void Initialize()
        {
            _discord.Log += LogAsync;
            _commands.Log += LogAsync;
        }
        
        private Task LogAsync(LogMessage message)
        {

            if(message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else if(message.Exception is not ProfessorMewException)
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }

            return Task.CompletedTask;
        }
    }
}
