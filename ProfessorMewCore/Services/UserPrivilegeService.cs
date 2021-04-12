using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProfessorMewData.Contexts;
using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewCore.Services
{
    public class UserPrivilegeService
    {
        private readonly Dictionary<ulong, Dictionary<ulong, Privileges>> entries;

        public UserPrivilegeService()
        {
            entries = new Dictionary<ulong, Dictionary<ulong, Privileges>>();
        }

        public async Task InitializeAsync()
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guilds = await context.Guilds
                    .Include(x => x.Users)
                    .ToListAsync();

                foreach (var guild in guilds)
                {
                    if (guild.Users is null || guild.Users.Count == 0) continue;

                    var users = new List<IUser>(guild.Users);
                    var guildUsers = new Dictionary<ulong, Privileges>();

                    foreach (var user in users)
                    {
                        if (guildUsers.ContainsKey(user.DiscordID)) continue;

                        guildUsers.Add(user.DiscordID, user.Privileges);
                    }
                    entries.Add(guild.DiscordID, guildUsers);
                }
            }
        }
        /*public async Task UpdateAsync(ulong guildID)
        {
            SavedGuild guild;
            using(var context = new MainDatabase.Contexts.GuildContext())
            {
                var guilds = await context.Guilds.ToListAsync();

                guild = guilds.FirstOrDefault(x => x.DiscordID == guildID);
            }

            if (guild is null) return;

            bool guildExists = entries.ContainsKey(guild.DiscordID);
            foreach(var user in guild.Users)
            {
                if(guildExists && entries[guild.DiscordID].ContainsKey(user.DiscordID))
                {
                    entries[guild.DiscordID][user.DiscordID] = user.Privileges;
                }
                else if(guildExists)
                {
                    entries[guild.DiscordID].Add(user.DiscordID, user.Privileges);
                }
                else
                {
                    entries.Add(guild.DiscordID, new Dictionary<ulong, Privileges>());
                    guildExists = true;
                }
            }
        }*/

        public async Task UpdateAsync()
        {
            entries.Clear();
            await InitializeAsync();
        }

        public Privileges GetUserPrivilege(ulong guildID, ulong userID)
        {
            if(!entries.ContainsKey(guildID))
            {
                return Privileges.None;
            }
            if(!entries[guildID].ContainsKey(userID))
            {
                return Privileges.None;
            }
            return entries[guildID][userID];
        }
    }
}
