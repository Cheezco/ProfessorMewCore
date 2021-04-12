using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Exceptions.Guild;
using ProfessorMewData.Interfaces.Guild;
using ProfessorMewData.Models.Guild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfessorMewData.Extensions.Guild
{
    public static class GuildExtensions
    {
        public static IUser GetUser(this ISavedGuild guild, ulong discordID)
        {
            if (guild is null || guild.Users is null)
            {
                throw new NullReferenceException();
            }
            var user = guild.Users.FirstOrDefault(x => x.DiscordID == discordID);
            if (user is null)
            {
                throw new ProfessorMewException("Failed to find user with a given id.");
            }

            return user;
        }

        public static List<IUser> GetUsers(this ISavedGuild guild, params PointDisplay[] pointDisplays)
        {
            if (guild is null || guild.Users is null)
            {
                throw new NullReferenceException();
            }
            if (pointDisplays is null)
            {
                return guild.Users.ToList();
            }
            var users = guild.Users.Where(x => pointDisplays.Contains(x.PointDisplay));
            if (users is null)
            {
                throw new ProfessorMewException("Failed to get users with given point displays.");
            }

            return users.ToList();
        }

        public static ISavedChannel GetChannel(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Channels is null)
            {
                throw new NullReferenceException();
            }
            var channel = guild.Channels.FirstOrDefault(x => x.Name.ToUpperInvariant() == name.ToUpperInvariant());
            if (channel is null)
            {
                throw new ProfessorMewException("Failed to get a channel with a given name.");
            }

            return channel;
        }

        public static ISavedChannel GetChannel(this ISavedGuild guild, ulong discordID)
        {
            if (guild is null)
            {
                throw new NullReferenceException();
            }
            var channel = guild.Channels.FirstOrDefault(x => x.DiscordID == discordID);
            if (channel is null)
            {
                throw new ProfessorMewException("Failed to get a channel with a given id.");
            }

            return channel;
        }

        public static ISavedLink GetLink(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Links is null)
            {
                throw new NullReferenceException();
            }
            var link = guild.Links.FirstOrDefault(x => x.Name == name);
            if (link is null)
            {
                throw new ProfessorMewException("Failed to get a link with a given name.");
            }

            return link;
        }

        public static ILotteryUser GetLotteryUser(this ISavedGuild guild, ulong userID)
        {
            if (guild is null || guild.LotteryUsers is null)
            {
                throw new NullReferenceException();
            }
            var user = guild.LotteryUsers.FirstOrDefault(x => x.DiscordID == userID);
            if (user is null)
            {
                throw new ProfessorMewException("Failed to get user with a given id.");
            }

            return user;
        }

        public static ISavedMessage GetMessage(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Messages is null)
            {
                throw new NullReferenceException();
            }
            var message = guild.Messages.FirstOrDefault(x => x.Name == name);
            if (message is null)
            {
                throw new ProfessorMewException("Failed to get a saved message with a given name.");
            }

            return message;
        }

        public static IRank GetRank(this ISavedGuild guild, int points)
        {
            if (guild is null || guild.Ranks is null)
            {
                throw new NullReferenceException();
            }
            var rank = guild.Ranks.FirstOrDefault(x => points >= x.MinPoints && points <= x.MaxPoints);
            if (rank is null)
            {
                throw new ProfessorMewException("Failed to get a rank with a given points.");
            }

            return rank;
        }

        public static IGuildRole GetRole(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Roles is null)
            {
                throw new NullReferenceException();
            }
            var role = guild.Roles.FirstOrDefault(x => x.Name == name);
            if (role is null)
            {
                throw new ProfessorMewException("Failed to get a role with a given name.");
            }

            return role;
        }

        public static IGuildRole GetRoleToUpper(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Roles is null)
            {
                throw new NullReferenceException();
            }
            var role = guild.Roles.FirstOrDefault(x => x.Name.ToUpperInvariant() == name.ToUpperInvariant());
            if (role is null)
            {
                throw new ProfessorMewException("Failed to get a role with a given name.");
            }

            return role;
        }

        public static IGuildRole GetRoleToLower(this ISavedGuild guild, string name)
        {
            if (guild is null || guild.Roles is null)
            {
                throw new NullReferenceException();
            }
            var role = guild.Roles.FirstOrDefault(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant());
            if (role is null)
            {
                throw new ProfessorMewException("Failed to get a role with a given name.");
            }

            return role;
        }

        public static ISavedGuild GetGuild(this List<ISavedGuild> guilds, ulong id)
        {
            if (guilds is null || guilds.Any() == false)
            {
                throw new NullReferenceException();
            }
            var guild = guilds.FirstOrDefault(x => x.DiscordID == id);
            if (guild is null)
            {
                throw new ProfessorMewException("Failed to get a guild with a given id.");
            }

            return guild;
        }

        public static SavedGuild GetGuild(this List<SavedGuild> guilds, ulong id)
        {
            if (guilds is null || guilds.Any() == false)
            {
                throw new NullReferenceException();
            }
            var guild = guilds.FirstOrDefault(x => x.DiscordID == id);
            if (guild is null)
            {
                throw new ProfessorMewException("Failed to get a guild with a given id.");
            }

            return guild;
        }

        public static IRank GetDefaultRank(this ISavedGuild guild)
        {
            if (guild is null)
            {
                throw new NullReferenceException();
            }

            var rank = guild.Ranks.FirstOrDefault(X => X.Default);
            if (rank is null)
            {
                throw new ProfessorMewException("Failed to get default rank.");
            }

            return rank;
        }
    }
}
