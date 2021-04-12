using Discord.Commands;
using ProfessorMewCore.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2SeaCore;
using System.Net.Http;

namespace ProfessorMewCore.Modules.GW2Achievements
{
    public class GW2AchievementModule : ModuleBase<SocketCommandContext>
    {
        public HttpClient Client { get; set; }

        [Command("Dailies")]
        [LimitChannel]
        public async Task GetDailiesAsync()
        {
            var client = new GW2ApiClient(Client);
            var dailies = await client.AchievementEndpoint.GetDailyAchievementsAsync();
            var ids = new List<int>();
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.Fractals));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.PVE));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.PVE));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.WVW));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.Special));
            var achievements = await client.AchievementEndpoint.GetAchievementsAsync(ids);
            var dailyData = TaskUtils.GetDailyMessageDataAsync(dailies, achievements);

            await ReplyAsync(embed: EmbedUtils.CreateDailyEmbed(dailyData, "Today's dailies"));
        }
        
        [Command("TDailies")]
        [LimitChannel]
        public async Task GetTomorrowsDailiesAsync()
        {
            var client = new GW2ApiClient(Client);
            var dailies = await client.AchievementEndpoint.GetTomorrowDailyAchievementsAsync();
            var ids = new List<int>();
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.Fractals));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.PVE));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.PVE));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.WVW));
            ids.AddRange(TaskUtils.GetAchievementIDs(dailies.Special));
            var achievements = await client.AchievementEndpoint.GetAchievementsAsync(ids);
            var dailyData = TaskUtils.GetDailyMessageDataAsync(dailies, achievements);

            await ReplyAsync(embed: EmbedUtils.CreateDailyEmbed(dailyData, "Tomorrows's dailies"));
        }
    }
}
