using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using ProfessorMewCore.Preconditions;
using ProfessorMewData.Enums.Guild;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Poll
{
    [Group("Poll")]
    public class PollModule : InteractiveBase
    {
        [Command("Create", RunMode = RunMode.Async)]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task CreatePollAsync(int choiceCount, int duration, ITextChannel channel)
        {
            choiceCount = choiceCount > 9 ? 9 : choiceCount;

            await ReplyAsync("Enter poll title");
            var titleResponse = await NextMessageAsync();

            if (titleResponse is null || string.IsNullOrEmpty(titleResponse.Content))
            {
                await ReplyAsync("Error occured");
                return;
            }
            if(PollManager.Instance.Contains(titleResponse.Content))
            {
                await ReplyAsync("Poll with the specified title already exists");
                return;
            }

            var poll = new Poll(duration, titleResponse.Content);
            for(int i = 0; i < choiceCount; i++)
            {
                await ReplyAsync($"Enter poll choice {i + 1}");
                var choiceResponse = await NextMessageAsync();
                if(choiceResponse is null || string.IsNullOrEmpty(choiceResponse.Content))
                {
                    await ReplyAsync("Error occured");
                    return;
                }
                poll.AddChoice(choiceResponse.Content);
            }

            PollManager.Instance.Add(poll);
            await poll.StartAsync(channel);
        }

        [Command("Stop")]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task StopPollAsync(string title)
        {
            PollManager.Instance.Stop(title);

            await ReplyAsync("Poll stopped");
        }

        [Command("Extend")]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ExtendPollAsync(string title, int duration)
        {
            PollManager.Instance.Extend(title, duration);

            await ReplyAsync("Poll extended");
        }

        [Command("End")]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task EndPollAsync(string title)
        {
            await PollManager.Instance.EndAsync(title);

            await ReplyAsync("Poll ended");
        }
    }
}
