using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Interfaces.Guild;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfessorMewData.Models.Guild
{
    public class User : SavedEntity, IUser
    {
        public int TotalPoints { get; set; }

        public int MonthPoints { get; set; }

        public DateTime LastUpdate { get; set; }

        public PointDisplay PointDisplay { get; set; }

        public Privileges Privileges { get; set; }

        public ISavedGuild Guild { get; set; }

        public IRank Rank { get; set; }

        public ILotteryUser LotteryUser { get; set; }
        [NotMapped]
        public int GuildRanking { get; set; }
        [NotMapped]
        public int PointsRequiredForNextRankup => Rank.MaxPoints;
        [NotMapped]
        public string AvatarUrl { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
