using Discord.Commands;
using ProfessorMewCore.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfessorMewData.Contexts;

namespace ProfessorMewCore.Modules.Testing
{
    public class TestingModule : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        [LimitChannel]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong");
        }

        [Command("Exception")]
        [DebugOnlyCommand]
        public async Task TestExceptionAsync()
        {
            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                /*var guild = await context.Guilds
                    .Include(x => x.Roles)
                    .Include(x => x.Ranks)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());
                guild.TestException();*/
            }
        }
        [Command("EF")]
        [DebugOnlyCommand]
        public async Task TestQueriesAsync()
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                /*var userRank = context.Users.AsQueryable()
                    .Where(x => x.Guild.DBDiscordID == Context.Guild.Id.ToString())
                    .GroupBy(x => x.TotalPoints)
                    .Select(x => new { ID = x.Key, Count = x.Count() }).FirstOrDefault();

                await ReplyAsync(userRank.Count.ToString());*/
                

                /*var userRank = context.Users.AsQueryable()
                    .GroupBy(x => x.TotalPoints)
                    .Concat(numList)*/

                var entries =
                    from entry in context.Users.AsQueryable()
                    orderby entry.TotalPoints descending
                    select entry;
                var entriesEnum = entries.ToAsyncEnumerable();

                int ranking = 1;
                await foreach(var en in entriesEnum)
                {
                    if (en.DiscordID == Context.User.Id) break;

                    ranking++;
                }
            }
        }
    }
}
