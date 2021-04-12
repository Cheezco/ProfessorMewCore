using Discord;
using System.Collections.Generic;
using System.Globalization;
using ProfessorMewData.Extensions.Guild;

namespace ProfessorMewCore.Modules.Points
{
    public static class EmbedUtils
    {
        public static Embed CreateRankUpEmbed(ProfessorMewData.Interfaces.Guild.IUser user, string imageUrl)
        {
            return new EmbedBuilder()
                .WithTitle($"Congratulations you reached {user.Rank.Name} rank!")
                .WithDescription($"Now you have: {user.TotalPoints} points")
                .WithColor(user.Rank.GetDiscordColor())
                .WithImageUrl(imageUrl)
                .Build();
        }

        public static Embed CreateRankDownEmbed(ProfessorMewData.Interfaces.Guild.IUser user, string imageUrl)
        {
            return new EmbedBuilder()
                .WithTitle($"You were demoted\nNow you have {user.Rank.Name} rank")
                .WithDescription($"Now you have: {user.TotalPoints} points")
                .WithColor(user.Rank.GetDiscordColor())
                .WithImageUrl(imageUrl)
                .Build();
        }

        public static Embed CreatePointsProfileEmbed(ProfessorMewData.Interfaces.Guild.IUser user)
        {
            return new EmbedBuilder()
                .WithColor(user.Rank.GetDiscordColor())
                .WithCurrentTimestamp()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(user.Name)
                    .WithIconUrl(user.AvatarUrl))
                .WithFooter(new EmbedFooterBuilder()
                    .WithText("Due to technical difficulties image was replaced.\nIt will return once everything is fixed."))
                .AddField("💰 Total points", $"```{user.TotalPoints}/{user.Rank.MaxPoints}```", true)
                .AddField("📅 Monthly points", $"```{user.MonthPoints}```", true)
                .AddField("📊 Ranking", $"```{user.GuildRanking}```")
                .AddField("🆙 Rank", $"```{user.Rank.Name}```")
                .AddField("🔄 Last update", string.Format(CultureInfo.InvariantCulture, "```{0:yy/MM}```", user.LastUpdate))
                .Build();
        }

        public static Embed CreatePlayerDataEmbed(ProfessorMewData.Interfaces.Guild.IUser user)
        {
            return new EmbedBuilder()
                .WithTitle(user.Name)
                .WithColor(user.Rank.GetDiscordColor())
                .AddField("Rank", user.Rank.Name)
                .AddField("Guild ranking", user.GuildRanking)
                .AddField("Total points", user.TotalPoints)
                .AddField("Month points", user.MonthPoints)
                .AddField("Last time points added", user.LastUpdate)
                .Build();
        }

        public static Embed CreatePointLeaderboardEmbed(string title, string description, Discord.Color color, List<ProfessorMewData.Interfaces.Guild.IUser> users, int fieldCount)
        {
            if(users is null || users.Count == 0)
            {
                return null;
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color);
            foreach(var user in users)
            {
                if(embedBuilder.Fields.Count < fieldCount &&
                    embedBuilder.Fields.Count < EmbedBuilder.MaxFieldCount)
                {
                    string fieldValue = string.Format(CultureInfo.InvariantCulture,
                        "Rank: {0}\nTotal points: {1}\nThis month points: {2}\nLast time points added: {3:yyyy/MM/dd}",
                        user.Rank.Name, user.TotalPoints, user.MonthPoints, user.LastUpdate);
                    embedBuilder.AddField(user.Name, fieldValue);
                }
                else
                {
                    break;
                }
            }

            return embedBuilder.Build();
        }
    }
}
