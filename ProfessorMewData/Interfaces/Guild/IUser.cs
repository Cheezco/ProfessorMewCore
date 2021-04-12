using ProfessorMewData.Enums.Guild;
using System;

namespace ProfessorMewData.Interfaces.Guild
{
    public interface IUser : ISavedEntity
    {
        int TotalPoints { get; set; }
        int MonthPoints { get; set; }
        DateTime LastUpdate { get; set; }
        PointDisplay PointDisplay { get; set; }
        Privileges Privileges { get; set; }

        ISavedGuild Guild { get; set; }
        IRank Rank { get; set; }
        ILotteryUser LotteryUser { get; set; }

        int GuildRanking { get; set; }
        string AvatarUrl { get; set; }
        string Name { get; set; }
    }
}
