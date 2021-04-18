using Discord;
using Discord.WebSocket;
using ProfessorMewData.Interfaces.Guild;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ProfessorMewCore.Misc
{
    public class DiscordUtils
    {
        public static async Task<KeyValuePair<bool, string>> TryAddRoleAsync(IGuildUser guildUser, IGuildRole role)
        {
            if(role is null)
            {
                return new KeyValuePair<bool, string>(false, "Error.");
            }

            return await TryAddRoleAsync(guildUser, role.DiscordID);
        }

        public static async Task<KeyValuePair<bool, string>> TryAddRoleAsync(IGuildUser guildUser, IRank rank)
        {
            if(rank is null)
            {
                return new KeyValuePair<bool, string>(false, "Error.");
            }

            return await TryAddRoleAsync(guildUser, rank.DiscordID);
        }

        public static async Task DeleteMessagesAsync(ITextChannel textChannel, int messageCount)
        {
            if(textChannel is null)
            {
                throw new NullReferenceException();
            }

            var messages = await textChannel.GetMessagesAsync(messageCount).FlattenAsync();

            if (messages is null || !messages.Any()) return;

            await textChannel.DeleteMessagesAsync(messages);
        }

        public static SocketGuild GetGuild(IReadOnlyCollection<SocketGuild> guilds, ulong discordID)
        {
            if(guilds is null || guilds.Count == 0)
            {
                throw new NullReferenceException();
            }
            var guild = guilds.FirstOrDefault(x => x.Id == discordID);
            if(guild is null)
            {
                throw new Exceptions.ProfessorMewException("Failed to find a guild with given id.");
            }
            return guild;
        }

        private static async Task<KeyValuePair<bool, string>> TryAddRoleAsync(IGuildUser guildUser, ulong roleID)
        {
            if(guildUser is null)
            {
                return new KeyValuePair<bool, string>(false, "Error.");
            }
            if(guildUser.RoleIds.Any(x => x == roleID))
            {
                return new KeyValuePair<bool, string>(false, "User already has that role.");
            }
            var discordRole = guildUser.Guild.Roles.FirstOrDefault(x => x.Id == roleID);
            if (discordRole is null)
            {
                return new KeyValuePair<bool, string>(false, "Role not found.");
            }

            await guildUser.AddRoleAsync(discordRole);
            return new KeyValuePair<bool, string>(true, "Role added.");
        }
    }
}
