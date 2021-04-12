using Discord;
using System;
using System.Collections.Generic;

namespace ProfessorMewCore.Modules.GW2Achievements
{
    public static class EmbedUtils
    {
        public static Embed CreateDailyEmbed(Dictionary<string, string> dailyData, string title)
        {
            if(dailyData is null || dailyData.Count == 0)
            {
                throw new NullReferenceException();
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithCurrentTimestamp()
                .WithColor(Discord.Color.Gold);

            foreach(var dailyCategory in dailyData)
            {
                if (string.IsNullOrEmpty(dailyCategory.Value)) continue;

                embedBuilder.AddField(dailyCategory.Key, dailyCategory.Value);
            }

            return embedBuilder.Build();
        }
    }
}
