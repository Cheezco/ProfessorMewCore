using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ProfessorMewCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Preconditions
{
    public class LimitChannelAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.Channel is SocketGuildChannel guildChannel)
            {
                bool canExecute = services.GetRequiredService<GuildService>()
                    .CanExecuteCommand(guildChannel.Guild.Id, guildChannel.Id);
                if(canExecute)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                return Task.FromResult(PreconditionResult.FromError("Cannot execute commands in that channel."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
