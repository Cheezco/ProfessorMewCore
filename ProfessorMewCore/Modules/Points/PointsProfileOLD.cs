using ProfessorMewData.Interfaces.Guild;
using ProfessorMewData.Models.Guild;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Points
{
    public static class PointsProfileOLD
    {
        private static readonly string _imagePath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Images/";

        public static string CreatePlayerProfile(IUser user)
        {
            using(var background = new Bitmap(Image.FromFile($"{_imagePath}background.png"), 500, 400))
            {
                using(var image = new ProfileImageOLD(user, background))
                {
                    image.labels = GetUserLabels(user);
                    image.Render($"{_imagePath}{user.DiscordID}.jpeg");
                }
            }

            return $"{_imagePath}{user.DiscordID}.jpeg";
        }

        private static List<Label> GetUserLabels(IUser user)
        {
            var totalPointsIcon = Image.FromFile(_imagePath + "bag.png");
            var rankingIcon = Image.FromFile(_imagePath + "stats.png");
            var monthlyPointsIcon = Image.FromFile(_imagePath + "gold.png");
            var rankIcon = Image.FromFile(_imagePath + "level_up.png");
            var lastUpdateIcon = Image.FromFile(_imagePath + "feather.png");


            var labels = new List<Label>
            {
                new Label()
                {
                    Key = "Total Points",
                    Value = user.TotalPoints + " / " + (user.Rank.MaxPoints >= 99999 ?  "♾️" : user.Rank.MaxPoints),
                    Icon = totalPointsIcon
                },
                new Label()
                {
                    Key = "Monthly Points",
                    Value = user.MonthPoints.ToString(CultureInfo.InvariantCulture),
                    Icon = monthlyPointsIcon
                },
                new Label()
                {
                    Key = "Ranking",
                    Value = user.GuildRanking.ToString(CultureInfo.InvariantCulture),
                    Icon = rankingIcon
                },
                new Label()
                {
                    Key = "Rank",
                    Value = user.Rank.Name,
                    Icon = rankIcon
                },
                new Label()
                {
                    Key = "Last update",
                    Value = user.LastUpdate.Date.ToString("MM/dd", CultureInfo.InvariantCulture),
                    Icon = lastUpdateIcon
                }
            };

            return labels;
        }
    }
}
