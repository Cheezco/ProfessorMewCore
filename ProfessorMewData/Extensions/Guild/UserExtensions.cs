using Discord.Commands;
using Discord.WebSocket;
using ProfessorMewData.Interfaces.Guild;
using System;
using System.Collections.Generic;

namespace ProfessorMewData.Extensions.Guild
{
    public static class UserExtensions
    {
        private const string _defaultAvatarUrl = @"https://cdn.discordapp.com/avatars/345219128745000971/0ad6c46bfc2805c3e9e45f270e7bf303.png?size=128";
        public static void AddPoints(this IUser user, int points, bool addToMonthly)
        {
            points = Math.Abs(points);
            user.TotalPoints += points;

            if (!addToMonthly) return;

            user.MonthPoints += points;
        }
        public static void ReducePoints(this IUser user, int points, bool reduceMonthly)
        {
            points = Math.Abs(points);
            user.TotalPoints -= points;

            if (!reduceMonthly) return;

            user.MonthPoints -= points;
        }
        public static string GetName(this IUser user, SocketCommandContext context)
        {
            var discordUser = context.Client.GetUser(user.DiscordID);
            var guildUser = context.Guild.GetUser(user.DiscordID);

            if (discordUser is null && guildUser is null)
            {
                return user.DiscordID.ToString();
            }

            if (context.IsPrivate || guildUser is null)
            {
                return discordUser.Username;
            }

            return string.IsNullOrEmpty(guildUser.Nickname) ? guildUser.Username : guildUser.Nickname;

        }
        public static void SetNames(this List<IUser> users, SocketCommandContext context, bool overrideCurrentName = true)
        {
            foreach (var user in users)
            {
                if (!overrideCurrentName && !string.IsNullOrWhiteSpace(user.Name)) continue;

                user.Name = GetName(user, context);
            }
        }
        public static string GetAvatarUrl(this IUser user, SocketCommandContext context)
        {
            var discordUser = context.Client.GetUser(user.DiscordID);

            if (discordUser is null)
            {
                return _defaultAvatarUrl;
            }

            var imageUrl = discordUser.GetAvatarUrl();

            return string.IsNullOrEmpty(imageUrl) ? _defaultAvatarUrl : imageUrl;
        }
        public static bool RankedChanged(this IUser user)
        {
            if (user.Rank.MaxPoints < user.TotalPoints || user.Rank.MinPoints > user.TotalPoints)
            {
                return true;
            }
            return false;
        }

        public static void RemoveNonExistingUsers(this List<IUser> users, SocketCommandContext context)
        {
            RemoveNonExistingUsers(users, context.Guild, context.Client);
        }

        public static void RemoveNonExistingUsers(this List<IUser> users, SocketGuild guild, DiscordSocketClient client)
        {
            var usersToRemove = new List<IUser>();
            foreach (var user in users)
            {
                var discordUser = client.GetUser(user.DiscordID);
                var guildUser = guild.GetUser(user.DiscordID);

                if (discordUser is not null && guildUser is not null) continue;

                usersToRemove.Add(user);
            }

            usersToRemove.ForEach(x => users.Remove(x));
        }
    }
}
