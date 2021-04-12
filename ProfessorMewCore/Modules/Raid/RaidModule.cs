using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using ProfessorMewCore.Misc;
using ProfessorMewCore.Preconditions;
using ProfessorMewCore.Extensions;
using ProfessorMewData.Contexts;
using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Enums.Raid;
using ProfessorMewData.Models.Raid;
using ProfessorMewData.Extensions.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paging;
using ProfessorMewData.Interfaces.Raid;
using Discord.Addons.Interactive;

namespace ProfessorMewCore.Modules.Raid
{
    [Group("DPS")]
    public class RaidModule : InteractiveBase
    {
        [Priority(1)]
        [Command]
        [LimitChannel]
        [RequireContext(ContextType.Guild)]
        public async Task GetPlayerDPSProfileAsync(int pageIndex = 1)
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Links)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild was not found in the database.");
                    return;
                }

                var user = await context.Users
                    .Include(x => x.Records)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.User.Id.ToString() && x.Guild == guild);
                var benches = context.RaidBenches
                    .AsQueryable()
                    .Where(x => x.Guild.DBDiscordID == guild.DBDiscordID)
                    .AsAsyncEnumerable();
                if(user is null)
                {
                    await ReplyAsync(embed: EmbedUtils.CreatePlayerNotFoundEmbed());
                    return;
                }
                
                //user.Records.UpdateRecordStatuses(guild.Benches);
                var paging = new Pager<IRaidRecord>(user.Records.ToList(), 5);
                var records = paging.GetPage(pageIndex - 1).ToList();
                await records.UpdateRecordStatusesAsync(benches);

                await ReplyAsync(embed: EmbedUtils.CreateDPSProfileEmbed(records, pageIndex, paging.PageCount));
            }
        }

        [Priority(0)]
        [Command(RunMode = RunMode.Async)]
        [LimitChannel]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        public async Task GetPlayerDPSProfileAsync(IGuildUser guildUser, int pageIndex = 1)
        {
            using (var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Links)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild was not found in the database.");
                    return;
                }

                var user = await context.Users
                    .Include(x => x.Records)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild == guild);
                var benches = context.RaidBenches
                    .AsQueryable()
                    .Where(x => x.Guild.DBDiscordID == guild.DBDiscordID)
                    .ToAsyncEnumerable();
                if (user is null)
                {
                    await ReplyAsync(embed: EmbedUtils.CreatePlayerNotFoundEmbed());
                    return;
                }

                //user.Records.UpdateRecordStatuses(guild.Benches);
                var paging = new Pager<IRaidRecord>(user.Records.ToList(), 5);
                var records = paging.GetPage(pageIndex - 1).ToList();
                await records.UpdateRecordStatusesAsync(benches);

                await ReplyAsync(embed: EmbedUtils.CreateDPSProfileEmbed(records, pageIndex, paging.PageCount));
            }
        }

        [Command("Init")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task InitializeAsync()
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is not null)
                {
                    await ReplyAsync("Guild already exists in the database.");
                    return;
                }

                var raidGuild = new RaidGuild()
                {
                    DiscordID = Context.Guild.Id
                };

                context.Guilds.Add(raidGuild);
                await context.SaveChangesAsync();
            }

            await ReplyAsync("Completed.");
        }

        [Command("Add")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddDPSAsync(IGuildUser guildUser, int dps, Role role, Class userClass, Specialization spec = Specialization.Base, double boonUptime = 0, string characterName = "None" )
        {
            boonUptime.Clamp(0, 100);

            using (var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild was not found in the database.");
                    return;
                }

                var user = await context.Users
                    .Include(x => x.Records)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild == guild);

                if(user is null)
                {
                    await ReplyAsync("User was not found in the database.");
                    return;
                }

                var record = new RaidRecord()
                {
                    User = user,
                    CharacterName = characterName,
                    DPS = dps,
                    Role = role,
                    Class = userClass,
                    Specialization = spec,
                    BoonUptime = boonUptime
                };

                user.Records.Add(record);

                await context.SaveChangesAsync();
            }

            await ReplyAsync("Entry added.");
        }

        [Command("Remove")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task RemoveDPSAsync(IGuildUser guildUser, int id = -1)
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var user = await context.Users
                    .Include(x => x.Records)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild == guild);

                if(user is null)
                {
                    await ReplyAsync("User not found in the database.");
                    return;
                }

                if(id == -1)
                {
                    context.Users.Remove(user);
                }
                else
                {
                    var record = await context.Records
                        .AsQueryable()
                        .FirstOrDefaultAsync(x => x.ID == id);
                    if(record is null)
                    {
                        await ReplyAsync("Invalid id.");
                        return;
                    }
                    user.Records.Remove(record);
                }

                await context.SaveChangesAsync();
            }

            await ReplyAsync("Entry deleted");

        }

        [Command("New")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task CreateNewDPSUserAsync(IGuildUser guildUser, string accountName = "None")
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }
                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID ==  guildUser.Id.ToString() && x.Guild == guild);

                if(user is not null)
                {
                    await ReplyAsync("User already exists.");
                    return;
                }

                user = new RaidUser()
                {
                    DiscordID = guildUser.Id,
                    AccountName = accountName,
                    Guild = guild
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            await ReplyAsync("User added.");
        }

        /*[Command("Get")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task GetRaidUserAsync(IGuildUser guildUser)
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Benches)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var user = await context.Users
                    .Include(x => x.Records)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild == guild);

                if(user is null)
                {
                    await ReplyAsync("User not found in the database.");
                    return;
                }

                user.Records.UpdateRecordStatuses(guild.Benches);
                await ReplyAsync(embed: EmbedUtils.CreateDPSProfileEmbed(user));
            }
        }*/

        [Command("AddBench")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddBenchAsync(int dps, Role role, Class userClass, Specialization spec, double boonUptime = 0, double scale = 0.8)
        {
            scale.Clamp(0, 1);
            boonUptime.Clamp(0, 100);

            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    //.Include(x => x.Benches)
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var exists = context.RaidBenches.Any(x => x.Guild == guild &&
                    x.Role == role &&
                    x.Class == userClass &&
                    x.Specialization == spec);
                if(exists)
                {
                    await ReplyAsync("Bench already exists.");
                    return;
                }

                var newBench = new RaidBench()
                {
                    Role = role,
                    Class = userClass,
                    Specialization = spec,
                    DPS = dps,
                    Scale = scale,
                    Guild = guild,
                    BoonUptime = boonUptime
                };

                context.RaidBenches.Add(newBench);
                await context.SaveChangesAsync();
            }

            await ReplyAsync("Bench added.");
        }
        [Command("RemoveBench")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task RemoveBenchAsync(int id)
        {
            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    //.Include(x => x.Benches)
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var bench = await context.RaidBenches
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.ID == id);

                if(bench is null)
                {
                    await ReplyAsync("Bench not found.");
                    return;
                }

                context.RaidBenches.Remove(bench);
                await context.SaveChangesAsync();
            }
        }
        [Command("Benches")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task GetBenchesAsync(int pageIndex = 1)
        {
            /*await ReplyAsync("Not implemented yet");
            return;*/

            using(var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Benches)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var paging = new Pager<IRaidBench>(guild.Benches.ToList(), 5);

                await ReplyAsync(embed: EmbedUtils.CreateBenchEmbed(paging.GetPage(pageIndex - 1), pageIndex, paging.PageCount));
            }
        }
        [Priority(1)]
        [Command("Edit")]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task EditDPSAsync()
        {
            await ReplyAsync("What do you want to edit?(Respond with a number)\n1.Account name\n2.Character name");

            var editResponse = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));

            if(editResponse is null)
            {
                await ReplyAsync("Command timed out");
                return;
            }
            if(!int.TryParse(editResponse.Content, out int editNumber))
            {
                await ReplyAsync("Invalid input");
                return;
            }

            var newValueResponse = await NextMessageAsync(timeout: TimeSpan.FromMinutes(2));

            if(newValueResponse is null)
            {
                await ReplyAsync("Command timed out");
                return;
            }

            using (var context = new RaidContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Benches)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if (guild is null)
                {
                    await ReplyAsync("Guild not found.\nRun startup command first.");
                    return;
                }

                var user = await context.Users
                    .AsQueryable()
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.User.Id.ToString() && x.Guild == guild);

                switch(editNumber)
                {
                    case 1:
                        user.AccountName = newValueResponse.Content;
                        break;
                    case 2:

                        break;
                    default:
                        return;
                }
            }
        }


        [Priority(0)]
        [Command("Edit")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task EditDPSAsync(IGuildUser guildUser)
        {
            await ReplyAsync("Not implemented yet");
        }
    }
}
