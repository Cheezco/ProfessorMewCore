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
        /// <summary>
        /// Tries to add discord role
        /// </summary>
        /// <param name="guildUser"><see cref="IGuildUser"/> that will receive new role</param>
        /// <param name="role"><see cref="IGuildRole"/> that will be added to <paramref name="guildUser"/></param>
        /// <returns><see cref="KeyValuePair"/>. Key shows whether role was added. Value shows result message or error.</returns>
        public static async Task<KeyValuePair<bool, string>> TryAddRoleAsync(IGuildUser guildUser, IGuildRole role)
        {
            if(role is null)
            {
                return new KeyValuePair<bool, string>(false, "Error.");
            }

            return await TryAddRoleAsync(guildUser, role.DiscordID);
        }
        /// <summary>
        /// Tries to add discord role
        /// </summary>
        /// <param name="guildUser"><see cref="IGuildUser"/> that will receive new role</param>
        /// <param name="rank"><see cref="IRank"/> that will be added to <paramref name="guildUser"/></param>
        /// <returns><see cref="KeyValuePair"/>. Key shows whether role was added. Value shows result message or error.</returns>
        public static async Task<KeyValuePair<bool, string>> TryAddRoleAsync(IGuildUser guildUser, IRank rank)
        {
            if(rank is null)
            {
                return new KeyValuePair<bool, string>(false, "Error.");
            }

            return await TryAddRoleAsync(guildUser, rank.DiscordID);
        }
        /// <summary>
        /// Deletes messages in specified <see cref="ITextChannel"/>
        /// </summary>
        /// <param name="textChannel"><see cref="ITextChannel"/> where messages will be deleted</param>
        /// <param name="messageCount">Amount of messages that will be deleted</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="textChannel"/> is <see cref="null"/></exception>
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
        /// <summary>
        /// Deletes message in <paramref name="textChannel"/>
        /// </summary>
        /// <param name="textChannel"><see cref="ITextChannel"/> where message will be deleted</param>
        /// <param name="messageID">ID of the message that will be deleted</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="textChannel"/> is <see cref="null"/></exception>
        public static async Task DeleteMessageAsync(ITextChannel textChannel, ulong messageID)
        {
            if (textChannel is null)
            {
                throw new NullReferenceException();
            }

            var message = await textChannel.GetMessageAsync(messageID);

            if (message is null) return;

            await message.DeleteAsync();
        }
        /// <summary>
        /// Gets guild with <paramref name="discordID"/> from <paramref name="guilds"/>
        /// </summary>
        /// <param name="guilds">List of guilds that will be searched</param>
        /// <param name="discordID">Guild ID</param>
        /// <returns><see cref="SocketGuild"/> with <paramref name="discordID"/></returns>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="guilds"/> is null</exception>
        /// <exception cref="ProfessorMewData.Exceptions.Guild.ProfessorMewException">Thrown when guild is not found</exception>
        public static SocketGuild GetGuild(IReadOnlyCollection<SocketGuild> guilds, ulong discordID)
        {
            if(guilds is null)
            {
                throw new NullReferenceException();
            }
            var guild = guilds.FirstOrDefault(x => x.Id == discordID);
            if(guild is null)
            {
                throw new ProfessorMewData.Exceptions.Guild.ProfessorMewException("Failed to find a guild with specified id.");
            }
            return guild;
        }
        /// <summary>
        /// Tries to add discord role
        /// </summary>
        /// <param name="guildUser"><see cref="IGuildUser"/> that will receive new role</param>
        /// <param name="roleID">ID of the role that will be added to <paramref name="guildUser"/></param>
        /// <returns><see cref="KeyValuePair"/>. Key shows whether role was added. Value shows result message or error.</returns>
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
