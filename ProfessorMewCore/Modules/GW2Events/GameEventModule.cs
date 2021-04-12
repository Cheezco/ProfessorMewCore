using Discord.Commands;
using ProfessorMewCore.Preconditions;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.GW2Events
{
    public class GameEventModule : ModuleBase<SocketCommandContext>
    {
        [Command("Event")]
        [LimitChannel]
#pragma warning disable IDE0060 // Remove unused parameter
        public async Task GetGameEventAsync([Remainder] string eventName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            await ReplyAsync("Command not implemented.");
        }

        [Command("Events")]
        [LimitChannel]
        public async Task GetGameEventsAsync()
        {
            await ReplyAsync("Command not implemented.");
        }
    }
}
