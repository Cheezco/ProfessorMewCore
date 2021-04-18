using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using ProfessorMewCore.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Contexts;
using ProfessorMewData.Models.Guild;
using ProfessorMewData.Extensions.Guild;
using System.Linq;

namespace ProfessorMewCore.Modules.Points
{
    public class PointsModule : ModuleBase<SocketCommandContext>
    {
        public IServiceProvider Services { get; set; }

        [Command("Transfer")]
        [RequirePermission(Privileges.Superadmin)]
        public async Task TransferAsync(ulong guildID)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines($"{AppDomain.CurrentDomain.BaseDirectory}Data/TransferData.txt");
                using (var context = new GuildContext())
                {
                    await context.Database.EnsureCreatedAsync();

                    /*var guild = await context.Guilds
                        .Include(x => x.Ranks)
                        .Include(x => x.Users)
                        .ThenInclude(x => x.Rank)
                        .SingleOrDefaultAsync(x => x.DBDiscordID == guildID.ToString());*/

                    var query =
                        from entry in context.Guilds.AsQueryable()
                        where entry.DBDiscordID == guildID.ToString()
                        select entry;
                    var guild = await query.ToAsyncEnumerable().SingleOrDefaultAsync();

                    foreach (string line in lines)
                    {
                        string[] values = line.Split(';');
                        string id = values[0];
                        if(!int.TryParse(values[1], out int totalPoints))
                        {
                            Console.WriteLine("Failed to parse totalPoints");
                        }
                        if(!int.TryParse(values[2], out int monthPoints))
                        {
                            Console.WriteLine("Failed to parse monthPoints");
                        }
                        if(!DateTime.TryParse(values[3], out DateTime lastUpdate))
                        {
                            lastUpdate = DateTime.UtcNow;
                        }
                        PointDisplay pointDisplay = (PointDisplay)Enum.Parse(typeof(PointDisplay), values[4]);

                        var user = new User()
                        {
                            DiscordID = ulong.Parse(id),
                            TotalPoints = totalPoints,
                            MonthPoints = monthPoints,
                            LastUpdate = lastUpdate,
                            PointDisplay = pointDisplay,
                            Privileges = Privileges.None,
                            Rank = guild.GetRank(totalPoints),
                            Guild = guild
                        };
                        await context.Users.AddAsync(user);
                    }
                    await context.SaveChangesAsync();
                }
                await ReplyAsync("Transfer completed");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                await ReplyAsync("Transfer failed");
            }
        }
        [Command("MyPoints")]
        [LimitChannel]
        [RequireContext(ContextType.Guild)]
        public async Task GetPlayerProfileAsync()
        {
            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var users = context.Users
                    .Include(x => x.Rank)
                    .Where(x => x.Guild.DBDiscordID == Context.Guild.Id.ToString() && (x.PointDisplay != PointDisplay.HideAll || x.PointDisplay != PointDisplay.HideTotal))
                    .OrderByDescending(x => x.TotalPoints)
                    .AsAsyncEnumerable();

                ProfessorMewData.Interfaces.Guild.IUser user = null;
                int ranking = 0;
                await foreach(var entry in users)
                {
                    if(entry.DiscordID == Context.User.Id)
                    {
                        user = entry;
                        user.GuildRanking = ranking;
                        break;
                    }

                    ranking++;
                }

                if (user is null)
                {
                    await ReplyAsync("User was not found in the database.");
                    return;
                }
                user.AvatarUrl = user.GetAvatarUrl(Context);
                user.Name = user.GetName(Context);
                string path = PointsProfile.CreatePlayerProfile(user);
                try
                {
                    await Context.Channel.SendFileAsync(path);
                }
                catch (Exception)
                {
                    await Context.Channel.SendMessageAsync(embed: EmbedUtils.CreatePointsProfileEmbed(user));
                }
            }
        }
        [Priority(1)]
        [Command("Add")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddPointsAsync(ulong discordID, int points, bool addToMonthly = true)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Ranks)
                    .Include(x => x.Links)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild was not found in the database");
                    return;
                }
                var user = await context.Users
                    .Include(x => x.Rank)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == discordID.ToString() && x.Guild == guild);

                if (user is null)
                {
                    await ReplyAsync("User was not found in the database.");
                    return;
                }
                user.AddPoints(points, addToMonthly);
                if(user.RankedChanged())
                {
                    var newRank = guild.GetRank(user.TotalPoints);
                    var link = guild.GetLink("Rankup");
                    user.Rank = newRank;
                    var guildUser = Context.Guild.GetUser(user.DiscordID);
                    await guildUser.SendMessageAsync(embed: EmbedUtils.CreateRankUpEmbed(user, link.URL));
                    await ReplyAsync((await Misc.DiscordUtils.TryAddRoleAsync(guildUser, newRank)).Value);
                }
                await context.SaveChangesAsync();
            }

            await ReplyAsync($"{points} added to <@{discordID}>");
        }
        [Priority(0)]
        [Command("Add")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddPointsAsync(IGuildUser guildUser, int points, bool addToMonthly = true)
        {
            _ = guildUser ?? throw new NullReferenceException("AddPointsAsync - guildUser");

            await AddPointsAsync(guildUser.Id, points, addToMonthly);
        }
        [Priority(1)]
        [Command("Reduce")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ReducePointsAsync(ulong discordID, int points, bool reduceMonthly = true)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Ranks)
                    .Include(x => x.Links)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild was not found in the database.");
                    return;
                }
                var user = await context.Users
                    .Include(x => x.Rank)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == discordID.ToString() && x.Guild == guild);

                if (user is null)
                {
                    await ReplyAsync("User was not found in the database");
                    return;
                }
                user.ReducePoints(points, reduceMonthly);
                if (user.RankedChanged())
                {
                    var newRank = guild.GetRank(user.TotalPoints);
                    var link = guild.GetLink("Rankdown");
                    user.Rank = newRank;
                    var guildUser = Context.Guild.GetUser(user.DiscordID);
                    await guildUser.SendMessageAsync(embed: EmbedUtils.CreateRankDownEmbed(user, link.URL));
                    await ReplyAsync((await Misc.DiscordUtils.TryAddRoleAsync(guildUser, newRank)).Value);
                }
                await context.SaveChangesAsync();
            }

            await ReplyAsync($"<@{discordID}> lost {points} points");
        }
        [Priority(0)]
        [Command("Reduce")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ReducePointsAsync(IGuildUser guildUser, int points, bool reduceMonthly = true)
        {
            _ = guildUser ?? throw new NullReferenceException("ReducePointsAsync - guildUser");

            await ReducePointsAsync(guildUser.Id, points, reduceMonthly);
        }
        [Priority(1)]
        [Command("New", RunMode = RunMode.Async)]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task CreateNewUserAsync(ulong discordID)
        {
            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Ranks)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }
                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == discordID.ToString() && x.Guild == guild);

                if(user is not null)
                {
                    await ReplyAsync("User already exists.");
                    return;
                }

                user = new User()
                {
                    DiscordID = discordID,
                    TotalPoints = 0,
                    MonthPoints = 0,
                    LastUpdate = DateTime.UtcNow,
                    PointDisplay = PointDisplay.ShowAll,
                    Privileges = Privileges.None,
                    Rank = guild.GetDefaultRank(),
                    Guild = guild
                };
                await context.AddAsync(user);
                await context.SaveChangesAsync();
            }

            await ReplyAsync("User added");
        }
        [Priority(0)]
        [Command("New")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task CreateNewUserAsync(IGuildUser guildUser)
        {
            _ = guildUser ?? throw new NullReferenceException("CreateNewUserAsync - guildUser");

            await CreateNewUserAsync(guildUser.Id);
        }
        [Priority(1)]
        [Command("Remove")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task RemoveUserAsync(ulong discordID)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild was not found in the database.");
                    return;
                }
                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == discordID.ToString() && x.Guild == guild);

                if (user is null)
                {
                    await ReplyAsync("User not found in the database.");
                    return;
                }

                context.Remove(user);
                await context.SaveChangesAsync();
            }

            await ReplyAsync("User removed");
        }
        [Priority(0)]
        [Command("Remove")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task RemoveUserAsync(IGuildUser guildUser)
        {
            _ = guildUser ?? throw new NullReferenceException("RemoveUserAsync - guildUser");

            await RemoveUserAsync(guildUser.Id);
        }
        [Priority(1)]
        [Command("Display")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ChangePointDisplayAsync(ulong discordID, PointDisplay pointDisplay)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild was not found in the database");
                    return;
                }
                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == discordID.ToString() && x.Guild == guild);

                if (user is null)
                {
                    await ReplyAsync("User not found in the database");
                    return;
                }

                user.PointDisplay = pointDisplay;
                await context.SaveChangesAsync();
            }

            await ReplyAsync("Value changed");
        }
        [Priority(0)]
        [Command("Display")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ChangePointDisplayAsync(IGuildUser guildUser, PointDisplay pointDisplay)
        {
            _ = guildUser ?? throw new NullReferenceException("ChangePointDisplayAsync - guildUser");

            await ChangePointDisplayAsync(guildUser.Id, pointDisplay);
        }

        [Command("Permissions")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ChangePermissionsAsync(IGuildUser guildUser, Privileges privileges)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild was not found in the database");
                    return;
                }
                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild == guild);
                var executingUser = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.User.Id.ToString() && x.Guild == guild);

                if (user is null || executingUser is null)
                {
                    await ReplyAsync("User not found in the database");
                    return;
                }
                if(executingUser.Privileges < privileges)
                {
                    await ReplyAsync("Insufficient permissions");
                    return;
                }
                user.Privileges = privileges;

                await context.SaveChangesAsync();
            }
            await Services.GetRequiredService<Services.UserPrivilegeService>().UpdateAsync();
            await ReplyAsync("Value changed");
        }

        [Command("UpdateRanks", RunMode = RunMode.Async)]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task UpdateRanksAsync()
        {
            using(var context  = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .Include(x => x.Ranks)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild was not found in the database");
                    return;
                }

                var users = context.Users
                    .Include(x => x.Rank)
                    .Where(x => x.Guild.DBDiscordID == Context.Guild.Id.ToString())
                    .OrderByDescending(x => x.TotalPoints)
                    .AsAsyncEnumerable();

                await ReplyAsync("Updating user ranks");

                await foreach(var user in users)
                {
                    if (!user.RankedChanged()) continue;

                    var newRank = guild.GetRank(user.TotalPoints);
                    if(newRank is null)
                    {
                        await ReplyAsync($"Failed to update {user.DiscordID} rank");
                        continue;
                    }

                    user.Rank = newRank;
                    var guildUser = Context.Guild.GetUser(user.DiscordID);

                    if(!(await Misc.DiscordUtils.TryAddRoleAsync(guildUser, newRank)).Key)
                    {
                        await ReplyAsync($"Updated {user.DiscordID} rank, but failed to get guild user. Update user's roles manually");
                        await Task.Delay(100);
                        continue;
                    }
                    await ReplyAsync($"Updated {user.DiscordID} rank");
                    await Task.Delay(100);
                }
                await context.SaveChangesAsync();
            }
            await ReplyAsync("Finished updating user ranks");
        }

        [Command("GetP")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task GetPlayerAsync(IGuildUser guildUser)
        {
            using (var context = new GuildContext())
            {
                var users = context.Users
                    .Include(x => x.Rank)
                    .Where(x => x.Guild.DBDiscordID == Context.Guild.Id.ToString() && (x.PointDisplay != PointDisplay.HideAll || x.PointDisplay != PointDisplay.HideTotal))
                    .OrderByDescending(x => x.TotalPoints)
                    .AsAsyncEnumerable();

                ProfessorMewData.Interfaces.Guild.IUser user = null;
                int ranking = 0;
                await foreach (var entry in users)
                {
                    if (entry.DiscordID == guildUser.Id)
                    {
                        user = entry;
                        user.GuildRanking = ranking;
                        break;
                    }

                    ranking++;
                }

                if (user is null)
                {
                    await ReplyAsync("User was not found in the database.");
                    return;
                }

                var embed = EmbedUtils.CreatePlayerDataEmbed(user);
                await ReplyAsync(embed: embed);
            }
        }

        [Command("Update", RunMode = RunMode.Async)]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task UpdatePointLeaderboardAsync()
        {
            const int topLeaderboardCount = 10;
            const int monthlyLeaderboardCount = 3;

            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Users)
                    .ThenInclude(x => x.Rank)
                    .Include(x => x.Channels)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild was not found in the database");
                    return;
                }
                var savedChannel = guild.GetChannel("Points");
                var textChannel = Context.Guild.GetTextChannel(savedChannel.DiscordID);
                await Misc.DiscordUtils.DeleteMessagesAsync(textChannel, 2);

                var playersMonthly = guild.GetUsers(PointDisplay.ShowAll, PointDisplay.HideTotal).ToList();
                var playersTotal = guild.GetUsers(PointDisplay.ShowAll, PointDisplay.HideMonthly).ToList();
                if (playersMonthly is not null || playersMonthly.Count > 0)
                {
                    playersMonthly.RemoveNonExistingUsers(Context);
                    playersMonthly.SetNames(Context, false);
                    playersMonthly.Sort(delegate (ProfessorMewData.Interfaces.Guild.IUser u1, ProfessorMewData.Interfaces.Guild.IUser u2)
                    {
                        return u2.MonthPoints.CompareTo(u1.MonthPoints);
                    });
                }
                if (playersTotal is not null || playersTotal.Count > 0)
                {
                    playersTotal.RemoveNonExistingUsers(Context);
                    playersTotal.SetNames(Context, false);
                    playersTotal.Sort(delegate (ProfessorMewData.Interfaces.Guild.IUser u1, ProfessorMewData.Interfaces.Guild.IUser u2)
                    {
                        return u2.TotalPoints.CompareTo(u1.TotalPoints);
                    });
                }

                var monthlyEmbed = EmbedUtils.CreatePointLeaderboardEmbed("The Crystal Wolves monthly leaderboard", "Here you can see the top 3 players who have earned the most points this month so far", Discord.Color.DarkBlue, playersMonthly, monthlyLeaderboardCount);
                var totalEmbed = EmbedUtils.CreatePointLeaderboardEmbed("The Crystal Wolves leaderboard", "Here you can see the top 10 players in our guild", Discord.Color.DarkPurple, playersTotal, topLeaderboardCount);

                if(monthlyEmbed is not null)
                {
                    await textChannel.SendMessageAsync(embed: totalEmbed);
                }
                if(totalEmbed is not null)
                {
                    await textChannel.SendMessageAsync(embed: monthlyEmbed);
                }
            }
        }
    }
}
