using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Poll
{
    public static class EmbedUtils
    {
        public static Embed CreatePollEmbed(Poll poll)
        {
            string choices = string.Empty;
            for(int i = 0; i< poll.Choices.Count; i++)
            {
                choices += $"\n{Poll.Emotes[i].Discord} - {poll.Choices[i]}";
            }

            return new EmbedBuilder()
                .WithTitle(poll.Title)
                .WithDescription(choices)
                .WithFooter("Vote by reacting to this message")
                .Build();
        }

        public static Embed CreatePollResultEmbed(Poll poll, Dictionary<IEmote, int> votes)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Poll results")
                .WithDescription(poll.Title);

            string title;
            string value;
            for(int i = 0; i < votes.Count; i++)
            {
                var vote = votes.ElementAt(i);
                title = $"{vote.Key} {poll.Choices[i]}";
                value = $"Votes: {vote.Value}";
                embedBuilder.AddField(title, value, true);
            }

            return embedBuilder.Build();
        }
    }
}
