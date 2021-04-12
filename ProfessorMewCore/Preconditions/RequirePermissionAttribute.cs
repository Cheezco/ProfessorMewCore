using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ProfessorMewCore.Services;
using ProfessorMewData.Enums.Guild;

namespace ProfessorMewCore.Preconditions
{
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        private readonly Privileges _privileges;

        public RequirePermissionAttribute(Privileges privileges) => _privileges = privileges;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if(context.User is SocketGuildUser guildUser)
            {
                var privilege = services.GetRequiredService<UserPrivilegeService>().GetUserPrivilege(guildUser.Guild.Id, guildUser.Id);
                if(privilege >= _privileges)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                return Task.FromResult(PreconditionResult.FromError("You don't have permissions to use this command"));
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command"));
            }
        }
    }
}
