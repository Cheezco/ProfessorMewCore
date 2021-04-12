using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2SeaCore.Endpoints.V2.AchievementEndpoint.Models;

namespace ProfessorMewCore.Modules.GW2Achievements
{
    public class TaskUtils
    {
        public static Dictionary<string, string> GetDailyMessageDataAsync(DailyAchievements dailies, List<Achievement> achievements)
        {
            return new Dictionary<string, string>()
            {
                { "PvE", GetAchievementDatas(dailies.PVE, achievements) },
                { "PvP", GetAchievementDatas(dailies.PVP, achievements) },
                { "WvW", GetAchievementDatas(dailies.WVW, achievements) },
                { "Fractals", GetAchievementDatas(dailies.Fractals, achievements) },
                { "Special", GetAchievementDatas(dailies.Special, achievements) },
            };
        }
        public static List<int> GetAchievementIDs(List<DailyAchievement> dailies)
        {
            var ids = new List<int>();
            dailies.ForEach(x => ids.Add(x.ID));

            return ids;
        }

        private static string GetAchievementDatas(List<DailyAchievement> dailies, List<Achievement> achievements)
        {
            var ids = GetAchievementIDs(dailies);
            string dailyMessagePart = string.Empty;

            if (ids is null || ids.Count == 0) return dailyMessagePart;

            var achievementNames = achievements
                .Where(x => ids.Any(y => y == x.ID))
                .Select(x => x.Name)
                .ToList();

            achievementNames.ForEach(x => dailyMessagePart += x + "\n");
            achievements.RemoveAll(x => achievementNames.Any(y => y == x.Name));

            return dailyMessagePart;
        }
    }
}
