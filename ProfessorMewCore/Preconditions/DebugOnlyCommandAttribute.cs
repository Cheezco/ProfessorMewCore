using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Preconditions
{
    public class DebugOnlyCommandAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
#if DEBUG
return Task.FromResult(PreconditionResult.FromSuccess());
#endif

            return Task.FromResult(PreconditionResult.FromError("This command is debug only command."));
        }
    }
}
