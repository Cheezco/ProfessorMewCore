using Discord.Commands;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using ProfessorMewCore.Preconditions;
using System.Collections.Generic;
using ProfessorMewData.Contexts;
using ProfessorMewData.Extensions.Guild;
using ProfessorMewData.Models.Guild;
using ProfessorMewData.Enums.Guild;

namespace ProfessorMewCore.Modules.Roles
{
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        [Command("Role")]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddRoleAsync(string roleName)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Roles)
                    .Include(x => x.Ranks)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());
                if(guild is null)
                {
                    await ReplyAsync("Error");
                    return;
                }
                var role = guild.GetRoleToUpper(roleName);
                if(role is null)
                {
                    await ReplyAsync("Role not found");
                    return;
                }
                var response = await Misc.DiscordUtils.TryAddRoleAsync(Context.User as SocketGuildUser, role);
                await ReplyAsync(response.Value);

                if(role.AddUserToDatabase)
                {
                    var user = await context.Users
                        .SingleOrDefaultAsync(x => x.DBDiscordID == Context.User.Id.ToString() && x.Guild == guild);
                    if (user is not null) return;

                    var defaultRank = guild.GetDefaultRank();

                    user = new User()
                    {
                        DiscordID = Context.User.Id,
                        TotalPoints = 0,
                        MonthPoints = 0,
                        LastUpdate = DateTime.UtcNow,
                        PointDisplay = PointDisplay.ShowAll,
                        Privileges = Privileges.None,
                        Rank = defaultRank,
                        Guild = guild
                    };
                    await context.AddAsync(user);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
