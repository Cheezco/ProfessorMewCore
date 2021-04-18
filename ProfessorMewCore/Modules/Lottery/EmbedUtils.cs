using Discord;
using ProfessorMewData.Interfaces.Guild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfessorMewCore.Modules.Lottery
{
    public class EmbedUtils
    {
        /// <summary>
        /// Creates lottery winner embed
        /// </summary>
        /// <param name="winners"><see cref="ILotteryUser">ILotteryUsers</see> whose information will be used</param>
        /// <param name="prizes">Lottery prizes</param>
        /// <param name="imageUrl">Winner embed image url</param>
        /// <returns>Lottery winner embed</returns>
        public static Embed CreateWinnerEmbed(List<ILotteryUser> winners, List<string> prizes, string imageUrl)
        {
            if(winners is null || prizes is null || string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new NullReferenceException();
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle("The Crystal Wolves lottery")
                .WithImageUrl(imageUrl);

            for(int i = 0; i < prizes.Count; i++)
            {
                embedBuilder.AddField(prizes[i], winners[i].Name);
            }

            return embedBuilder.Build();
        }
        /// <summary>
        /// Creates participant embed
        /// </summary>
        /// <param name="users"><see cref="ILotteryUser">whose information will be used</see></param>
        /// <returns>Lottery participant embed</returns>
        public static Embed CreateParticipantEmbed(List<ILotteryUser> users)
        {
            if(users is null)
            {
                throw new NullReferenceException();
            }

            int totalSum = users.Sum(user => user.Tickets);
            int nextWeeksPrize = (int)Math.Ceiling(totalSum * 0.9);
            int guildGold = (int)Math.Floor(totalSum * 0.1);
            string participants = string.Empty;

            users.ForEach(user => participants += $"{user.Name} - {user.Tickets}  <:Gold:684861847786356736> \n");

            return new EmbedBuilder()
                .WithTitle("The Crystal Wolves lottery")
                .WithDescription("Each ticket costs 1 <:Gold:684861847786356736>")
                .WithFooter(
                    new EmbedFooterBuilder()
                    .WithText("Weekly lottery"))
                .WithCurrentTimestamp()
                .WithColor(Discord.Color.Gold)
                .AddField("Next week's prize pool", nextWeeksPrize + " <:Gold:684861847786356736>")
                .AddField("Guild will receive", guildGold + " <:Gold:684861847786356736>")
                .AddField("Participants", participants)
                .Build();

        }
    }
}
